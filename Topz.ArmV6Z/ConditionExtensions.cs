namespace Topz.ArmV6Z
{
    /// <summary>
    /// Provides extensions for the <see cref="Condition"/> enum.
    /// </summary>
    internal static class ConditionExtensions
    {
        /// <summary>
        /// Converts a condition to its assembly form.
        /// </summary>
        /// <param name="condition">The condition to convert.</param>
        /// <returns>The converted condition.</returns>
        public static string AsText(this Condition condition)
        {
            switch (condition)
            {
                case Condition.Equal:
                    return Mnemonic.EqualExtension;
                case Condition.NotEqual:
                    return Mnemonic.NotEqualExtension;
                case Condition.CarrySet:
                    return Mnemonic.CarrySetExtension;
                case Condition.CarryClear:
                    return Mnemonic.CarryClearExtension;
                case Condition.Minus:
                    return Mnemonic.MinusExtension;
                case Condition.Plus:
                    return Mnemonic.PlusExtension;
                case Condition.Overflow:
                    return Mnemonic.OverflowExtension;
                case Condition.NoOverflow:
                    return Mnemonic.NoOverflowExtension;
                case Condition.UnsignedHigher:
                    return Mnemonic.UnsignedHigherExtension;
                case Condition.UnsignedLowerOrSame:
                    return Mnemonic.UnsignedLowerOrSameExtension;
                case Condition.SignedGreaterThanOrEqual:
                    return Mnemonic.SignedGreaterThanOrEqualExtension;
                case Condition.SignedLessThan:
                    return Mnemonic.SignedLessThanExtension;
                case Condition.SignedGreaterThan:
                    return Mnemonic.SignedGreaterThanExtension;
                case Condition.LessThanOrEqual:
                    return Mnemonic.LessThanOrEqualExtension;
                default:
                    return "";
            }
        }
    }
}