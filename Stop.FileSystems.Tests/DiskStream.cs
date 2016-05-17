using NUnit.Framework;
using System.Windows.Forms;

namespace Stop.FileSystems
{
    /// <summary>
    /// Provides tests for the <see cref="DiskStream"/> class.
    /// </summary>
    [TestFixture, Explicit]
    public class DiskStreamTests
    {
        /// <summary>
        /// Figures out which disk to test on.
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            MessageBox.Show("Only once");
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void Test1()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void Test2()
        {

        }
    }
}