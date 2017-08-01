﻿using System;
using System.Runtime.Serialization;

namespace Topz.Text
{
    /// <summary>
    /// The exception that thrown when an error occur in a lexical analyzer.
    /// </summary>
    [Serializable]
    public class LexicalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalException"/> class.
        /// </summary>
        public LexicalException() 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalException"/> 
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LexicalException(string message) : base(message) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception, or a null reference
        /// if no inner exception is specified.
        /// </param>
        public LexicalException(string message, Exception inner) : base(message, inner) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalException"/> class 
        /// with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="SerializationException">
        /// The class name is null or <see cref="Exception.HResult"/> is zero.
        /// </exception>
        protected LexicalException(SerializationInfo info, StreamingContext context) : base(info, context) 
        { 
        }
    }
}