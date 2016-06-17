using NUnit.Framework;
using System.IO;

namespace Stop.FileSystems.Fat32
{
    /// <summary>
    /// Provides tests for the <see cref="Fat32FileSystem"/> class.
    /// </summary>
    public class Fat32FileSystemTests
    {
        /// <summary>
        /// Tests that <see cref="Fat32FileSystem"/> can read and write files.
        /// </summary>
        [Test]
        public void Test()
        {
            string myFile = "myfile.txt";

            using (var file = File.Open("D:\\Fat32.bin", FileMode.Open))
            {
                var system = new Fat32FileSystem(file);
                system.Create(myFile);

                using (var original = File.OpenRead("D:\\Hamlet.txt"))
                {
                    Assert.True(system.Exist(myFile));
                    using (var stream = system.Open(myFile))
                    {
                        original.CopyTo(stream);
                        stream.Flush();
                    }
                }

                using (var dump = File.Create("D:\\copy.txt"))
                {
                    using (var stream = system.Open(myFile))
                    {
                        stream.CopyTo(dump);
                        dump.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Fat32FileSystem"/> can read a file that was already on the image.
        /// </summary>
        [Test]
        public void ReadFile()
        {
            using (var image = File.Open("D:\\Fat32.bin", FileMode.Open))
            {
                var system = new Fat32FileSystem(image);

                using (var file = system.Open("ReadMe.txt"))
                {
                    using (var dump = File.Create("D:\\ReadMe.txt"))
                    {
                        file.CopyTo(dump);
                        dump.Flush();
                    }
                }
            }
        }
    }
}
