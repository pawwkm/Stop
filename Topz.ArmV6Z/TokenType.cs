namespace Topz.ArmV6Z
{
    /// <summary>
    /// The type of tokens in a program.
    /// </summary>
    internal enum TokenType
    {
        /// <summary>
        /// There is no more input,
        /// </summary>
        EndOfInput,

        /// <summary>
        /// The token is a keyword.
        /// </summary>
        Keyword,

        /// <summary>
        /// The token is an identifier.
        /// </summary>
        Identifier,

        /// <summary>
        /// The token is an mnemonic.
        /// </summary>
        Mnemonic,

        /// <summary>
        /// The token is a register.
        /// </summary>
        Register,

        /// <summary>
        /// The token is a register shifter.
        /// </summary>
        RegisterShifter,

        /// <summary>
        /// The token is an integer.
        /// </summary>
        Integer,

        /// <summary>
        /// The token is a string.
        /// </summary>
        String,

        /// <summary>
        /// The token is a symbol.
        /// </summary>
        Symbol,

        /// <summary>
        /// The token is one of the <see cref="Condition"/> values.
        /// </summary>
        Condition,

        /// <summary>
        /// The token is a coprocessor.
        /// </summary>
        Coprocessor,

        /// <summary>
        /// The contents of the token could not be identified.
        /// </summary>
        Unknown,
    }
}