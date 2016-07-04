using System.Collections.Generic;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// Defines all the keywords.
    /// </summary>
    public static class Keywords
    {
        /// <summary>
        /// The 'select' keyword.
        /// </summary>
        public const string Select = "select";

        /// <summary>
        /// The 'disk' keyword.
        /// </summary>
        public const string Disk = "disk";

        /// <summary>
        /// The 'create' keyword.
        /// </summary>
        public const string Create = "create";

        /// <summary>
        /// The 'mbr' keyword.
        /// </summary>
        public const string Mbr = "mbr";

        /// <summary>
        /// The 'partition' keyword.
        /// </summary>
        public const string Partition = "partition";

        /// <summary>
        /// The 'offset' keyword.
        /// </summary>
        public const string Offset = "offset";

        /// <summary>
        /// The 'sectors' keyword.
        /// </summary>
        public const string Sectors = "sectors";

        /// <summary>
        /// The 'ask' keyword.
        /// </summary>
        public const string Ask = "ask";

        /// <summary>
        /// The 'format' keyword.
        /// </summary>
        public const string Format = "format";

        /// <summary>
        /// The 'fat32' keyword.
        /// </summary>
        public const string Fat32 = "fat32";

        /// <summary>
        /// The 'move' keyword.
        /// </summary>
        public const string Move = "move";

        /// <summary>
        /// The 'to' keyword.
        /// </summary>
        public const string To = "to";

        /// <summary>
        /// All the keywords as a list.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                return new string[]
                {
                    Select,
                    Disk,
                    Create,
                    Mbr,
                    Partition,
                    Offset,
                    Sectors,
                    Ask,
                    Format,
                    Fat32,
                    Move,
                    To
                };
            }
        }
    }
}