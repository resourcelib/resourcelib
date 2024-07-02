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
    public class FontResourceTests
    {
        [Test]
        public void TestLoadFontResources()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            string filename = Path.Combine(uriPath, @"Binaries\custom.exe");
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                Assert.AreEqual(2, ri[Kernel32.ResourceTypes.RT_FONT].Count);
                foreach (FontResource rc in ri[Kernel32.ResourceTypes.RT_FONT])
                {
                    Console.WriteLine("FontResource: {0}, {1}", rc.Name, rc.TypeName);
                    DumpResource.Dump(rc);
                }
            }
        }
    }
}
