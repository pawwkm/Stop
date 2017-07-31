using System;
using System.Collections.Generic;
using System.Linq;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Provides extension methods to the <see cref="Bit"/> enum.
    /// </summary>
    internal static class BitExtensions
    {
        private static readonly Dictionary<string, Bit> Table = new Dictionary<string, Bit>()
        {
            { "",  Bit.None },
            { "l", Bit.L },
            { "L", Bit.L },
            { "s", Bit.S },
            { "S", Bit.S },
            { "h", Bit.H },
            { "H", Bit.H },
            { "d", Bit.D },
            { "D", Bit.D },
            { "b", Bit.B },
            { "B", Bit.B },
        };

        /// <summary>
        /// Converts a bit to its assembly form.
        /// </summary>
        /// <param name="bit">The bit to convert.</param>
        /// <returns>The converted bit.</returns>
        public static string AsText(this Bit bit)
        {
            if (Table.ContainsValue(bit))
                return Table.First(x => x.Value == bit).Key;

            throw new NotSupportedException($"The value '{bit}' is unsupported.");
        }

        /// <summary>
        /// Converts a bit from its assembly form.
        /// </summary>
        /// <param name="bit">The bit to convert.</param>
        /// <returns>The converted bit.</returns>
        public static Bit ToBit(this string bit)
        {
            if (Table.ContainsKey(bit))
                return Table[bit];

            throw new NotSupportedException($"The value '{bit}' is unsupported.");
        }

        /// <summary>
        /// Tests if the string represents a <see cref="Bit"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the string represents a <see cref="Bit"/> value: otherwise false.</returns>
        public static bool IsBit(this string value)
        {
            return Table.ContainsKey(value);
        }

        /// <summary>
        /// Tests if the string represents a <see cref="Bit"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the string represents a <see cref="Bit"/> value: otherwise false.</returns>
        public static bool IsBit(this char value)
        {
            return Table.ContainsKey(value.ToString());
        }
    }
}