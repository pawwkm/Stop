using System;

namespace Topz.Text
{
    /// <summary>
    /// A position in source code.
    /// </summary>
    public sealed class Position : IDeepCopy<Position>
    {
        private string file = "";

        /// <summary>
        /// The column in the input.
        /// </summary>
        public int Column
        {
            get;
            internal set;
        } = 1;

        /// <summary>
        /// The line in the input.
        /// </summary>
        public int Line
        {
            get;
            internal set;
        } = 1;

        /// <summary>
        /// The file that this position is within.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public string File
        {
            get
            {
                return file;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                file = value;
            }
        }

        /// <summary>
        /// Returns a string that represents this position.
        /// </summary>
        /// <returns>A string that represents this position.</returns>
        public override string ToString()
        {
            if (File.Length == 0)
                return $"{Line}:{Column}";

            return $"{File}:{Line}:{Column}";
        }

        /// <summary>
        /// Returns a string that represents this position with a message.
        /// </summary>
        /// <param name="message">The message following the position.</param>
        /// <returns>A string that represents this position.</returns>
        public string ToString(string message)
        {
            return $"{ToString()}: {message}";
        }

        /// <summary>
        /// Deep copies this position.
        /// </summary>
        /// <returns>A deep copy of the position.</returns>
        public Position DeepCopy()
        {
            return new Position()
            {
                Line = Line,
                Column = Column,
                File = File
            };
        }
    }
}