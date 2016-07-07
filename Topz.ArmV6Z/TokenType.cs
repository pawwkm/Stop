namespace Topz.ArmV6Z
{
    /// <summary>
    /// The type of tokens in a script.
    /// </summary>
    internal enum TokenType
    {
        /// <summary>
        /// There is no more input,
        /// </summary>
        EndOfInput,

        /// <summary>
        /// The contents of the token could not be identified.
        /// </summary>
        Unknown,
    }
}