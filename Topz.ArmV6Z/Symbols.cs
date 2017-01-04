using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Defines all the symbols available.
    /// </summary>
    internal static class Symbols
    {
        /// <summary>
        /// The end of label symbol.
        /// </summary>
        public const string EndOfLabel = ":";

        /// <summary>
        /// The comma symbol.
        /// </summary>
        public const string Comma = ",";

        /// <summary>
        /// The left square bracket symbol.
        /// </summary>
        public const string LeftSquareBracket = "[";

        /// <summary>
        /// The right square bracket symbol.
        /// </summary>
        public const string RightSquareBracket = "]";

        /// <summary>
        /// The plus symbol.
        /// </summary>
        public const string Plus = "+";

        /// <summary>
        /// The minus symbol.
        /// </summary>
        public const string Minus = "-";

        /// <summary>
        /// All of the symbols.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                yield return EndOfLabel;
                yield return Comma;
                yield return LeftSquareBracket;
                yield return RightSquareBracket;
                yield return Plus;
                yield return Minus;
            }
        }
    }
}