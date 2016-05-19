using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Assert.True(system.Exist("F:\\ReadMe.txt"));
            }
        }
    }
}
