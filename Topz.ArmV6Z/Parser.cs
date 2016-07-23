using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Parser for ArmV6Z assembly code.
    /// </summary>
    internal sealed class Parser
    {
        private LexicalAnalyzer<TokenType> analyzer;

        private Program program;

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
            Token<TokenType> keyword = analyzer.Next();
            if (keyword.Text != Keywords.Procedure)
                throw new ParsingException(keyword.Position.ToString($"Expected the '{Keywords.Procedure}' keyword."));

            Token<TokenType> identifier = analyzer.Next();
            if (identifier.Type != TokenType.Identifier)
                throw new ParsingException(identifier.Position.ToString("Expected an identifier."));

            Token<TokenType> start = analyzer.Next();
            if (start.Text != Symbols.StartOfBlock)
                throw new ParsingException(start.Position.ToString($"Expected the '{Symbols.StartOfBlock}' symbol."));

            Procedure procedure = new Procedure(identifier.Position, identifier.Text);
            while (!analyzer.EndOfInput)
            {
                if (analyzer.NextIs(Symbols.EndOfBlock))
                    break;

                procedure.Instructions.Add(Instruction());
            }

            Token<TokenType> end = analyzer.Next();
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
            Label label = null;

            Token<TokenType> token = analyzer.Next();
            if (token.Type == TokenType.Identifier)
            {
                Token<TokenType> symbol = analyzer.Next();
                if (symbol.Text != Symbols.EndOfLable)
                    throw new ParsingException(symbol.Position.ToString($"Expected the '{Symbols.EndOfLable}' symbol."));

                label = new Label(token.Position, token.Text);
                token = analyzer.Next();
            }

            if (token.Type != TokenType.Mnemonic)
                throw new ParsingException(token.Position.ToString("Mnemonic expected."));

            Mnemonic mnemonic = new Mnemonic(token.Text, token.Position);
            switch (mnemonic.RawName)
            {
                case Mnemonic.Adc:
                    return AddWithCarryInstruction(label, mnemonic);
                case Mnemonic.B:
                    return BranchInstruction(label, mnemonic);
                default:
                    throw new ParsingException(analyzer.Position.ToString("Unknown instruction"));
            }   
        }

        /// <summary>
        /// Parses a <see cref="AddWithCarryInstruction"/>.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <returns></returns>
        private AddWithCarryInstruction AddWithCarryInstruction(Label label, Mnemonic mnemonic)
        {
            var instruction = new AddWithCarryInstruction(label, mnemonic);
            instruction.Desitnation = RegisterOperand();

            Token<TokenType> separator = analyzer.Next();
            if (separator.Text != Symbols.ListItemSeparator)
                throw new ParsingException(separator.Position.ToString($"Expected a '{Symbols.ListItemSeparator}'."));

            instruction.FirstOperand = RegisterOperand();

            separator = analyzer.Next();
            if (separator.Text != Symbols.ListItemSeparator)
                throw new ParsingException(separator.Position.ToString($"Expected a '{Symbols.ListItemSeparator}'."));

            instruction.ShifterOperand = ShifterOperand();

            return instruction;
        }

        /// <summary>
        /// Parses a <see cref="BranchInstruction"/>.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <returns></returns>
        private BranchInstruction BranchInstruction(Label label, Mnemonic mnemonic)
        {
            Token<TokenType> integer = analyzer.Next();
            if (integer.Type != TokenType.Integer)
                throw new ParsingException(integer.Position.ToString("Expected an integer."));

            TargetOperand operand = new TargetOperand(integer.Position, int.Parse(integer.Text.Substring(1)));

            return new BranchInstruction(label, mnemonic, operand);
        }

        /// <summary>
        /// Parses a register operand.
        /// </summary>
        /// <returns>The parsed operand.</returns>
        private RegisterOperand RegisterOperand()
        {
            Token<TokenType> register = analyzer.Next();
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
            Token<TokenType> operand = analyzer.Next();
            if (operand.Type == TokenType.Integer)
                return new ShifterOperand(operand.Position, int.Parse(operand.Text.Substring(1)));
            if (operand.Type == TokenType.Register)
            {
                if (analyzer.NextIs(TokenType.RegisterShifter))
                {
                    Token<TokenType> shifter = analyzer.Next();
                    if (shifter.Type != TokenType.RegisterShifter)
                        throw new ParsingException(shifter.Position.ToString("Expected a register shifter."));

                    Token<TokenType> immediate = analyzer.Next();
                    if (immediate.Type != TokenType.Integer)
                        throw new ParsingException(immediate.Position.ToString("Expected an integer."));

                    return new ShifterOperand(operand.Position, int.Parse(operand.Text.Substring(1)), shifter.Text);
                }
                else
                    return new ShifterOperand(operand.Position, operand.Text);
            }

            throw new ParsingException(operand.Position.ToString("Expected a shifter operand."));
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
    }
}