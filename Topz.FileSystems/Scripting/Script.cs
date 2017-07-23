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
        /// Initializes a new instance of the <see cref="Script"/> class with no commands.
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

            using (var reader = new StreamReader(code))
            {
                var analyzer = new LexicalAnalyzer(reader);
                var parser = new Parser();

                commands.AddRange(parser.Parse(analyzer));
            }
        }

        /// <summary>
        /// Runs the script.
        /// </summary>
        public void Run()
        {
            foreach (var command in commands)
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

            var analyzer = new LexicalAnalyzer(code.ToStreamReader());
            var parser = new Parser();

            foreach (var command in parser.Parse(analyzer))
            {
                command.Execute(context);
                commands.Add(command);
            }
        }
    }
}