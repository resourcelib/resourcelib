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
        private static IEnumerable<string> TestFiles {
            get {
                Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
                return Directory.GetFiles(Path.Combine(uriPath, "Binaries"));
            }
        }

        [TestCaseSource("TestFiles")]
        public void SampleEnumerateResources(string filename)
        {
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
        }

        [NonParallelizable]
        [TestCaseSource("TestFiles")]        
        public void TestReadWriteResourceBytes(string path)
        {
            var filename = Path.GetFileName(path);
            var isDotNet = filename.StartsWith("ClassLibrary_NET") || filename == "idea64.exe";
            if (filename == "idea64.exe")
            {
                Assert.Ignore("idea64.exe doesn't consider null byte for StringTableEntry length");
            }

            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(path);
                foreach (Resource rc in ri)
                {
                    Console.WriteLine("Resource: {0} - {1}", rc.TypeName, rc.Name);
                    GenericResource genericResource = new GenericResource(rc.Type, rc.Name, rc.Language);                    
                    genericResource.LoadFrom(path);
                    try
                    {
                        StringTableEntry.ConsiderPaddingForLength = isDotNet;
                        byte[] data = rc.WriteAndGetBytes();                        
                        ByteUtils.CompareBytes(genericResource.Data, data);
                    }
                    finally
                    {
                        StringTableEntry.ConsiderPaddingForLength = false;
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

        [Test]
        public void TestBatchUpdate()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string originalFilename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\gutils.dll");
            string filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\gutils_changed.dll");
            if (File.Exists(filename)) File.Delete(filename);
            File.Copy(originalFilename, filename);
            Assert.IsTrue(File.Exists(filename));
            Console.WriteLine("Filename: {0}", filename);

            // read existing string table
            var sr = new StringResource();
            sr.Name = new ResourceId(StringResource.GetBlockId(402));
            sr.Language = 1033;
            sr.LoadFrom(filename);
            Console.WriteLine("StringResource before patching: {0}, {1}, {2}", sr.Name, sr.TypeName, sr.Language);
            DumpResource.Dump(sr);
            Assert.IsNotNull(sr);
            Assert.AreEqual("Out Of Memory", sr[402]);

            // change string and save it
            sr[402] = "OOM";
            Resource.Save(filename, new[] { sr });

            // re-read string table and verify the changed string table entry
            sr = new StringResource();
            sr.Name = new ResourceId(StringResource.GetBlockId(402));
            sr.Language = 1033;
            sr.LoadFrom(filename);
            Console.WriteLine("StringResource after patching: {0}, {1}, {2}", sr.Name, sr.TypeName, sr.Language);
            DumpResource.Dump(sr);
            Assert.AreEqual("OOM", sr[402]);
        }
    }
}
