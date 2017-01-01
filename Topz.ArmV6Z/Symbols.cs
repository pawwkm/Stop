using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Defines all the symbols available.
    /// </summary>
    internal static class Symbols
    {
        /// <summary>
        /// The start of block symbol.
        /// </summary>
        public const string StartOfBlock = "{";

        /// <summary>
        /// The end of block symbol.
        /// </summary>
        public const string EndOfBlock = "}";

        /// <summary>
        /// The end of lable symbol.
        /// </summary>
        public const string EndOfLable = ":";

        /// <summary>
        /// The list item separator symbol.
        /// </summary>
        public const string ListItemSeparator = ",";

        /// <summary>
        /// The left square bracket symbol.
        /// </summary>
        public const string LeftSquareBracket = "[";

        /// <summary>
        /// The right square bracket symbol.
        /// </summary>
        public const string RightSquareBracket = "]";

        /// <summary>
        /// All of the symbols.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                return new[]
                {
                    StartOfBlock,
                    EndOfBlock,
                    EndOfLable,
                    ListItemSeparator,
                    LeftSquareBracket,
                    RightSquareBracket
                };
            }
        }
    }
}