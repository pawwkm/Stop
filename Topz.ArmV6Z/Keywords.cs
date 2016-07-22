using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Keywords of the language.
    /// </summary>
    internal static class Keywords
    {
        /// <summary>
        /// The 'procedure' keyword.
        /// </summary>
        public const string Procedure = "procedure";

        /// <summary>
        /// The 'data' keyword.
        /// </summary>
        public const string Data = "data";

        /// <summary>
        /// The 'string' keyword.
        /// </summary>
        public const string String = "string";

        /// <summary>
        /// The 'external' keyword.
        /// </summary>
        public const string External = "external";

        /// <summary>
        /// All of the keywords.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                return new[] 
                {
                    Procedure,
                    Data,
                    String,
                    External
                };
            }
        }
    }
}