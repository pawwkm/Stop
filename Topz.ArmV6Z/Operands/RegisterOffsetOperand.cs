using System;

namespace Topz.ArmV6Z.Operands
{
    /// <summary>
    /// Load and Store Word or unsigned byte - Register offset.
    /// </summary>
    /// <remarks>See section A5.2.3</remarks>
    internal sealed class RegisterOffsetOperand : AddressingMode2
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterOffsetOperand"/> class.
        /// </summary>
        /// <param name="baseAddress">Specifies the register containing the base address.</param>
        /// <param name="addToBase">
        /// If true then <paramref name="offset"/> is added to
        /// <paramref name="baseAddress"/>; otherwise it is 
        /// subtracted from it.
        /// </param>
        /// <param name="offset">
        /// The offset to add or subtract from the
        /// <paramref name="baseAddress"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="baseAddress"/> or <paramref name="offset"/> is null.
        /// </exception>
        public RegisterOffsetOperand(RegisterOperand baseAddress, bool addToBase, RegisterOperand offset) : base(baseAddress)
        {
            if (offset == null)
                throw new ArgumentNullException(nameof(offset));

            AddToBase = addToBase;
            Offset = offset;
        }

        /// <summary>
        /// If true then <see cref="Offset"/> is added to
        /// <see cref="AddressingMode2.Rn"/>;
        /// otherwise it is subtracted from it.
        /// </summary>
        public bool AddToBase
        {
            get;
            private set;
        }

        /// <summary>
        /// The offset to add or subtract from the
        /// <see cref="AddressingMode2.Rn"/>.
        /// </summary>
        public RegisterOperand Offset
        {
            get;
            private set;
        }
    }
}