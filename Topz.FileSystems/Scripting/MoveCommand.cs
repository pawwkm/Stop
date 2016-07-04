using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topz.FileSystems.Scripting
{
    /// <summary>
    /// Moves one or more files to and from a virtualized file system.
    /// </summary>
    internal class MoveCommand : FileCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveCommand"/> class.
        /// </summary>
        /// <param name="source">The source to get file(s) from.</param>
        /// <param name="destination">The destination where the file(s) is moved to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="destination"/>.
        /// </exception>
        public MoveCommand(string source, string destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            Source = source;
            Destination = destination;
        }

        /// <summary>
        /// The path to the source to move.
        /// </summary>
        public string Source
        {
            get;
            private set;
        }

        /// <summary>
        /// The path to the destination of the 
        /// file(s) given by <see cref="Source"/>.
        /// This must be a directory.
        /// </summary>
        public string Destination
        {
            get;
            private set;
        }

        public override void Execute(Context context)
        {
            base.Execute(context);
            if (FileSystem == null)
                return;

            if (Source.StartsWith("/"))
            {

            }
            else
            {
                string[] files = null;
                if (IsHostPathADirctory(Source))
                    files = Directory.GetFiles(Source, "*.*", SearchOption.AllDirectories);
                else
                    files = new[] { Source };

                if (!Destination.StartsWith("/"))
                    throw new InvalidOperationException();

                MoveFilesFromHostToVirtual(files);
            }
        }

        private void MoveFilesFromHostToVirtual(IEnumerable<string> files)
        {
            string destination = Destination.Substring(1);
            foreach (string file in files)
            {
                string path = destination + Path.GetFileName(file);

                if (!FileSystem.Exist(path))
                    FileSystem.Create(path);

                using (Stream s = File.OpenRead(file))
                {
                    using (Stream d = FileSystem.Open(path))
                    {
                        s.CopyTo(d);
                        d.Flush();
                    }
                }
            }
        }

        private bool IsHostPathADirctory(string path)
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(path);

                return attributes.HasFlag(FileAttributes.Directory);
            }
            catch (DirectoryNotFoundException)
            {
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }
    }
}