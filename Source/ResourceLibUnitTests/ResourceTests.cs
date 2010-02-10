#region Example: Using Headers
using System;
using System.IO;
using Vestris.ResourceLib;
#endregion

using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using System.Web;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class ResourceTests
    {
        [Test]
        public void SampleEnumerateResources()
        {
            #region Example: Enumerating Resources
            string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
            using (ResourceInfo vi = new ResourceInfo())
            {
                vi.Load(filename);
                foreach (ResourceId id in vi.ResourceTypes)
                {
                    Console.WriteLine(id);
                    foreach (Resource resource in vi.Resources[id])
                    {
                        Console.WriteLine("{0} ({1}) - {2} byte(s)",
                            resource.Name, resource.Language, resource.Size);
                    }
                }
            }
            #endregion
        }

        [Test]
        public void TestReadWriteResourceBytes()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            foreach (string filename in Directory.GetFiles(Path.Combine(uriPath, "Binaries")))
            {
                Console.WriteLine(filename);
                using (ResourceInfo ri = new ResourceInfo())
                {
                    ri.Load(filename);
                    foreach (Resource rc in ri)
                    {
                        Console.WriteLine("Resource: {0} - {1}", rc.TypeName, rc.Name);
                        GenericResource genericResource = new GenericResource(rc.Type, rc.Name, rc.Language);
                        genericResource.LoadFrom(filename);
                        byte[] data = rc.WriteAndGetBytes();
                        ByteUtils.CompareBytes(genericResource.Data, data);
                    }
                }
            }
        }

        [Test]
        public void TestCustom()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\custom.exe");
            Assert.IsTrue(File.Exists(filename));
            using (ResourceInfo vi = new ResourceInfo())
            {
                vi.Load(filename);
                // version resources (well-known)
                List<Resource> versionResources = vi[Kernel32.ResourceTypes.RT_VERSION]; // RT_VERSION
                Assert.IsNotNull(versionResources);
                Assert.AreEqual(1, versionResources.Count);
                Resource versionResource = versionResources[0];
                Assert.AreEqual(versionResource.Name.ToString(), "1");
                Assert.AreEqual(versionResource.Type.ToString(), "16");
                // custom resources
                List<Resource> customResources = vi["CUSTOM"];
                Assert.IsNotNull(customResources);
                Assert.AreEqual(1, customResources.Count);
                Resource customResource = customResources[0];
                // check whether the properties are string representations
                Assert.AreEqual(customResource.Name.ToString(), "RES_CONFIGURATION");
                Assert.AreEqual(customResource.Type.ToString(), "CUSTOM");
            }
        }
    }
}
