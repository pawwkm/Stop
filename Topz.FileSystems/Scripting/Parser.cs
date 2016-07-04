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
                    case Keywords.Select:
                        Select();
                        break;
                    case Keywords.Create:
                        Create();
                        break;
                    case Keywords.Format:
                        Format();
                        break;
                    case Keywords.Move:
                        Move();
                        break;
                    default:
                        string error = string.Format("Expected the {0}, {1}, {2}, {3} or keyword.", Keywords.Select, Keywords.Create, Keywords.Format, Keywords.Move);

                        throw new ParsingException(token.Position.ToString(error));
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
            if (select.Text.ToLower() != Keywords.Select)
                throw new ParsingException(select.Position.ToString("Expected the '" + Keywords.Select + "' keyword."));

            Token<TokenType> obj = source.Next();

            Token<TokenType> operand = source.Next();
            switch (obj.Text.ToLower())
            {
                case Keywords.Disk:
                    if (operand.Text.ToLower() == Keywords.Ask)
                        commands.Add(new SelectDiskCommand());
                    else if (operand.Type == TokenType.Integer)
                        commands.Add(new SelectDiskCommand(int.Parse(operand.Text)));
                    else if (operand.Type == TokenType.String)
                        commands.Add(new SelectDiskCommand(operand.Text));
                    else
                        throw new ParsingException(operand.Position.ToString("Expected a string, integer or the '" + Keywords.Ask + "' keyword."));
                    break;
                case Keywords.Partition:
                    int index = 0;

                    if (operand.Type != TokenType.Integer)
                        throw new ParsingException(operand.Position.ToString("Expected integer."));
                    else
                        index = int.Parse(operand.Text);

                    if (index < 1 || index > 4)
                        throw new ParsingException(operand.Position.ToString("Must be from 1 to 4."));

                    commands.Add(new SelectPartitionCommand(index));
                    break;
                default:
                    throw new ParsingException(obj.Position.ToString("Expected the '" + Keywords.Disk + "' or 'partition' keyword."));
            }
        }

        /// <summary>
        /// Parses a create command.
        /// </summary>
        private void Create()
        {
            Token<TokenType> create = source.Next();
            if (create.Text.ToLower() != Keywords.Create)
                throw new ParsingException(create.Position.ToString("Expected the '" + Keywords.Create + "' keyword."));

            Token<TokenType> obj = source.Next();
            switch (obj.Text.ToLower())
            {
                case Keywords.Mbr:
                    commands.Add(new CreateMbrCommand());
                    break;
                case Keywords.Partition:
                    CreatePartitionCommand command = new CreatePartitionCommand();

                    Token<TokenType> token = source.Next();
                    if (token.Type != TokenType.Integer)
                        throw new ParsingException(token.Position.ToString("Expected integer."));
                    else
                        command.Index = int.Parse(token.Text);

                    if (command.Index < 1 || command.Index > 4)
                        throw new ParsingException(token.Position.ToString("Must be from 1 to 4."));

                    token = source.Next();
                    if (token.Text.ToLower() != Keywords.Offset)
                        throw new ParsingException(token.Position.ToString("Expected the '" + Keywords.Offset + "' keyword."));

                    token = source.Next();
                    if (token.Type != TokenType.Integer)
                        throw new ParsingException(token.Position.ToString("Expected integer."));
                    else
                        command.Offset = uint.Parse(token.Text);

                    token = source.Next();
                    if (token.Text.ToLower() != Keywords.Sectors)
                        throw new ParsingException(token.Position.ToString("Expected the '" + Keywords.Sectors + "' keyword."));

                    token = source.Next();
                    if (token.Type != TokenType.Integer)
                        throw new ParsingException(token.Position.ToString("Expected integer."));
                    else
                        command.Sectors = uint.Parse(token.Text);

                    commands.Add(command);
                    break;
                default:
                    throw new ParsingException(obj.Position.ToString("Expected the '" + Keywords.Mbr + "' or '" + Keywords.Partition + "' keyword."));
            }
        }

        /// <summary>
        /// Parses a format command.
        /// </summary>
        private void Format()
        {
            Token<TokenType> format = source.Next();
            if (format.Text.ToLower() != Keywords.Format)
                throw new ParsingException(format.Position.ToString("Expected the '" + Keywords.Format + "' keyword."));

            Token<TokenType> system = source.Next();
            if (system.Text.ToLower() == Keywords.Fat32)
                commands.Add(new FormatFat32Command());
            else
                throw new ParsingException(system.Position.ToString("Expected the fat32 format."));
        }

        /// <summary>
        /// Parses a move command.
        /// </summary>
        private void Move()
        {
            Token<TokenType> move = source.Next();
            if (move.Text.ToLower() != Keywords.Move)
                throw new ParsingException(move.Position.ToString("Expected the '" + Keywords.Move + "' keyword."));

            Token<TokenType> s = source.Next();
            if (s.Type != TokenType.String)
                throw new ParsingException(s.Position.ToString("Expected a string."));

            Token<TokenType> to = source.Next();
            if (to.Text.ToLower() != Keywords.To)
                throw new ParsingException(move.Position.ToString("Expected the '" + Keywords.To + "' keyword."));

            Token<TokenType> destination = source.Next();
            if (destination.Type != TokenType.String)
                throw new ParsingException(s.Position.ToString("Expected a string."));

            commands.Add(new MoveCommand(s.Text, destination.Text));
        }
    }
}