using Pote.CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using Topz.FileFormats.Atom;

namespace Linker
{
    /// <summary>
    /// Links one or more object files into a binary executable.
    /// </summary>
    internal class LinkCommand : Command
    {
        /// <summary>
        /// The destination file for linker result.
        /// </summary>
        [Option('o', "out", Help = "The destination file for linker result.", IsRequired = true)]
        public string Destination
        {
            get;
            set;
        }

        /// <summary>
        /// The object files to link.
        /// </summary>
        [Option(Help = "The object files to link", MetaName = "object files", Default = new string[0])]
        [FileInterceptor("o", SearchAllDirectories = true)]
        [ExtensionValidator("o", IgnoreNonFilePaths = true)]
        public IEnumerable<string> ObjectFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Links the given object files into an executable binary.
        /// </summary>
        /// <returns>The exit code of the link process.</returns>
        public override int Execute()
        {
            List<ObjectFile> files = new List<ObjectFile>();
            foreach (string path in ObjectFiles)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine(path + " does't exist.");

                    return 1;
                }

                using (Stream stream = File.OpenRead(path))
                {
                    AtomReader reader = new AtomReader();
                    files.Add(reader.Read(stream));
                }
            }

            using (Stream destination = File.Create(Destination))
            {
                AtomLinker linker = new AtomLinker();
                linker.Link(files, destination);
            }

            return 0;
        }
    }
}