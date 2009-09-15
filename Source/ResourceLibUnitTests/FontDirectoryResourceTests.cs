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
    public class FontDirectoryResourceTests
    {
        [Test]
        public void TestLoadFontDirectoryResources()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            string filename = Path.Combine(uriPath, @"Binaries\custom.exe");
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                Assert.AreEqual(1, ri[Kernel32.ResourceTypes.RT_FONTDIR].Count);
                foreach (FontDirectoryResource rc in ri[Kernel32.ResourceTypes.RT_FONTDIR])
                {
                    Console.WriteLine("FontDirectoryResource: {0}, {1}", rc.Name, rc.TypeName);
                    DumpResource.Dump(rc);
                }
            }
        }
    }
}
