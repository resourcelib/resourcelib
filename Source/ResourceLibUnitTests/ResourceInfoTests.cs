using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Web;
using Vestris.ResourceLib;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class ResourceInfoTests
    {
        [Test]
        public void TestLoad()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));

            string[] files = 
            {
                Path.Combine(Environment.SystemDirectory, "regedt32.exe"),
                Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe"),
                Path.Combine(uriPath, "Binaries\\gutils.dll"),
                Path.Combine(uriPath, "Binaries\\6to4svc.dll"),
                Path.Combine(uriPath, "Binaries\\custom.exe"),
            };

            foreach (string filename in files)
            {
                Console.WriteLine(filename);
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
