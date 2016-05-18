using NUnit.Framework;
using System.IO;
using System.Text;

namespace Stop.FileSystems
{
    /// <summary>
    /// Provides tests for the <see cref="DiskStream"/> class.
    /// </summary>
    [TestFixture, Explicit]
    public class DiskStreamTests
    {
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void Test1()
        {
            using (var disk = new DiskStream(1, true))
            {
            }
        }
    }
}