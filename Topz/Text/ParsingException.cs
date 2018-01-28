using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Topz.Text
{
    /// <summary>
    /// The exception that is thrown when a parsing error occurs.
    /// </summary>
    [Serializable]
    public class ParsingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        public ParsingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ParsingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class with 
        /// a specified error message and a reference to the inner exception that is 
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference.</param>
        public ParsingException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> that 
        /// holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual
        /// information about the source or destination.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// The class name is null or System.Exception.HResult is zero.
        /// </exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}