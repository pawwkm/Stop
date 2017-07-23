using System;
using Topz.FileFormats.Atom;
using Pote;

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
        /// <param name="source">The program to assemble.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <returns>The assembled program.</returns>
        public ObjectFile Assemble(string source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using (var reader = source.ToStreamReader())
            {
                var analyzer = new LexicalAnalyzer(reader, source);
                var parser = new Parser();

                var encoder = new EncodingPass();
                encoder.Visit(parser.Parse(analyzer));

                return encoder.Code;
            }
        }
    }
}