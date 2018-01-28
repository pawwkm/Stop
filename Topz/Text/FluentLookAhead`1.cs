using System;

namespace Topz.Text
{
    /// <summary>
    /// Fluent interface for looking ahead.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Token{T}.Type"/>.</typeparam>
    public class FluentLookAhead<T> where T : IComparable
    {
        private LexicalAnalyzer<T> source;

        private int current;

        private bool success;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentLookAhead{T}"/> class.
        /// </summary>
        /// <param name="analyzer">The analyzer to look ahead in.</param>
        /// <param name="lookAhead">The current look ahead.</param>
        /// <param name="result">The result of the first look ahead.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="analyzer"/> is null.
        /// </exception>
        internal FluentLookAhead(LexicalAnalyzer<T> analyzer, int lookAhead, bool result)
        {
            if (analyzer == null)
                throw new ArgumentNullException(nameof(analyzer));

            source = analyzer;
            current = lookAhead;
            success = result;
        }

        /// <summary>
        /// Returns the success of the last look ahead match.
        /// </summary>
        /// <param name="fla">The instance to operate on.</param>
        public static implicit operator bool(FluentLookAhead<T> fla)
        {
            return fla.success;
        }

        /// <summary>
        /// Checks if the next token's text matches the <paramref name="expected"/> text. 
        /// </summary>
        /// <param name="expected">The expected text of the next token.</param>
        /// <returns>This instance.</returns>
        public FluentLookAhead<T> Then(string expected)
        {
            if (success)
                success = source.LookAhead(current++).Text == expected;

            return this;
        }

        /// <summary>
        /// Checks if the next token's type matches the <paramref name="expected"/> text. 
        /// </summary>
        /// <param name="expected">The expected type of the next token.</param>
        /// <returns>This instance.</returns>
        public FluentLookAhead<T> Then(T expected)
        {
            if (success)
                success = source.LookAhead(current++).Type.Equals(expected);

            return this;
        }
    }
}