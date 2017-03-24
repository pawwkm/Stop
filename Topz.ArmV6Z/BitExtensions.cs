using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Provides extension methods to the <see cref="Bit"/> enum.
    /// </summary>
    internal static class BitExtensions
    {
        /// <summary>
        /// Converts a bit to its assembly form.
        /// </summary>
        /// <param name="bit">The bit to convert.</param>
        /// <returns>The converted bit.</returns>
        public static string AsText(this Bit bit)
        {
            switch (bit)
            {
                case Bit.L:
                    return "l";
                case Bit.None:
                    return "";
                case Bit.S:
                    return "s";
                default:
                    throw new NotSupportedException($"The value '{bit}' is unsupported.");
            }
        }

        public static Bit ToBit(this string bit)
        {
            switch (bit)
            {
                case "l":
                    return Bit.L;
                case "":
                    return Bit.None;
                case "s":
                    return Bit.S;
                default:
                    throw new NotSupportedException($"The value '{bit}' is unsupported.");
            }
        }
    }
}