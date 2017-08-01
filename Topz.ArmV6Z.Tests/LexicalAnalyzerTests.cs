using NUnit.Framework;
using Pote;
using System.Text;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Provides tests for the <see cref="LexicalAnalyzer"/> class.
    /// </summary>
    [TestFixture]
    public class LexicalAnalyzerTests
    {
        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> can skip 
        /// a single line comment.
        /// </summary>
        [Test]
        public void NextTokenFromSource_SingleLineComment_SkipsComment()
        {
            var code = "; This is a comment";

            var analyzer = new LexicalAnalyzer(code);
            var token = analyzer.Next();

            Assert.AreEqual("", token.Text);
            Assert.AreEqual(TokenType.EndOfInput, token.Type);
            Assert.AreEqual(20, token.Position.Column);
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> can skip 
        /// a multi line comment.
        /// </summary>
        [Test]
        public void NextTokenFromSource_MultiLineComment_SkipsComment()
        {
            var code = new StringBuilder();
            code.AppendLine("/*");
            code.AppendLine("This is a multi line comment.");
            code.AppendLine("*/");

            var analyzer = new LexicalAnalyzer(code.ToString());
            var token = analyzer.Next();

            Assert.AreEqual("", token.Text);
            Assert.AreEqual(TokenType.EndOfInput, token.Type);
            Assert.AreEqual(1, token.Position.Column);
            Assert.AreEqual(4, token.Position.Line);
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> recognizes
        /// all keywords in <see cref="Keywords.All"/> as <see cref="TokenType.Keyword"/>.
        /// </summary>
        [Test]
        public void NextTokenFromSource_Keywords_KeywordsRecognized()
        {
            foreach (var keyword in Keywords.All)
            {
                var analyzer = new LexicalAnalyzer(keyword);
                var token = analyzer.Next();

                Assert.AreEqual(keyword, token.Text);
                Assert.AreEqual(TokenType.Keyword, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> recognizes
        /// all registers in <see cref="Register.All"/> as <see cref="TokenType.Register"/>.
        /// </summary>
        [Test]
        public void NextTokenFromSource_Registers_RegistersRecognized()
        {
            foreach (var register in Register.All)
            {
                var analyzer = new LexicalAnalyzer(register);
                var token = analyzer.Next();

                Assert.AreEqual(register, token.Text);
                Assert.AreEqual(TokenType.Register, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> recognizes
        /// all registers in <see cref="Symbols.All"/> as <see cref="TokenType.Symbol"/>.
        /// </summary>
        [Test]
        public void NextTokenFromSource_Symbols_SymbolsRecognized()
        {
            foreach (var symbol in Symbols.All)
            {
                var analyzer = new LexicalAnalyzer(symbol);
                var token = analyzer.Next();

                Assert.AreEqual(symbol, token.Text);
                Assert.AreEqual(TokenType.Symbol, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> recognizes
        /// all mnemonics in <see cref="Mnemonic.All"/> as 
        /// <see cref="TokenType.Mnemonic"/>.
        /// </summary>
        [Test]
        public void NextTokenFromSource_Mnemonics_MnemonicsRecognized()
        {
            foreach (var mnemonic in Mnemonic.All)
            {
                var analyzer = new LexicalAnalyzer(mnemonic);
                var token = analyzer.Next();

                Assert.AreEqual(mnemonic, token.Text);
                Assert.AreEqual(TokenType.Mnemonic, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> recognizes
        /// all register shifters in <see cref="RegisterShifter.All"/> as 
        /// <see cref="TokenType.RegisterShifter"/>.
        /// </summary>
        [Test]
        public void NextTokenFromSource_RegisterShifters_RegisterShiftersRecognized()
        {
            foreach (var shifter in RegisterShifter.All)
            {
                var analyzer = new LexicalAnalyzer(shifter);
                var token = analyzer.Next();

                Assert.AreEqual(shifter, token.Text);
                Assert.AreEqual(TokenType.RegisterShifter, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> recognizes
        /// identifiers as <see cref="TokenType.Identifier"/>.
        /// </summary>
        /// <param name="identifier">The valid identifier to test.</param>
        [TestCase("A")]
        [TestCase("a")]
        [TestCase("Ab")]
        [TestCase("ab")]
        [TestCase("A1")]
        [TestCase("a1")]
        [TestCase("_A")]
        [TestCase("_a")]
        [TestCase("_1")]
        public void NextTokenFromSource_Identifier_IdentifiersRecognized(string identifier)
        {
            var analyzer = new LexicalAnalyzer(identifier);
            var token = analyzer.Next();

            Assert.AreEqual(identifier, token.Text);
            Assert.AreEqual(TokenType.Identifier, token.Type);
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> recognizes
        /// integers as <see cref="TokenType.Integer"/>.
        /// </summary>
        /// <param name="value">The value to test.</param>
        [TestCase(0u)]
        [TestCase(uint.MaxValue / 2)]
        [TestCase(uint.MaxValue)]
        public void NextTokenFromSource_Integers_IntegersRecognized(uint value)
        {
            var analyzer = new LexicalAnalyzer($"#{value}");
            var token = analyzer.Next();

            Assert.AreEqual(value.ToString(), token.Text);
            Assert.AreEqual(TokenType.Integer, token.Type);
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> recognizes
        /// strings as <see cref="TokenType.String"/>.
        /// </summary>
        [Test]
        public void NextTokenFromSource_Strings_StringsRecognized()
        {
            var valid = new[] { "", "Abc" };
            foreach (var str in valid)
            {
                var qouted = '"' + str + '"';

                var analyzer = new LexicalAnalyzer(qouted);
                var token = analyzer.Next();

                Assert.AreEqual(str, token.Text);
                Assert.AreEqual(TokenType.String, token.Type);
            }
        }
    }
}