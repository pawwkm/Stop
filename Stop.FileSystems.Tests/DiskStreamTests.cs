using NUnit.Framework;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Management;

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
            //MessageBox.Show("Only once");
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void Test1()
        {
            using (var disk = new DiskStream(1))
            {

            }
        }
    }
}