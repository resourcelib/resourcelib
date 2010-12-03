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
                // Path.Combine(Environment.SystemDirectory, "regedt32.exe"),
                // Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe"),
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

        //[Test]
        //public void FindSmallestBinaryWithResources()
        //{
        //    FindSmallestBinaryWithResources(
        //        Environment.SystemDirectory,
        //        "*.exe", 
        //        Kernel32.ResourceTypes.RT_MENU);
        //}

        private void FindSmallestBinaryWithResources(
            string path, string ext, Kernel32.ResourceTypes rt)
        {
            long smallest = 0;
            string[] files = Directory.GetFiles(path, ext);
            foreach (string filename in files)
            {
                try
                {
                    using (ResourceInfo ri = new ResourceInfo())
                    {
                        ri.Load(filename);

                        if (!ri.Resources.ContainsKey(new ResourceId(rt)))
                            continue;

                        FileInfo fi = new FileInfo(filename);
                        //if (fi.Length < smallest || smallest == 0)
                        //{
                            Console.WriteLine("{0} {1}", filename, new FileInfo(filename).Length);
                            smallest = fi.Length;
                        // }
                        break;
                    }
                }
                catch
                {
                }
            }
        }
    }
}
