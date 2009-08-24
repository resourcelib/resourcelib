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
    public class IconResourceTests
    {
        [Test]
        public void TestLoadIconResources()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "regedt32.exe");
            Assert.IsTrue(File.Exists(filename));
            IconDirectoryResource iconDirectoryResource = new IconDirectoryResource();
            iconDirectoryResource.LoadFrom(filename);
            DumpResource.Dump(iconDirectoryResource);
        }

        [Test]
        public void TestLoadAndSaveIconResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string icon1filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Icons\\Icon1.ico");
            Assert.IsTrue(File.Exists(icon1filename));
            IconFile iconFile = new IconFile(icon1filename);

            Console.WriteLine("{0}: {1}", Path.GetFileName(icon1filename), iconFile.Type);
            foreach (IconFileIcon icon in iconFile.Icons)
            {
                Console.WriteLine(" {0}", icon.ToString());
            }

            Console.WriteLine("Converted IconDirectoryResource:");
            IconDirectoryResource iconDirectoryResource = iconFile.ConvertToIconDirectoryResource(
                new ResourceId(Kernel32.ResourceTypes.RT_ICON));
            DumpResource.Dump(iconDirectoryResource);
            Assert.AreEqual(iconFile.Icons.Count, iconDirectoryResource.Icons.Count);

            string filename = HttpUtility.UrlDecode(uri.AbsolutePath);
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".exe");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            iconDirectoryResource.SaveTo(targetFilename);

            Console.WriteLine("Written IconDirectoryResource:");
            IconDirectoryResource newIconDirectoryResource = new IconDirectoryResource();
            newIconDirectoryResource.LoadFrom(targetFilename);
            DumpResource.Dump(newIconDirectoryResource);
            File.Delete(targetFilename);
        }

        [Test]
        public void TestReplaceIconResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string icon1filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Icons\\Icon1.ico");
            Assert.IsTrue(File.Exists(icon1filename));
            IconFile iconFile = new IconFile(icon1filename);

            Console.WriteLine("{0}: {1}", Path.GetFileName(icon1filename), iconFile.Type);
            foreach (IconFileIcon icon in iconFile.Icons)
            {
                Console.WriteLine(" {0}", icon.ToString());
            }

            Console.WriteLine("Converted IconDirectoryResource:");
            IconDirectoryResource iconDirectoryResource = iconFile.ConvertToIconDirectoryResource(
                new ResourceId(Kernel32.ResourceTypes.RT_ICON));
            DumpResource.Dump(iconDirectoryResource);
            Assert.AreEqual(iconFile.Icons.Count, iconDirectoryResource.Icons.Count);

            string filename = Path.Combine(Environment.SystemDirectory, "write.exe");
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "write.exe");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            iconDirectoryResource.SaveTo(targetFilename);

            Console.WriteLine("Written IconDirectoryResource:");
            IconDirectoryResource newIconDirectoryResource = new IconDirectoryResource();
            newIconDirectoryResource.LoadFrom(targetFilename);
            DumpResource.Dump(newIconDirectoryResource);
        }
    }
}
