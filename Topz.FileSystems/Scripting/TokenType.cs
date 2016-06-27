namespace Topz.FileSystems.Scripting
{
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

        /// <summary>
        /// The token is an integer.
        /// </summary>
        Integer,

        /// <summary>
        /// The token is a keyword.
        /// </summary>
        Keyword,

        /// <summary>
        /// The token is a string.
        /// </summary>
        String
    }
}