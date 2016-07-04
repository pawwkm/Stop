using Pote.CommandLine;

namespace Linker
{
    /// <summary>
    /// The main class of the progam. 
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Links a set of object files to a program or a single object file.
        /// </summary>
        /// <param name="args">The object files to link and the destination file path.</param>
        /// <returns>The exit code of the program.</returns>
        private static int Main(string[] args)
        {
            Interpreter interpreter = new Interpreter();
            interpreter.AddCommand<LinkCommand>();

            return interpreter.Run(args);
        }
    }
}