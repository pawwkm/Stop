using Pote.Text;
using System;
using System.IO;
using Topz.FileSystems.Scripting;

namespace Disk
{
    /// <summary>
    /// The main class of the program. 
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Runs a script or starts interactive mode depending on the given <paramref name="arguments"/>.
        /// </summary>
        /// <param name="arguments">A path to a script or nothing.</param>
        private static void Main(string[] arguments)
        {
            if (arguments.Length == 0)
                InteractiveMode();
            else
                RunScript(arguments[0]);
        }

        /// <summary>
        /// Enters interactive scripting mode.
        /// </summary>
        private static void InteractiveMode()
        {
            var script = new Script();
            while (true)
            {
                var code = Console.ReadLine();
                if (code.ToLower() == "exit")
                    break;

                try
                {
                    script.Run(code);
                }
                catch (ParsingException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Runs a script and exits the program.
        /// </summary>
        /// <param name="path">The path to the script to run.</param>
        private static void RunScript(string path)
        {
            try
            {
                using (var stream = File.OpenRead(path))
                {
                    var script = new Script(stream);
                    script.Run();
                }
            }
            catch (ParsingException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}