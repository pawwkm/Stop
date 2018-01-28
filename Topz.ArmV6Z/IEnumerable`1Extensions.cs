using System.Collections.Generic;
using System.Linq;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Provides extensions to the <see cref="IEnumerable{T}"/> class.
    /// </summary>
    public static class IEnumerable1Extensions
    {
        /// <summary>
        /// Converts a list of bytes to 32 bit unsigned integers as hex.
        /// </summary>
        /// <param name="bytes">The bytes to convert.</param>
        /// <returns>The converted bytes.</returns>
        public static string ToHex(this IEnumerable<byte> bytes)
        {
            var instructions = bytes.Select((x, i) => new { Index = i, Value = x })
                                    .GroupBy(x => x.Index / 4)
                                    .Select(x => x.Select(v => v.Value));

            return string.Join(" ", from instruction in instructions
                                    select string.Join("", from b in instruction
                                                           select b.ToString("X2")));
        }
    }
}