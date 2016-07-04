namespace Topz.FileSystems
{
    /// <summary>
    /// Justifications for ignoring static analysis warnings.
    /// </summary>
    internal static class Justifications
    {
        /// <summary>
        /// The underlying type is required while language interoperability is not.
        /// </summary>
        public const string UnderlyingTypeIsRequired = "The underlying type is required while language interoperability is not.";

        /// <summary>
        /// Merging this namespace with another namespace would make sense.
        /// </summary>
        public const string MergingWouldNotMakeSense = "Merging this namespace with another namespace would make sense.";

        /// <summary>
        /// The 'consequences' of this call stack are intentional.
        /// </summary>
        public const string CallStackIsCorrect = "The 'consequences' of this call stack are intentional.";

        /// <summary>
        /// This works do NOT touch it!
        /// </summary>
        /// <remarks>Do not use this one just for silencing a warning!</remarks>
        public const string ItWorksNoTouchy = "This works do NOT touch it!";
    }
}