namespace Topz
{
    /// <summary>
    /// Justifications for ignoring static analysis warnings.
    /// </summary>
    public static class Justifications
    {
        /// <summary>
        /// The conflict is acceptable because the client code should have intimate knowledge about the system.
        /// </summary>
        public const string AcceptableConflict = "The conflict is acceptable because the client code should have intimate knowledge about the system.";

        /// <summary>
        /// The 'Collection' suffix would no make sense in this context.
        /// </summary>
        public const string CollectionSuffixNotApplicable = "The 'Collection' suffix would no make sense in this context.";

        /// <summary>
        /// Instance access may be needed later on and the change from a static class to an instance one could cause major breakage.
        /// </summary>
        public const string InstanceAccessMayBeNeededLater = "Instance access may be needed later on and the change from a static class to an instance one could cause major breakage.";

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

        /// <summary>
        /// Though there are many cases they are simple.
        /// </summary>
        public const string SimpleSwitch = "Though there are many cases they are simple.";
    }
}