using NUnit.Framework;

namespace Topz.FileSystems.Fat32
{
    /// <summary>
    /// Provides tests for the <see cref="FileEntry"/> class.
    /// </summary>
    public class FileEntryTests
    {
        /// <summary>
        /// Tests that <see cref="FileEntry.FirstCluster"/> can handle numbers 
        /// bigger than <see cref="ushort.MaxValue"/>.
        /// </summary>
        [Test]
        public void FirstCluster_BigCluster_Persists()
        {
            FileEntry entry = new FileEntry();
            entry.FirstCluster = ushort.MaxValue + 1;

            Assert.AreEqual(ushort.MaxValue + 1, entry.FirstCluster);
        }

        /// <summary>
        /// Tests that <see cref="FileEntry.FirstCluster"/> can handle numbers 
        /// smaller than <see cref="ushort.MaxValue"/>.
        /// </summary>
        [Test]
        public void FirstCluster_SmallCluster_Persists()
        {
            FileEntry entry = new FileEntry();
            entry.FirstCluster = 512;

            Assert.AreEqual(512, entry.FirstCluster);
        }
    }
}