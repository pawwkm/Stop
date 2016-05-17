using NUnit.Framework;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Management;
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


                //var block = new byte[512];
                //disk.Read(block, 0, 512);

                //File.WriteAllBytes("D:\\Dump.bin", block);

                //var bytes = Encoding.UTF8.GetBytes("Cock");
                //var buffer = new byte[512];

                //for (int i = 0; i < bytes.Length; i++)
                //    buffer[i] = bytes[i];

                //disk.Write(buffer, 0, buffer.Length);
            }
        }
    }
}