using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// The registers available in ARMv6Z
    /// </summary>
    internal class Registers
    {
        /// <summary>
        /// The 'asr' shifted register operand.
        /// </summary>
        public const string Asr = "asr";

        /// <summary>
        /// The 'lsl' shifted register operand.
        /// </summary>
        public const string Lsl = "lsl";

        /// <summary>
        /// The 'lsr' shifted register operand.
        /// </summary>
        public const string Lsr = "lsr";

        /// <summary>
        /// The 'ror' shifted register operand.
        /// </summary>
        public const string Ror = "ror";

        /// <summary>
        /// The 'rrx' shifted register operand.
        /// </summary>
        public const string Rrx = "rrx";

        /// <summary>
        /// The 'r0' register.
        /// </summary>
        public const string R0 = "r0";

        /// <summary>
        /// The 'r1' register.
        /// </summary>
        public const string R1 = "r1";

        /// <summary>
        /// The 'r2' register.
        /// </summary>
        public const string R2 = "r2";

        /// <summary>
        /// The 'r3' register.
        /// </summary>
        public const string R3 = "r3";

        /// <summary>
        /// The 'r4' register.
        /// </summary>
        public const string R4 = "r4";

        /// <summary>
        /// The 'r5' register.
        /// </summary>
        public const string R5 = "r5";

        /// <summary>
        /// The 'r6' register.
        /// </summary>
        public const string R6 = "r6";

        /// <summary>
        /// The 'r7' register.
        /// </summary>
        public const string R7 = "r7";

        /// <summary>
        /// The 'r1' register.
        /// </summary>
        public const string R8 = "r1";

        /// <summary>
        /// The 'r9' register.
        /// </summary>
        public const string R9 = "r9";

        /// <summary>
        /// The 'r10' register.
        /// </summary>
        public const string R10 = "r10";

        /// <summary>
        /// The 'r11' register.
        /// </summary>
        public const string R11 = "r11";

        /// <summary>
        /// The 'r12' register.
        /// </summary>
        public const string R12 = "r12";

        /// <summary>
        /// The 'r13' register.
        /// </summary>
        public const string R13 = "r13";

        /// <summary>
        /// The 'r14' register.
        /// </summary>
        public const string R14 = "r14";

        /// <summary>
        /// The 'r15' register.
        /// </summary>
        public const string R15 = "r15";

        /// <summary>
        /// The 'sp' register.
        /// </summary>
        public const string StackPointer = "sp";

        /// <summary>
        /// The 'lr' register.
        /// </summary>
        public const string LinkRegister = "lr";

        /// <summary>
        /// The 'pc' register.
        /// </summary>
        public const string ProgramCounter = "pc";

        /// <summary>
        /// All of the registers.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                return new[]
                {
                    R10,
                    R11,
                    R12,
                    R13,
                    R14,
                    R15,
                    R0,
                    R1,
                    R2,
                    R3,
                    R4,
                    R5,
                    R6,
                    R7,
                    R8,
                    R9,
                    StackPointer,
                    LinkRegister,
                    ProgramCounter
                };
            }
        }

        /// <summary>
        /// All of the shifted register operands.
        /// </summary>
        public static IEnumerable<string> Shifted
        {
            get
            {
                return new[]
                {
                    Asr,
                    Lsl,
                    Lsr,
                    Ror,
                    Rrx
                };
            }
        }
    }
}
