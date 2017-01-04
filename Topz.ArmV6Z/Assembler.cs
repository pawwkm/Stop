using Pote.Text;
using System;
using System.Collections.Generic;
using System.IO;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// An ArmV6Z assembler.
    /// </summary>
    public sealed class Assembler
    {
        /// <summary>
        /// Assembles a program.
        /// </summary>
        /// <param name="source">The path to the source file.</param>
        /// <param name="destination">The path to the file to store the result.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="destination"/> is null.
        /// </exception>
        /// <exception cref="ParsingException">
        /// A problem occurred when parsing.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// <paramref name="source"/> could not be found.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// <paramref name="source"/> is invalid, such as being on an unmapped drive.
        /// </exception>
        public void Assemble(string source, string destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            using (var reader = new StreamReader(source))
            {
                var analyzer = new LexicalAnalyzer(reader, source);
                var parser = new Parser();

                var program = parser.Parse(analyzer);
                foreach (var pass in GetPasses())
                    pass.Visit(program);
            }
        }

        /// <summary>
        /// Gets the passes that fits the users selected options.
        /// </summary>
        /// <returns>The passes that fits the users selected options.</returns>
        private IEnumerable<IPass> GetPasses()
        {
            yield break;
        }
    }
}