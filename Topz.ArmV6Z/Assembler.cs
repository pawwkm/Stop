using System;
using System.Diagnostics.CodeAnalysis;
using Topz.FileFormats.Atom;
using Topz.Text;

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
        /// <param name="code">The program to assemble.</param>
        /// <param name="path">The path to the <paramref name="code"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="code"/> or <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="ParsingException">
        /// A problem occurred when parsing.
        /// </exception>
        /// <exception cref="EncodingException">
        /// A problem occurred when encoding.
        /// </exception>
        /// <returns>The assembled program.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = Justifications.InstanceAccessMayBeNeededLater)]
        public ObjectFile Assemble(string code, string path)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var analyzer = new LexicalAnalyzer(code, path);
            var parser = new Parser();

            var encoder = new EncodingPass();
            encoder.Visit(parser.Parse(analyzer));

            return encoder.Code;
        }

        /// <summary>
        /// Assembles a program.
        /// </summary>
        /// <param name="code">The program to assemble.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="code"/> is null.
        /// </exception>
        /// <exception cref="ParsingException">
        /// A problem occurred when parsing.
        /// </exception>
        /// <exception cref="EncodingException">
        /// A problem occurred when encoding.
        /// </exception>
        /// <returns>The assembled program.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = Justifications.InstanceAccessMayBeNeededLater)]
        public ObjectFile Assemble(string code)
        {
            return Assemble(code, "");
        }
    }
}