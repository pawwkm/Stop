namespace Topz
{
    /// <summary>
    /// Defines an interface for performing a deep copy.
    /// </summary>
    /// <typeparam name="T">The type that is copied.</typeparam>
    public interface IDeepCopy<T>
    {
        /// <summary>
        /// Deep copies all the data of the object.
        /// </summary>
        /// <returns>A deep copy of the object.</returns>
        T DeepCopy();
    }
}