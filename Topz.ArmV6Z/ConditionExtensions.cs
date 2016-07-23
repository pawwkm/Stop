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
                    return "eq";
                case Condition.NotEqual:
                    return "ne";
                case Condition.CarrySet:
                    return "cs";
                case Condition.CarryClear:
                    return "cc";
                case Condition.Minus:
                    return "mi";
                case Condition.Plus:
                    return "pl";
                case Condition.Overflow:
                    return "vs";
                case Condition.NoOverflow:
                    return "vc";
                case Condition.UnsignedHigher:
                    return "hi";
                case Condition.UnsignedLowerOrSame:
                    return "ls";
                case Condition.SignedGreaterThanOrEqual:
                    return "ge";
                case Condition.SignedLessThan:
                    return "lt";
                case Condition.SignedGreaterThan:
                    return "gt";
                case Condition.LessThanOrEqual:
                    return "le";
                default:
                    return "";
            }
        }
    }
}