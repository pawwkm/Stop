using System;
using System.Runtime.Serialization;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// The exception that is thrown when an instruction can't be encoded.
    /// </summary>
    [Serializable]
    public class EncodingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingException"/> class.
        /// </summary>
        public EncodingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingException"/> 
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EncodingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception, or a null reference
        /// if no inner exception is specified.
        /// </param>
        public EncodingException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingException"/> class 
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
        protected EncodingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}