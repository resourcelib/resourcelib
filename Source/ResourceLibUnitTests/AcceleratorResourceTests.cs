using System;
using System.Collections.Generic;
using System.Text;
using Vestris.ResourceLib;
using System.IO;
using System.Web;
using NUnit.Framework;
using System.Reflection;
using System.Drawing;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class AcceleratorResourceTests
    {
        [Test]
        public void TestLoadAcceleratorResources()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            string filename = Path.Combine(uriPath, @"Binaries\custom.exe");
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                Assert.AreEqual(2, ri[Kernel32.ResourceTypes.RT_ACCELERATOR].Count);
                foreach (AcceleratorResource rc in ri[Kernel32.ResourceTypes.RT_ACCELERATOR])
                {
                    Console.WriteLine("AcceleratorResource: {0}, {1}", rc.Name, rc.TypeName);
                    DumpResource.Dump(rc);
                }
                Assert.AreEqual(109, ri[Kernel32.ResourceTypes.RT_ACCELERATOR][0].Name.Id.ToInt32());
                Assert.AreEqual(110, ri[Kernel32.ResourceTypes.RT_ACCELERATOR][1].Name.Id.ToInt32());
            }
        }
    }
}
