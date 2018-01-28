using System.Collections.Generic;

namespace Topz.Text
{
    /// <summary>
    /// Defines all the available new line characters.
    /// </summary>
    public static class Newline
    {
        /// <summary>
        /// The line feed character.
        /// </summary>
        public const char LineFeed = '\u000A';

        /// <summary>
        /// The vertical tab character.
        /// </summary>
        public const char VerticalTab = '\u000B';

        /// <summary>
        /// The form feed character.
        /// </summary>
        public const char FormFeed = '\u000C';

        /// <summary>
        /// The carriage return character.
        /// </summary>
        public const char CarriageReturn = '\u000D';

        /// <summary>
        /// The next line character.
        /// </summary>
        public const char NextLine = '\u0085';

        /// <summary>
        /// The line separator character.
        /// </summary>
        public const char LineSeparator = '\u2028';

        /// <summary>
        /// The paragraph separator character.
        /// </summary>
        public const char ParagraphSeparator = '\u2029';

        /// <summary>
        /// The available operators.
        /// </summary>
        public static IEnumerable<char> All
        {
            get
            {
                yield return LineFeed;
                yield return VerticalTab;
                yield return FormFeed;
                yield return CarriageReturn;
                yield return NextLine;
                yield return LineSeparator;
                yield return ParagraphSeparator;
            }
        }
    }
}