﻿namespace Topz.ArmV6Z
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
        /// The token is a keyword.
        /// </summary>
        Keyword,

        /// <summary>
        /// The token is an indentifier.
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
        /// The token is an integer.
        /// </summary>
        Integer,

        /// <summary>
        /// The token is a string.
        /// </summary>
        String,

        /// <summary>
        /// The contents of the token could not be identified.
        /// </summary>
        Unknown,
    }
}