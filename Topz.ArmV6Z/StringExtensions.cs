using System;
using System.Collections.Generic;
using System.Linq;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Provides extensions to the <see cref="string"/> class.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Finds the string that has the longest match with another string.
        /// </summary>
        /// <param name="source">The source to match against.</param>
        /// <param name="values">The possible matches.</param>
        /// <returns>The string with the longest match</returns>
        public static string LongestMatch(this string source, IEnumerable<string> values)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return (from value in values
                    where source.StartsWith(value) && source.Length >= value.Length
                    orderby value.Length
                    select value).ToArray().FirstOrDefault();
        }
    }
}