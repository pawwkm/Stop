using System;
using System.Linq;

namespace Topz.FileSystems
{
    /// <summary>
    /// Defines an <see cref="ISerializer{T}"/> to use for a class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SerializerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerAttribute"/> class.
        /// </summary>
        /// <param name="serializer">The type of serializer to associate with a class.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serializer"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="serializer"/> doesn't implement the <see cref="ISerializer{T}"/> interface.
        /// <paramref name="serializer"/> doesn't have a parameterless constructor.
        /// </exception>
        public SerializerAttribute(Type serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            bool implementsInterface = serializer.GetInterfaces()
                                       .Where(i => i.IsGenericType)
                                       .Any(i => i.GetGenericTypeDefinition() == typeof(ISerializer<>));

            if (!implementsInterface)
                throw new ArgumentException("The type is not a serializer.", nameof(serializer));
            if (serializer.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException("The type has no parameterless constructor.", nameof(serializer));

            Serializer = serializer;
        }

        /// <summary>
        /// The serializer associated with the class.
        /// </summary>
        public Type Serializer
        {
            get;
            private set;
        }
    }
}