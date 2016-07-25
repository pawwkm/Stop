using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Performs a bitwise Exclusive-OR of two values.
    /// </summary>
    /// <remarks>See A.4.1.18 for more info.</remarks>
    internal class ExclusiveOrInstruction : Format1Instruction
    {
        /// <summary>
        /// Intializes a new instance of the <see cref="ExclusiveOrInstruction"/> class.
        /// </summary>
        /// <param name="label">The label of the instruction, if any.</param>
        /// <param name="mnemonic">The mnemonic of the instruction.</param>
        /// <param name="r1">The first register.</param>
        /// <param name="r2">The second register.</param>
        /// <param name="shifter">The third register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mnemonic"/>, <paramref name="r1"/>, <paramref name="r2"/> or <paramref name="shifter"/> is null.
        /// </exception>
        public ExclusiveOrInstruction(Label label, Mnemonic mnemonic, RegisterOperand r1, RegisterOperand r2, ShifterOperand shifter) : base(label, mnemonic, r1, r2, shifter)
        {
        }
    }
}