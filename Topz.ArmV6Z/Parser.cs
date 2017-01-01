using Pote.Text;
using System;
using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Parser for ArmV6Z assembly code.
    /// </summary>
    internal sealed class Parser
    {
        private Dictionary<string, Func<Label, Mnemonic, Instruction>> table;

        private LexicalAnalyzer<TokenType> analyzer;

        private Program program;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        public Parser()
        {
            table = new Dictionary<string, Func<Label, Mnemonic, Instruction>>()
            {
                { Mnemonic.Adc, Format1<AddWithCarryInstruction> },
                { Mnemonic.Add, Format1<AddInstruction> },
                { Mnemonic.And, Format1<AndInstruction> },
                { Mnemonic.B, Format2<BranchInstruction> },
                { Mnemonic.Bic, Format1<BitClearInstruction> },
                { Mnemonic.Bkpt, Format3<BreakPointInstruction> },
                { Mnemonic.Bx, Format4<BranchAndExchangeInstruction> },
                { Mnemonic.Bxj, Format4<BranchAndChangeToJazelleInstruction> },
                { Mnemonic.Clz, Format5<CountLeadingZeroesInstruction> },
                { Mnemonic.Cmn, Format6<CompareNegativeInstruction> },
                { Mnemonic.Cmp, Format6<CompareInstruction> },
                { Mnemonic.Cpy, Format5<CopyInstruction> },
                { Mnemonic.Eor, Format1<ExclusiveOrInstruction> },
                { Mnemonic.Ldr, Format7<LoadRegisterInstruction> }
            };
        }

        /// <summary>
        /// Parses a program into an ast.
        /// </summary>
        /// <param name="source">The source to parse.</param>
        /// <returns>The parsed ast.</returns>
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
                if (analyzer.NextIs(Keywords.Procedure))
                    Procedure();
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

            Symbol(Symbols.StartOfBlock);

            var procedure = new Procedure(identifier.Position, identifier.Text);
            while (!analyzer.EndOfInput)
            {
                if (analyzer.NextIs(Symbols.EndOfBlock))
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

            var end = analyzer.Next();
            if (end.Text != Symbols.EndOfBlock)
                throw new ParsingException(end.Position.ToString($"Expected the '{Symbols.EndOfBlock}' symbol."));

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
        private Instruction Instruction()
        {
            var label = (Label)null;

            var token = analyzer.Next();
            if (token.Type == TokenType.Identifier)
            {
                Symbol(Symbols.EndOfLable);

                label = new Label(token.Position, token.Text);
                token = analyzer.Next();
            }

            if (token.Type != TokenType.Mnemonic)
                throw new ParsingException(token.Position.ToString("Mnemonic expected."));

            try
            {
                var mnemonic = new Mnemonic(token.Text, token.Position);
                return table[mnemonic.RawName](label, mnemonic);
            }
            catch (KeyNotFoundException)
            {
                throw new ParsingException(analyzer.Position.ToString("Unknown instruction"));
            }
        }

        /// <summary>
        /// <para>Parses an instruction with the following format.</para>
        /// <para>mnemonic, register, register, shifting operand</para>
        /// </summary>
        /// <typeparam name="T">The type of instruction.</typeparam>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <returns>The parsed instruction.</returns>
        private T Format1<T>(Label label, Mnemonic mnemonic) where T : Format1Instruction
        {
            var r1 = RegisterOperand();
            Symbol(Symbols.ListItemSeparator);

            var r2 = RegisterOperand();
            Symbol(Symbols.ListItemSeparator);

            var shifter = ShifterOperand();

            return (T)Activator.CreateInstance(typeof(T), label, mnemonic, r1, r2, shifter);
        }

        /// <summary>
        /// <para>Parses an instruction with the following format.</para>
        /// <para>mnemonic, target address</para>
        /// </summary>
        /// <typeparam name="T">The type of instruction.</typeparam>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <returns>The parsed instruction.</returns>
        private T Format2<T>(Label label, Mnemonic mnemonic) where T : Format2Instruction
        {
            var integer = Integer();

            try
            {
                var operand = new TargetOperand(integer.Position, int.Parse(integer.Text));

                return (T)Activator.CreateInstance(typeof(T), label, mnemonic, operand);
            }
            catch (OverflowException)
            {
                throw new ParsingException(integer.Position.ToString("The integer can't fit within 32 bits."));
            }
        }

        /// <summary>
        /// <para>Parses an instruction with the following format.</para>
        /// <para>mnemonic, immediate 16</para>
        /// </summary>
        /// <typeparam name="T">The type of instruction.</typeparam>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <returns>The parsed instruction.</returns>
        private T Format3<T>(Label label, Mnemonic mnemonic) where T : Format3Instruction
        {
            var integer = Integer();

            try
            {
                var operand = new Immediate16Operand(integer.Position, ushort.Parse(integer.Text));

                return (T)Activator.CreateInstance(typeof(T), label, mnemonic, operand);
            }
            catch (OverflowException)
            {
                throw new ParsingException(integer.Position.ToString("The integer can't fit within 16 bits."));
            }
        }

        /// <summary>
        /// <para>Parses an instruction with the following format.</para>
        /// <para>mnemonic, register</para>
        /// </summary>
        /// <typeparam name="T">The type of instruction.</typeparam>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <returns>The parsed instruction.</returns>
        private T Format4<T>(Label label, Mnemonic mnemonic) where T : Format4Instruction
        {
            return (T)Activator.CreateInstance(typeof(T), label, mnemonic, RegisterOperand());
        }

        /// <summary>
        /// <para>Parses an instruction with the following format.</para>
        /// <para>mnemonic, register, register</para>
        /// </summary>
        /// <typeparam name="T">The type of instruction.</typeparam>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <returns>The parsed instruction.</returns>
        private T Format5<T>(Label label, Mnemonic mnemonic) where T : Format5Instruction
        {
            var r1 = RegisterOperand();
            Symbol(Symbols.ListItemSeparator);

            var r2 = RegisterOperand();

            return (T)Activator.CreateInstance(typeof(T), label, mnemonic, r1, r2);
        }

        /// <summary>
        /// <para>Parses an instruction with the following format.</para>
        /// <para>mnemonic, register, shifting operand</para>
        /// </summary>
        /// <typeparam name="T">The type of instruction.</typeparam>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <returns>The parsed instruction.</returns>
        private T Format6<T>(Label label, Mnemonic mnemonic) where T : Format6Instruction
        {
            var register = RegisterOperand();
            Symbol(Symbols.ListItemSeparator);

            var shifter = ShifterOperand();

            return (T)Activator.CreateInstance(typeof(T), label, mnemonic, register, shifter);
        }

        /// <summary>
        /// <para>Parses an instruction with the following format.</para>
        /// <para>mnemonic, register, addressing mode</para>
        /// </summary>
        /// <typeparam name="T">The type of instruction.</typeparam>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <returns>The parsed instruction.</returns>
        private T Format7<T>(Label label, Mnemonic mnemonic) where T : Format7Instruction
        {
            var register = RegisterOperand();
            Symbol(Symbols.ListItemSeparator);

            var addressingMode = AddressingModeOperand();

            return (T)Activator.CreateInstance(typeof(T), label, mnemonic, register, addressingMode);
        }

        /// <summary>
        /// Parses a register operand.
        /// </summary>
        /// <returns>The parsed operand.</returns>
        private RegisterOperand RegisterOperand()
        {
            var register = analyzer.Next();
            if (register.Type != TokenType.Register)
                throw new ParsingException(register.Position.ToString("Expected a register."));

            return new RegisterOperand(register.Position, register.Text);
        }

        /// <summary>
        /// Parses a shifter operand.
        /// </summary>
        /// <returns>The shifter operand.</returns>
        private ShifterOperand ShifterOperand()
        {
            var operand = analyzer.Next();
            if (operand.Type == TokenType.Integer)
                return new ShifterOperand(operand.Position, int.Parse(operand.Text));
            if (operand.Type == TokenType.Register)
            {
                if (analyzer.NextIs(TokenType.RegisterShifter))
                {
                    var shifter = analyzer.Next();
                    if (shifter.Type != TokenType.RegisterShifter)
                        throw new ParsingException(shifter.Position.ToString("Expected a register shifter."));

                    var immediate = analyzer.Next();
                    if (immediate.Type != TokenType.Integer)
                        throw new ParsingException(immediate.Position.ToString("Expected an integer."));

                    return new ShifterOperand(operand.Position, int.Parse(operand.Text), shifter.Text);
                }
                else
                    return new ShifterOperand(operand.Position, operand.Text);
            }

            throw new ParsingException(operand.Position.ToString("Expected a shifter operand."));
        }

        /// <summary>
        /// Parses an adressing mode.
        /// </summary>
        /// <returns>The parsed addressing mode.</returns>
        private AddressingModeOperand AddressingModeOperand()
        {
            Symbol(Symbols.LeftSquareBracket);
            var register = Register();

            Symbol(Symbols.ListItemSeparator);
            var offset = Integer();

            Symbol(Symbols.RightSquareBracket);

            return new ImmediateOffsetOperand(register.Text, int.Parse(offset.Text));
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
        /// <returns>The parsed integer.</returns>
        /// <exception cref="ParsingException">
        /// The next token was not an integer.
        /// </exception>
        private Token<TokenType> Integer()
        {
            var integer = analyzer.Next();
            if (integer.Type != TokenType.Integer)
                throw new ParsingException(integer.Position.ToString("Expected an integer."));

            return integer;
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
        private Token<TokenType> Identifier()
        {
            var identifier = analyzer.Next();
            if (identifier.Type != TokenType.Identifier)
                throw new ParsingException(identifier.Position.ToString("Expected an identifier."));

            return identifier;
        }

        /// <summary>
        /// Checks if the next token is a register
        /// or throws an exception.
        /// </summary>
        /// <returns>The parsed register.</returns>
        /// <exception cref="ParsingException">
        /// The next token was not an register.
        /// </exception>
        private Token<TokenType> Register()
        {
            var register = analyzer.Next();
            if (register.Type != TokenType.Register)
                throw new ParsingException(register.Position.ToString("Expected a register."));

            return register;
        }
    }
}