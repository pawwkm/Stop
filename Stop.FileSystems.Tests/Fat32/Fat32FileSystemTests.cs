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
        /// 
        /// </summary>
        [Test]
        public void Test()
        {
            using (var file = File.Open("D:\\Fat32.bin", FileMode.Open))
            {
                var system = new Fat32FileSystem(file);
                system.Create("bootcode.bin");

                Assert.True(system.Exist("bootcode.bin"));
            }
        }
    }
}
