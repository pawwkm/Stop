using System;

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

        public static Condition ToCondition(this string condition)
        {
            switch (condition)
            {
                case "eq":
                    return Condition.Equal;
                case "ne":
                    return Condition.NotEqual;
                case "cs":
                    return Condition.CarrySet;
                case "cc":
                    return Condition.CarryClear;
                case "mi":
                    return Condition.Minus;
                case "pl":
                    return Condition.Plus;
                case "vs":
                    return Condition.Overflow;
                case "vc":
                    return Condition.NoOverflow;
                case "hi":
                    return Condition.UnsignedHigher;
                case "ls":
                    return Condition.UnsignedLowerOrSame;
                case "ge":
                    return Condition.SignedGreaterThanOrEqual;
                case "lt":
                    return Condition.SignedLessThan;
                case "gt":
                    return Condition.SignedGreaterThan;
                case "le":
                    return Condition.LessThanOrEqual;
                default:
                    throw new NotSupportedException($"The condition '{condition}' is not supported.");
            }
        }
    }
}