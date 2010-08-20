using System;
using System.Collections.Generic;
using System.Text;
using Vestris.ResourceLib;
using System.IO;
using System.Web;
using NUnit.Framework;
using System.Reflection;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class ManifestResourceTests
    {
        [Test]
        public void TestLoadManifestResources()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "write.exe");
            Assert.IsTrue(File.Exists(filename));
            ManifestResource manifestResource = new ManifestResource();
            manifestResource.LoadFrom(filename);
            DumpResource.Dump(manifestResource);
            Assert.AreEqual(Kernel32.ManifestType.CreateProcess, manifestResource.ManifestType);
        }

        [Test]
        public void TestLoadAndSaveCreateProcessManifestResource()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "write.exe");
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".exe");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            ManifestResource manifestResource = new ManifestResource(Kernel32.ManifestType.CreateProcess);
            manifestResource.SaveTo(targetFilename);
            Console.WriteLine("Written manifest:");
            ManifestResource newManifestResource = new ManifestResource();
            newManifestResource.LoadFrom(targetFilename);
            DumpResource.Dump(newManifestResource);
            File.Delete(targetFilename);
        }

        [Test]
        public void TestLoadAndSaveIsolationAwareManifestResource()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "write.exe");
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".exe");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            ManifestResource manifestResource = new ManifestResource(Kernel32.ManifestType.IsolationAware);
            manifestResource.SaveTo(targetFilename);
            Console.WriteLine("Written manifest:");
            ManifestResource newManifestResource = new ManifestResource();
            newManifestResource.LoadFrom(targetFilename, Kernel32.ManifestType.IsolationAware);
            DumpResource.Dump(newManifestResource);
            File.Delete(targetFilename);
        }

        [Test]
        public void TestLoadUnicodeManifestResourceWithBOM()
        {
            // the 6840.dll has a manifest with a BOM marker (the actual file)
            ManifestResource mr = new ManifestResource();
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string dll = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\6840.dll");
            mr.LoadFrom(dll, Kernel32.ManifestType.IsolationAware);
            Assert.IsNotNull(mr.Manifest);
            Console.WriteLine(mr.Manifest.OuterXml);
        }
    }
}
