using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using Vestris.ResourceLib;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class ResourceInfoTests
    {
        [Test]
        public void TestLoad()
        {
            string[] files = 
            {
                Path.Combine(Environment.SystemDirectory, "regedt32.exe"),
                Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll")
            };

            foreach (string filename in files)
            {
                Assert.IsTrue(File.Exists(filename));
                using (ResourceInfo vi = new ResourceInfo())
                {
                    vi.Load(filename);
                    DumpResource.Dump(vi);
                }
            }
        }
    }
}
