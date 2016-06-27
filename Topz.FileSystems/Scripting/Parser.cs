using Pote.Text;
using System;
using System.Collections.Generic;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// Parser for disk script.
    /// </summary>
    internal class Parser
    {
        private LexicalAnalyzer<TokenType> source;

        private List<Command> commands;

        /// <summary>
        /// Parses a disk script.
        /// </summary>
        /// <param name="analyzer">The lexical analyzer that provides the tokens of the script.</param>
        /// <returns>The parsed script.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="analyzer"/> is null.
        /// </exception>
        /// <exception cref="ParsingException">
        /// An error happend while parsing.
        /// </exception>
        public IEnumerable<Command> Parse(LexicalAnalyzer<TokenType> analyzer)
        {
            if (analyzer == null)
                throw new ArgumentNullException(nameof(analyzer));

            source = analyzer;
            commands = new List<Command>();

            while (!analyzer.EndOfInput)
            {
                Token<TokenType> token = analyzer.LookAhead();
                switch (token.Text.ToLower())
                {
                    case "select":
                        Select();
                        break;
                    case "create":
                        Create();
                        break;
                }
            }

            return commands;
        }

        /// <summary>
        /// Parses a selected command.
        /// </summary>
        private void Select()
        {
            Token<TokenType> select = source.Next();
            if (select.Text.ToLower() != "select")
                throw new ParsingException(select.Position.ToString("Expected the 'select' keyword."));

            Token<TokenType> obj = source.Next();

            Token<TokenType> operand = source.Next();
            switch (obj.Text.ToLower())
            {
                case "disk":
                    if (operand.Text.ToLower() == "ask")
                        commands.Add(new SelectDiskCommand());
                    else if (operand.Type == TokenType.Integer)
                        commands.Add(new SelectDiskCommand(int.Parse(operand.Text)));
                    else if (operand.Type == TokenType.String)
                        commands.Add(new SelectDiskCommand(operand.Text));
                    else
                        throw new ParsingException(operand.Position.ToString("Expected a string, integer or the 'ask' keyword."));
                    break;
                case "partition":
                    Token<TokenType> token = source.Next();
                    int index = 0;

                    if (token.Type != TokenType.Integer)
                        throw new ParsingException(token.Position.ToString("Expected integer."));
                    else
                        index = int.Parse(token.Text);

                    if (1 > index || 4 < index)
                        throw new ParsingException(token.Position.ToString("Must be from 1 to 4."));

                    commands.Add(new SelectPartitionCommand(index));
                    break;
                default:
                    throw new ParsingException(obj.Position.ToString("Expected the 'disk' keyword."));
            }
        }

        /// <summary>
        /// Parses a create command.
        /// </summary>
        private void Create()
        {
            Token<TokenType> create = source.Next();
            if (create.Text.ToLower() != "create")
                throw new ParsingException(create.Position.ToString("Expected the 'create' keyword."));

            Token<TokenType> obj = source.Next();
            switch (obj.Text.ToLower())
            {
                case "mbr":
                    commands.Add(new CreateMbrCommand());
                    break;
                case "partition":
                    CreatePartitionCommand command = new CreatePartitionCommand();

                    Token<TokenType> token = source.Next();
                    if (token.Type != TokenType.Integer)
                        throw new ParsingException(token.Position.ToString("Expected integer."));
                    else
                        command.Index = int.Parse(token.Text);

                    if (1 > command.Index || 4 < command.Index)
                        throw new ParsingException(token.Position.ToString("Must be from 1 to 4."));

                    token = source.Next();
                    if (token.Text.ToLower() != "offset")
                        throw new ParsingException(token.Position.ToString("Expected the 'offset' keyword."));

                    token = source.Next();
                    if (token.Type != TokenType.Integer)
                        throw new ParsingException(token.Position.ToString("Expected integer."));
                    else
                        command.Offset = uint.Parse(token.Text);

                    token = source.Next();
                    if (token.Text.ToLower() != "sectors")
                        throw new ParsingException(token.Position.ToString("Expected the 'sectors' keyword."));

                    token = source.Next();
                    if (token.Type != TokenType.Integer)
                        throw new ParsingException(token.Position.ToString("Expected integer."));
                    else
                        command.Sectors = uint.Parse(token.Text);

                    commands.Add(command);
                    break;
                default:
                    throw new ParsingException(obj.Position.ToString("Expected the 'mbr' or 'partition' keyword."));
            }
        }
    }
}