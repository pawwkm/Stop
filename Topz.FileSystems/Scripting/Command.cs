using System;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// A disk command.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Executes the disk command.
        /// </summary>
        /// <param name="context">The context of the script.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is null.
        /// </exception>
        public abstract void Execute(Context context);
    }
}