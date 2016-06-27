using Pote;
using System;
using System.Collections.Generic;
using System.IO;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// A disk script.
    /// </summary>
    public class Script
    {
        private List<Command> commands = new List<Command>();

        private Context context = new Context();

        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> with no commands.
        /// </summary>
        public Script()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        /// <param name="code">The code of the script.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="code"/> is null.
        /// </exception>
        public Script(Stream code)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            using (StreamReader reader = new StreamReader(code))
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(reader);
                Parser parser = new Parser();

                commands.AddRange(parser.Parse(analyzer));
            }
        }

        /// <summary>
        /// Runs the script.
        /// </summary>
        public void Run()
        {
            foreach (Command command in commands)
                command.Execute(context);
        }

        /// <summary>
        /// Runs the commands in the <paramref name="code"/> 
        /// and adds it to the script.
        /// </summary>
        /// <param name="code">
        /// The commands to run.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="code"/> is null.
        /// </exception>
        public void Run(string code)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            LexicalAnalyzer analyzer = new LexicalAnalyzer(code.ToStreamReader());
            Parser parser = new Parser();

            foreach (Command command in parser.Parse(analyzer))
            {
                command.Execute(context);
                commands.Add(command);
            }
        }
    }
}