using Pote.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Parser for ArmV6Z assembly code.
    /// </summary>
    internal sealed class Parser
    {
        private static readonly Regex Regex = new Regex(@"\w+|\{\w+\}|\<\w*\>|\{\<\w*\>\}|\,", RegexOptions.Compiled);

        private static readonly Dictionary<string, string> Table = new Dictionary<string, string>()
        {
            { "ADD{<cond>}{S} <Rd>, <Rn>, <shifter_operand>",   "cond 00 I 0100 S Rn Rd shifter_operand" },
            { "AND{<cond>}{S} <Rd>, <Rn>, <shifter_operand>",   "cond 00 I 0000 S Rn Rd shifter_operand" },
            { "B{L}{<cond>} <target_address>",                  "cond 101 L signed_immed_24" },
            { "CMP{<cond>} <Rn>, <shifter_operand>",            "cond 00 I 10101 Rn 0000 shifter_operand" },
            { "LDR{<cond>} <Rd>, <addressing_mode>",            "cond 01 I P U 0 W 1 Rn Rd addr_mode" },
            { "LDR{<cond>}B <Rd>, <addressing_mode>",           "cond 01 I P U 1 W 1 Rn Rd addr_mode" },
            { "LDR{<cond>}D <Rd>, <addressing_mode>",           "cond 000 P U I W 0 Rn Rd addr_mode 1101 addr_mode" },
            { "MOV{<cond>}{S} <Rd>, <shifter_operand>",         "cond 00 I 1101 S 0000 Rd shifter_operand" },
            { "STR{<cond>} <Rd>, <addressing_mode>",            "cond 01 I P U 0 W 0 Rn Rd addr_mode" },
            { "STR{<cond>}H <Rd>, <addressing_mode>",           "cond 000 P U I W 0 Rn Rd addr_mode 1011 addr_mode" },
            { "SUB{<cond>}{S} <Rd>, <Rn>, <shifter_operand>",   "cond 00 I 0010 S Rn Rd shifter_operand" },
            { "TEQ{<cond>} <Rn>, <shifter_operand>",            "cond 00 I 1001 1 Rn 0000 shifter_operand" },
            { "TST{<cond>} <Rn>, <shifter_operand>",            "cond 00 I 1000 1 Rn SBZ shifter_operand" }
        };

        private FormatMatch match;

        private LexicalAnalyzer<TokenType> analyzer;

        private Program program;

        private bool isExternal;

        /// <summary>
        /// Parses a program into an AST.
        /// </summary>
        /// <param name="source">The source to parse.</param>
        /// <returns>The parsed AST.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <exception cref="ParsingException">
        /// A problem occurred when parsing.
        /// </exception>
        public Program Parse(LexicalAnalyzer<TokenType> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            analyzer = source;

            program = new Program();
            while (!analyzer.EndOfInput)
            {
                isExternal = External();
                if (analyzer.NextIs(Keywords.Procedure))
                    Procedure();
                else if (analyzer.NextIs(Keywords.String))
                    String();
                else if (analyzer.NextIs(Keywords.Data))
                    Data();
            }

            return program;
        }

        /// <summary>
        /// Parses a procedure.
        /// </summary>
        private void Procedure()
        {
            Keyword(Keywords.Procedure);
            var identifier = Identifier();

            var procedure = new Procedure(identifier.Name, identifier.Position);
            procedure.IsExternal = isExternal;

            if (!isExternal)
            {
                while (!analyzer.EndOfInput)
                {
                    if (analyzer.NextIs(TokenType.Keyword))
                        break;

                    try
                    {
                        procedure.Instructions.Add(Instruction());
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ParsingException(ex.Message);
                    }
                }
            }

            try
            {
                program.Procedures.Add(procedure);
            }
            catch (ArgumentException ex)
            {
                throw new ParsingException(identifier.Position.ToString(ex.Message));
            }
        }

        /// <summary>
        /// Parses an instruction.
        /// </summary>
        /// <returns>The parsed instruction.</returns>
        private Instruction Instruction()
        {
            var label = Label();

            var instruction = new Instruction(Mnemonic());
            instruction.Label = label;
            instruction.Encoding = match.Encoding;

            while (match.Next != null)
            {
                switch (match.Next)
                {
                    case Symbols.Comma:
                        Symbol(Symbols.Comma);
                        break;
                    case Placeholders.Rd:
                    case Placeholders.Rn:
                        instruction.Values.Add(match.Next, Register());
                        break;
                    case Placeholders.ShifterOperand:
                        ShifterOperand(instruction);
                        break;
                    case Placeholders.TargetAddress:
                        TargetAddress(instruction);
                        break;
                    case Placeholders.AddressingMode:
                        AddressingMode(instruction);
                        break;
                    case Placeholders.Immediate16:
                        instruction.Values[Placeholders.Immediate16] = Integer(16, false);
                        break;
                    default:
                        throw new ParsingException(analyzer.LookAhead().Position.ToString($"{match.Next} expected."));
                }

                match.Current++;
            }

            return instruction;
        }

        /// <summary>
        /// Parses a piece of data.
        /// </summary>
        private void Data()
        {
        }

        /// <summary>
        /// Parses a string.
        /// </summary>
        private void String()
        {
        }

        /// <summary>
        /// Checks if the next token is an integer
        /// or throws an exception.
        /// </summary>
        /// <param name="size">The maximum size of the number in bytes.</param>
        /// <param name="isSigned">True of the number is signed: otherwise false for an unsigned number.</param>
        /// <returns>The parsed integer.</returns>
        /// <exception cref="ParsingException">
        /// The next token was not an integer. The integer 
        /// is not within the given <paramref name="size"/>.
        /// </exception>
        private Integer Integer(byte size, bool isSigned)
        {
            var integer = analyzer.Next();
            if (integer.Type != TokenType.Integer)
                throw new ParsingException(integer.Position.ToString("Expected an integer."));

            var value = int.Parse(integer.Text);
            if (!isSigned && value < 0)
                throw new ParsingException(integer.Position.ToString($"Expected an unsigned {size} bit integer."));

            if (isSigned && -Math.Pow(2, size) > value)
            {
            }
            else if (Math.Pow(2, size) - 1 < value)
                throw new ParsingException(integer.Position.ToString($"The integer must fit in {size} bytes or less."));

            if (Math.Pow(2, size) - 1 < value || isSigned && -Math.Pow(2, size) > value)
                throw new ParsingException(integer.Position.ToString($"The integer must fit in {size} bytes or less."));

            return new Integer(value, integer.Position);
        }

        /// <summary>
        /// Checks if the next token is a given symbol
        /// or throws an exception.
        /// </summary>
        /// <param name="symbol">The symbol to check for.</param>
        /// <exception cref="ParsingException">
        /// The next token was not the expected symbol.
        /// </exception>
        private void Symbol(string symbol)
        {
            var token = analyzer.Next();
            if (token.Text != symbol)
                throw new ParsingException(token.Position.ToString($"Expected a '{symbol}'."));
        }

        /// <summary>
        /// Checks if the next token is a given keyword
        /// or throws an exception.
        /// </summary>
        /// <param name="keyword">The keyword to check for.</param>
        /// <exception cref="ParsingException">
        /// The next token was not the expected keyword.
        /// </exception>
        private void Keyword(string keyword)
        {
            var token = analyzer.Next();
            if (token.Text != keyword)
                throw new ParsingException(token.Position.ToString($"Expected the '{keyword}' keyword."));
        }

        /// <summary>
        /// Checks if the next token is an identifier
        /// or throws an exception.
        /// </summary>
        /// <returns>The parsed identifier.</returns>
        /// <exception cref="ParsingException">
        /// The next token was not an identifier.
        /// </exception>
        private Identifier Identifier()
        {
            var token = analyzer.Next();
            if (token.Type != TokenType.Identifier)
                throw new ParsingException(token.Position.ToString("Expected an identifier."));

            return new Identifier(token.Text, token.Position);
        }

        /// <summary>
        /// Checks if the next token is a register
        /// or throws an exception.
        /// </summary>
        /// <returns>The parsed register.</returns>
        /// <exception cref="ParsingException">
        /// The next token was not an register.
        /// </exception>
        private Register Register()
        {
            var token = analyzer.Next();
            if (token.Type != TokenType.Register)
                throw new ParsingException(token.Position.ToString("Expected a register."));

            return new Register(token);
        }

        /// <summary>
        /// Checks if the next token is a register shifter
        /// or throws an exception.
        /// </summary>
        /// <returns>The parsed register shifter.</returns>
        /// <exception cref="ParsingException">
        /// The next token was not an register shifter.
        /// </exception>
        private RegisterShifter RegisterShifter()
        {
            var token = analyzer.Next();
            if (token.Type != TokenType.RegisterShifter)
                throw new ParsingException(token.Position.ToString("Expected a register shifter."));

            return new RegisterShifter(token);
        }

        /// <summary>
        /// Parses the next label if any.
        /// </summary>
        /// <returns>The next label; otherwise null.</returns>
        private Label Label()
        {
            if (!analyzer.NextIs(TokenType.Identifier))
                return null;

            var token = analyzer.Next();
            Symbol(Symbols.EndOfLabel);

            var label = new Label(token.Position, token.Text);
            token = analyzer.Next();

            return label;
        }

        /// <summary>
        /// Parses the next mnemonic.
        /// </summary>
        /// <returns>The next mnemonic.</returns>
        private Mnemonic Mnemonic()
        {
            var token = analyzer.Next();
            if (token.Type != TokenType.Mnemonic)
                throw new ParsingException(token.Position.ToString("Mnemonic expected."));

            var mnemonic = new Mnemonic(token.Text, token.Position);
            var postfix = analyzer.LookAhead().Text.IsBit() ? analyzer.Next() : null ;

            var entries = from row in Table
                          where row.Key.StartsWith(token.Text, StringComparison.InvariantCultureIgnoreCase)
                          select row;

            var wasSet = false;
            foreach (var entry in entries)
            {
                match = new FormatMatch();
                match.Encoding = entry.Value;
                match.Current++;

                foreach (Match m in Regex.Matches(entry.Key))
                {
                    if (m.Value.Length != 0)
                        match.Chunks.Add(m.Value);
                }

                if (postfix == null && !match.Chunks[2].IsBit() || postfix != null && match.Chunks[2].ToLower() == postfix.Text.ToLower())
                {
                    wasSet = true;
                    break;
                }
            }

            if (!wasSet)
                throw new ParsingException(token.Position.ToString("Fuck is this?"));

            while (match.Next.StartsWith("{"))
                Optional(mnemonic);

            if (postfix != null)
            {
                mnemonic.Bit = postfix.Text.ToBit();
                match.Current++;
            }

            return mnemonic;
        }

        /// <summary>
        /// Parses the next <see cref="Keywords.External"/> if any.
        /// </summary>
        /// <returns>True if the <see cref="Keywords.External"/> was parsed; otherwise false.</returns>
        private bool External()
        {
            if (!analyzer.NextIs(Keywords.External))
                return false;

            Keyword(Keywords.External);

            return true;
        }

        /// <summary>
        /// Parses the next shifter operand.
        /// </summary>
        /// <param name="instruction">The instruction to store the operand in.</param>
        private void ShifterOperand(Instruction instruction)
        {
            if (analyzer.NextIs(TokenType.Integer))
                instruction.Values.Add(Placeholders.Immediate, Integer(12, false));
            else if (analyzer.NextIs(TokenType.Register))
            {
                instruction.Values.Add(Placeholders.Rm, Register());
                if (!analyzer.NextIs(Symbols.Comma))
                    return;

                analyzer.Next();
                if (analyzer.NextIs(ArmV6Z.RegisterShifter.Rrx))
                    instruction.Values.Add(analyzer.Next().Text, null);
                else if (analyzer.NextIs(TokenType.RegisterShifter).Then(TokenType.Register))
                    instruction.Values.Add(analyzer.Next().Text, Register());
                else if (analyzer.NextIs(TokenType.RegisterShifter).Then(TokenType.Integer))
                    instruction.Values.Add(analyzer.Next().Text, Integer(12, false));
                else
                    throw new ParsingException(analyzer.LookAhead().Position.ToString("Register shifter expected."));
            }
            else
                throw new ParsingException(analyzer.LookAhead().Position.ToString($"{Placeholders.Immediate} or {Placeholders.Rm} expected."));
        }

        /// <summary>
        /// Parses the next target address operand.
        /// </summary>
        /// <param name="instruction">The instruction to store the operand in.</param>
        private void TargetAddress(Instruction instruction)
        {
            if (analyzer.NextIs(TokenType.Integer))
                instruction.Values[Placeholders.TargetAddress] = Integer(24, false);
            else if (analyzer.NextIs(TokenType.Identifier))
                instruction.Values[Placeholders.TargetAddress] = Identifier();
            else
                throw new ParsingException(analyzer.LookAhead().Position.ToString($"{Placeholders.TargetAddress} or identifier expected."));
        }

        /// <summary>
        /// Parses the next target addressing mode.
        /// </summary>
        /// <param name="instruction">The instruction to store the operand in.</param>
        private void AddressingMode(Instruction instruction)
        {
            Symbol(Symbols.LeftSquareBracket);
            instruction.Values.Add(Placeholders.Rn, Register());

            if (analyzer.NextIs(Symbols.Comma))
            {
                analyzer.Next();
                if (analyzer.NextIs(TokenType.Integer))
                {
                    instruction.Values.Add(Placeholders.Offset12, Integer(12, true));
                    Symbol(Symbols.RightSquareBracket);

                    if (analyzer.NextIs(Symbols.ExclamationMark))
                    {
                        analyzer.Next();
                        instruction.Values.Add(Symbols.ExclamationMark, null);
                    }
                }
                else if (analyzer.NextIs(Symbols.Plus).Then(TokenType.Register) || analyzer.NextIs(Symbols.Minus).Then(TokenType.Register))
                {
                    var sign = analyzer.Next();

                    instruction.Values.Add($"{sign.Text}{Placeholders.Rm}", Register());
                    if (analyzer.NextIs(Symbols.Comma))
                    {
                        analyzer.Next();
                        if (analyzer.NextIs(TokenType.RegisterShifter))
                            Shift(instruction);
                        else
                            throw new ParsingException(analyzer.LookAhead().Position.ToString("Shift expected."));

                        Symbol(Symbols.RightSquareBracket);
                        if (analyzer.NextIs(Symbols.ExclamationMark))
                        {
                            analyzer.Next();
                            instruction.Values.Add(Symbols.ExclamationMark, null);
                        }
                    }
                    else
                    {
                        Symbol(Symbols.RightSquareBracket);
                        if (analyzer.NextIs(Symbols.ExclamationMark))
                        {
                            analyzer.Next();
                            instruction.Values.Add(Symbols.ExclamationMark, null);
                        }
                    }
                }
                else
                    throw new ParsingException(analyzer.LookAhead().Position.ToString("Expected an integer or register."));
            }
            else if (analyzer.NextIs(Symbols.RightSquareBracket))
            {
                analyzer.Next();
                Symbol(Symbols.Comma);

                if (analyzer.NextIs(TokenType.Integer))
                    instruction.Values.Add(Placeholders.Offset12, Integer(12, false));
                else if (analyzer.NextIs(Symbols.Plus).Then(TokenType.Register) || analyzer.NextIs(Symbols.Minus).Then(TokenType.Register))
                {
                    var sign = analyzer.Next();
                    instruction.Values.Add($"{sign.Text}{Placeholders.Rm}", Register());

                    if (analyzer.NextIs(Symbols.Comma))
                    {
                        analyzer.Next();
                        Shift(instruction);
                    }
                }
                else
                    throw new ParsingException(analyzer.LookAhead().Position.ToString("Expected an integer or register."));
            }
        }

        /// <summary>
        /// Parses a shift.
        /// </summary>
        /// <param name="instruction">The instruction to store the operand in.</param>
        /// <remarks>See section A5.2.4.</remarks>
        private void Shift(Instruction instruction)
        {
            if (analyzer.NextIs(TokenType.RegisterShifter))
            {
                if (analyzer.NextIs(ArmV6Z.RegisterShifter.Rrx))
                    instruction.Values.Add(Placeholders.Shift, Register());
                else
                {
                    instruction.Values.Add(Placeholders.Shift, RegisterShifter());
                    if (analyzer.NextIs(ArmV6Z.RegisterShifter.Rrx))
                        throw new ParsingException(analyzer.LookAhead().Position.ToString($"{ArmV6Z.RegisterShifter.Rrx} not allowed."));

                    instruction.Values.Add(Placeholders.ShiftImmediate, RegisterShifter());
                }
            }
            else
                throw new ParsingException(analyzer.LookAhead().Position.ToString($"Expected a {Placeholders.Shift}."));
        }

        /// <summary>
        /// Parses an optional part of the instruction.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        private void Optional(Mnemonic mnemonic)
        {
            foreach (Bit bit in Enum.GetValues(typeof(Bit)))
            {
                var value = bit.ToString();
                if (match.Next == '{' + value + '}')
                {
                    if (analyzer.NextIs(value) || analyzer.NextIs(value.ToLower()))
                    {
                        analyzer.Next();
                        mnemonic.Bit |= bit;
                    }

                    match.Current++;
                }
            }

            if (match.Next == Placeholders.Condition)
            {
                if (analyzer.NextIs(TokenType.Condition))
                {
                    var token = analyzer.Next();
                    mnemonic.Condition = token.Text.ToCondition();
                }

                match.Current++;
            }
        }

        /// <summary>
        /// The match of the current instruction being parsed.
        /// </summary>
        private class FormatMatch
        {
            private List<string> chunks = new List<string>();

            /// <summary>
            /// The encoding of the instruction.
            /// </summary>
            public string Encoding
            {
                get;
                set;
            }

            /// <summary>
            /// Each chunk of the instruction format in lexical order.
            /// </summary>
            public List<string> Chunks
            {
                get
                {
                    return chunks;
                }
            }

            /// <summary>
            /// The index of the current in <see cref="Chunks"/>.
            /// </summary>
            public int Current
            {
                get;
                set;
            }

            /// <summary>
            /// The next element in <see cref="Chunks"/>.
            /// </summary>
            public string Next
            {
                get
                {
                    if (Chunks.Count <= Current)
                        return null;

                    return Chunks[Current];
                }
            }
        }
    }
}