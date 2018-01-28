using System;

namespace Topz
{
    /// <summary>
    /// Provides extensions for the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Checks if the string only contains 0 and 1.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the string is binary; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static bool IsBinary(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            foreach (var c in value)
            {
                if (c != '0' && c != '1')
                    return false;
            }

            return true;
        }
    }
}