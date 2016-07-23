using Pote;
using Pote.Text;
using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Defines a shifter operand.
    /// </summary>
    internal class ShifterOperand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShifterOperand"/> class. 
        /// </summary>
        /// <param name="position">The position where the operand was declared.</param>
        /// <param name="immediate">The immediate integer of the operand.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public ShifterOperand(InputPosition position, int immediate)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Position = position;
            Immediate = immediate;
            OperandType = ShifterOperandType.Immediate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShifterOperand"/> class.
        /// </summary>
        /// <param name="position">The position where the operand was declared.</param>
        /// <param name="immediate">The immediate integer of the operand.</param>
        /// <param name="shifter">The register shifter of the operand.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="shifter"/> is null.
        /// </exception>
        public ShifterOperand(InputPosition position, int immediate, string shifter) : this(position, immediate)
        {
            if (shifter == null)
                throw new ArgumentNullException(nameof(shifter));
            if (!shifter.ToLower().IsOneOf(Registers.Shifted))
                throw new ArgumentException("This is not a shifter.", nameof(shifter));

            Shifter = shifter;
            OperandType = ShifterOperandType.ShiftedRegister;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShifterOperand"/> class.
        /// </summary>
        /// <param name="position">The position where the operand was declared.</param>
        /// <param name="register">The register of the operand.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="register"/> is null.
        /// </exception>
        public ShifterOperand(InputPosition position, string register)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Position = position;
            Register = register;
            OperandType = ShifterOperandType.Register;
        }

        /// <summary>
        /// Defines the type of the shifter operand.
        /// </summary>
        public ShifterOperandType OperandType
        {
            get;
            private set;
        }

        /// <summary>
        /// The register shifter of the operand.
        /// </summary>
        public string Shifter
        {
            get;
            private set;
        }

        /// <summary>
        /// The register of the operand.
        /// </summary>
        public string Register
        {
            get;
            private set;
        }

        /// <summary>
        /// The immediate integer of the operand.
        /// </summary>
        public int Immediate
        {
            get;
            private set;
        }

        /// <summary>
        /// The position where the operand was declared.
        /// </summary>
        public InputPosition Position
        {
            get;
            private set;
        }
    }
}