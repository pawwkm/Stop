namespace Topz.ArmV6Z
{
    /// <summary>
    /// A condition for a given instruction.
    /// These values are masks for the 31:28 bits of an 
    /// instruction.
    /// </summary>
    internal enum Condition : uint
    {
        /// <summary>
        /// Execute the instruction, if Z is set.
        /// The mnemonic extension is 'EQ'.
        /// </summary>
        Equal = 0,

        /// <summary>
        /// Execute the instruction, if Z is clear.
        /// The mnemonic extension is 'NE'.
        /// </summary>
        NotEqual = 268435456,

        /// <summary>
        /// Execute the instruction, if C is set.
        /// The mnemonic extension is 'CS/HS'.
        /// </summary>
        CarrySet = 536870912,

        /// <summary>
        /// Execute the instruction, if C is clear.
        /// The mnemonic extension is 'CC/LO'.
        /// </summary>
        CarryClear = 805306368,

        /// <summary>
        /// Execute the instruction, if N is set.
        /// The mnemonic extension is 'MI'.
        /// </summary>
        Minus = 1073741824,

        /// <summary>
        /// Execute the instruction, if N is clear.
        /// The mnemonic extension is 'PL'.
        /// </summary>
        Plus = 1342177280,

        /// <summary>
        /// Execute the instruction, if V is set.
        /// The mnemonic extension is 'VS'.
        /// </summary>
        Overflow = 1610612736,

        /// <summary>
        /// Execute the instruction, if V is clear.
        /// The mnemonic extension is 'VC'.
        /// </summary>
        NoOverflow = 1879048192,

        /// <summary>
        /// Execute the instruction, if C is set and Z is clear.
        /// The mnemonic extension is 'HI'.
        /// </summary>
        UnsignedHigher = 2147483648,

        /// <summary>
        /// Execute the instruction, if C is clear and Z is set.
        /// The mnemonic extension is 'LS'.
        /// </summary>
        UnsignedLowerOrSame = 2415919104,

        /// <summary>
        /// Execute the instruction, if N == V.
        /// The mnemonic extension is 'GE'.
        /// </summary>
        SignedGreaterThanOrEqual = 2684354560,

        /// <summary>
        /// Execute the instruction, if N != V.
        /// The mnemonic extension is 'LT'.
        /// </summary>
        SignedLessThan = 2952790016,

        /// <summary>
        /// Execute the instruction, if Z == 0 &amp;&amp; N == V.
        /// The mnemonic extension is 'GT'.
        /// </summary>
        SignedGreaterThan = 3221225472,

        /// <summary>
        /// Execute the instruction, if Z == 1 || N != V.
        /// The mnemonic extension is 'LE'.
        /// </summary>
        LessThanOrEqual = 3489660928,

        /// <summary>
        /// Always execute the instruction.
        /// The mnemonic extension is 'AL'.
        /// </summary>
        Always = 3758096384
    }
}