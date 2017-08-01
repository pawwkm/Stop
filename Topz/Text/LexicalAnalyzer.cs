using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Topz.Text
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = Justifications.CollectionSuffixNotApplicable)]
    public abstract class LexicalAnalyzer<T> : IEnumerable<Token<T>> where T : IComparable
    {
        private int index;

        private bool lastWasCarriageReturn = false;

        private string source;

        private Queue<Token<T>> buffer = new Queue<Token<T>>();

        /// <summary>
        /// Reads the next token.
        /// </summary>
        /// <returns>The next token.</returns>
        public Token<T> Next()
        {
            if (buffer.Count != 0)
                return buffer.Dequeue();

            return NextTokenFromSource();
        }

        /// <summary>
        /// Gets the next token from the source code  
        /// without consuming it.
        /// </summary>
        /// <returns>
        /// The next token from source code. If there is 
        /// no more tokens in the source code then a token 
        /// with the type default(<typeparamref name="T"/>)
        /// is returned.
        /// </returns>
        public Token<T> LookAhead()
        {
            return LookAhead(0);
        }

        /// <summary>
        /// Gets the next token from the source code  
        /// without consuming it.
        /// </summary>
        /// <param name="amount">
        /// The number of tokens look ahead.
        /// </param>
        /// <returns>
        /// The token at the specified location. If there is 
        /// no more tokens in the source code at the specified location 
        /// then a token with the type default(<typeparamref name="T"/>)
        /// is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="amount"/> is less than zero.
        /// </exception>
        public Token<T> LookAhead(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (!EndOfSourceCode && buffer.Count <= amount)
            {
                var tokensInBuffer = buffer.Count;
                for (var i = 0; i < amount + 1 - tokensInBuffer; i++)
                {
                    var token = NextTokenFromSource();
                    if (!EqualityComparer<T>.Default.Equals(token.Type, default(T)))
                        buffer.Enqueue(token);
                }
            }

            if (buffer.Count <= amount)
                return new Token<T>("", default(T), Position.DeepCopy());

            return buffer.ElementAt(amount);
        }

        /// <summary>
        /// Checks if the next token's text matches the <paramref name="expected"/> text. 
        /// </summary>
        /// <param name="expected">The expected text of the next token.</param>
        /// <returns>The result of the look ahead and a fluent interface for continuing looking ahead.</returns>
        public FluentLookAhead<T> NextIs(string expected)
        {
            return NextIs(expected, 0);
        }

        /// <summary>
        /// Checks if the next token's text matches the <paramref name="expected"/> text. 
        /// </summary>
        /// <param name="expected">The expected text of the next token.</param>
        /// <param name="start">Where to start looking ahead.</param>
        /// <returns>The result of the look ahead and a fluent interface for continuing looking ahead.</returns>
        public FluentLookAhead<T> NextIs(string expected, int start)
        {
            return new FluentLookAhead<T>(this, start + 1, LookAhead(start).Text == expected);
        }

        /// <summary>
        /// Checks if the next token's type matches the <paramref name="expected"/> text. 
        /// </summary>
        /// <param name="expected">The expected type of the next token.</param>
        /// <returns>The result of the look ahead and a fluent interface for continuing looking ahead.</returns>
        public FluentLookAhead<T> NextIs(T expected)
        {
            return NextIs(expected, 0);
        }

        /// <summary>
        /// Checks if the next token's type matches the <paramref name="expected"/> text. 
        /// </summary>
        /// <param name="expected">The expected type of the next token.</param>
        /// <param name="start">Where to start looking ahead.</param>
        /// <returns>The result of the look ahead and a fluent interface for continuing looking ahead.</returns>
        public FluentLookAhead<T> NextIs(T expected, int start)
        {
            return new FluentLookAhead<T>(this, start + 1, LookAhead(start).Type.Equals(expected));
        }

        /// <summary>
        /// Returns a list of the tokens in the source code.
        /// </summary>
        /// <returns>A list of the tokens in the source code.</returns>
        public IEnumerator<Token<T>> GetEnumerator()
        {
            while (!EndOfSourceCode)
                yield return Next();
        }

        /// <summary>
        /// Reads the next token from the source code.
        /// </summary>
        /// <returns>The next token from the source code.</returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EndOfSourceCode"/> is true.
        /// </exception>
        protected abstract Token<T> NextTokenFromSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalAnalyzer{T}"/> class.
        /// </summary>
        /// <param name="code">The source code to analyze.</param>
        /// <param name="path">The path to the <paramref name="code"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="code"/> or <paramref name="path"/> is null.
        /// </exception>
        protected LexicalAnalyzer(string code, string path)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            source = code;
            Position.File = path;
        }

        /// <summary>
        /// True if all the source code has been analyzed; otherwise false.
        /// </summary>
        public bool EndOfSourceCode
        {
            get
            {
                return buffer.Count == 0 && NoMoreCharacters;
            }
        }

        /// <summary>
        /// The position within the source code.
        /// </summary>
        public Position Position
        {
            get;
        } = new Position();

        /// <summary>
        /// The next character in the source code.
        /// </summary>
        protected char NextCharacter
        {
            get
            {
                return source[index];
            }
        }

        /// <summary>
        /// Indecates whether there is more characters to read or not.
        /// </summary>
        protected bool NoMoreCharacters
        {
            get
            {
                return index == source.Length;
            }
        }

        /// <summary>
        /// Tests if a string in the given list is the next lexeme in the source code.
        /// </summary>
        /// <param name="lexemes">The lexemes to test against the source code.</param>
        /// <returns>True if one of the <paramref name="lexemes"/> is next in the source code; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="lexemes"/> is null.
        /// </exception>
        protected bool Match(IEnumerable<string> lexemes)
        {
            if (lexemes == null)
                throw new ArgumentNullException(nameof(lexemes));

            foreach (var lexeme in lexemes)
            {
                if (Match(lexeme))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Tests if a string is the next lexeme in the source code.
        /// </summary>
        /// <param name="lexeme">The lexeme to test against the source code.</param>
        /// <returns>True if the <paramref name="lexeme"/> is next in the source code; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="lexeme"/> is null.
        /// </exception>
        protected bool Match(string lexeme)
        {
            if (lexeme == null)
                throw new ArgumentNullException(nameof(lexeme));

            if (index + lexeme.Length > source.Length)
                return false;

            for (var i = 0; i < lexeme.Length; i++)
            {
                if (source[index + i] != lexeme[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Advances the <see cref="Position"/> according 
        /// to an <paramref name="amount"/> of characters.
        /// </summary>
        /// <param name="amount">The amount of characters to advance.</param>
        /// <returns>The read characters.</returns>
        protected string Advance(int amount)
        {
            var lexeme = "";
            for (var i = 0; i < amount; i++)
                lexeme += Advance();

            return lexeme;
        }

        /// <summary>
        /// Advances the <see cref="Position"/> according 
        /// to <see cref="NextCharacter"/>.
        /// </summary>
        /// <returns>The value of <see cref="NextCharacter"/>.</returns>
        protected char Advance()
        {
            var character = NextCharacter;
            index++;

            if (lastWasCarriageReturn)
            {
                lastWasCarriageReturn = false;
                if (character == Newline.LineFeed)
                {
                }
                else if (character == Newline.CarriageReturn)
                {
                    lastWasCarriageReturn = true;
                    Position.Column = 1;
                    Position.Line++;
                }
                else if (Newline.All.Contains(character))
                {
                    Position.Column = 1;
                    Position.Line++;
                }
                else
                    Position.Column++;
            }
            else if (character == Newline.CarriageReturn)
            {
                lastWasCarriageReturn = true;
                Position.Column = 1;
                Position.Line++;
            }
            else if (Newline.All.Contains(character))
            {
                Position.Column = 1;
                Position.Line++;
            }
            else
                Position.Column++;

            return character;
        }

        /// <summary>
        /// Consumes the given text if it is the next set of characters 
        /// in the input.
        /// </summary>
        /// <param name="text">The text to consume.</param>
        /// <returns>True if the text was consumed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> is null.
        /// </exception>
        protected bool Consume(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (!Match(text))
                return false;

            Advance(text.Length);

            return true;
        }

        /// <summary>
        /// Returns a list of the tokens in the source code.
        /// </summary>
        /// <returns>A list of the tokens in the source code.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}