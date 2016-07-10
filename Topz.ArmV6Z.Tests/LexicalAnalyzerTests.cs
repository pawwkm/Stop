using NUnit.Framework;
using Pote;
using Pote.Text;
using System.Text;

namespace Topz.ArmV6Z.Tests
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
            string code = "; This is a comment";

            LexicalAnalyzer analyzer = new LexicalAnalyzer(code.ToStreamReader());
            Token<TokenType> token = analyzer.Next();

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
            StringBuilder code = new StringBuilder();
            code.AppendLine("/*");
            code.AppendLine("This is a multi line comment.");
            code.AppendLine("*/");

            LexicalAnalyzer analyzer = new LexicalAnalyzer(code.ToStreamReader());
            Token<TokenType> token = analyzer.Next();

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
            foreach (string keyword in Keywords.All)
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(keyword.ToStreamReader());
                Token<TokenType> token = analyzer.Next();

                Assert.AreEqual(keyword, token.Text);
                Assert.AreEqual(TokenType.Keyword, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> recognizes
        /// all mnemonics in <see cref="Mnemonic.AllWithAndWithoutExtensions"/> as 
        /// <see cref="TokenType.Mnemonic"/>.
        /// </summary>
        [Test]
        public void NextTokenFromSource_Mnemonics_MnemonicsRecognized()
        {
            foreach (string keyword in Keywords.All)
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(keyword.ToStreamReader());
                Token<TokenType> token = analyzer.Next();

                Assert.AreEqual(keyword, token.Text);
                Assert.AreEqual(TokenType.Keyword, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer.NextTokenFromSource()"/> recognizes
        /// all keywords in <see cref="Keywords.All"/> as <see cref="TokenType.Keyword"/>.
        /// </summary>
        [Test]
        public void NextTokenFromSource_Identifiers_IdentifiersRecognized()
        {
            string[] valid = 
            {
                "A", "a", "Ab", "ab", "A1", "a1",
                "_A", "_a", "_1"
            };
            
            foreach (string identifier in valid)
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(identifier.ToStreamReader());
                Token<TokenType> token = analyzer.Next();

                Assert.AreEqual(identifier, token.Text);
                Assert.AreEqual(TokenType.Identifier, token.Type);
            }
        }
    }
}