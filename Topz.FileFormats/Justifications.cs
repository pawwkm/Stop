namespace Topz.FileFormats
{
    /// <summary>
    /// Justifications for ignoring static analysis warnings.
    /// </summary>
    internal static class Justifications
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
    }
}