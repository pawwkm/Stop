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
        /// All of the symbols.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                return new[]
                {
                    StartOfBlock,
                    EndOfBlock
                };
            }
        }
    }
}