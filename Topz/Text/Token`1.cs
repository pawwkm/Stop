using System;
using System.Diagnostics;

namespace Topz.Text
{
    /// <summary>
    /// Represents a token from some input.
    /// </summary>
    /// <typeparam name="T">The possible types of a token.</typeparam>
    [DebuggerDisplay("{Type} = {Text}")]
    public sealed class Token<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token{T}"/> class
        /// with data about the token itself and its position in the input.
        /// </summary>
        /// <param name="character">The character value of the token.</param>
        /// <param name="type">The type of token.</param>
        /// <param name="position">The position where the token starts in the source.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> or <paramref name="position"/> is null.
        /// </exception>
        public Token(char character, T type, Position position)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            else if (position == null)
                throw new ArgumentNullException(nameof(position));

            Text = character.ToString();
            Type = type;
            Position = position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token{T}"/> class
        /// with data about the token itself and its position in the input.
        /// </summary>
        /// <param name="text">The text value of the token.</param>
        /// <param name="type">The type of token.</param>
        /// <param name="position">The position where the token starts in the source.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/>, <paramref name="type"/> or <paramref name="position"/> is null.
        /// </exception>
        public Token(string text, T type, Position position)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            else if (type == null)
                throw new ArgumentNullException(nameof(type));
            else if (position == null)
                throw new ArgumentNullException(nameof(position));

            Text = text;
            Type = type;
            Position = position;
        }

        /// <summary>
        /// The value of the token.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// The type of token.
        /// </summary>
        public T Type
        {
            get;
            private set;
        }

        /// <summary>
        /// The position where the token starts in the input.
        /// </summary>
        public Position Position
        {
            get;
            private set;
        }
    }
}