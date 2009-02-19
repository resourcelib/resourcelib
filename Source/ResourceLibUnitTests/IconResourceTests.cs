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

            Console.WriteLine("Converted GroupIconResource:");
            GroupIconResource groupIconResource = iconFile.ConvertToGroupIconResource();
            DumpResource.Dump(groupIconResource);
            Assert.AreEqual(iconFile.Icons.Count, groupIconResource.Icons.Count);

            string filename = HttpUtility.UrlDecode(uri.AbsolutePath);
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "test.dll");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            groupIconResource.SaveTo(targetFilename);

            Console.WriteLine("Written GroupIconResource:");
            GroupIconResource newGroupIconResource = new GroupIconResource();
            newGroupIconResource.LoadFrom(targetFilename);
            DumpResource.Dump(newGroupIconResource);
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

            Console.WriteLine("Converted GroupIconResource:");
            GroupIconResource groupIconResource = iconFile.ConvertToGroupIconResource();
            DumpResource.Dump(groupIconResource);
            Assert.AreEqual(iconFile.Icons.Count, groupIconResource.Icons.Count);

            string filename = Path.Combine(Environment.SystemDirectory, "write.exe");
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "write.exe");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            groupIconResource.SaveTo(targetFilename);

            Console.WriteLine("Written GroupIconResource:");
            GroupIconResource newGroupIconResource = new GroupIconResource();
            newGroupIconResource.LoadFrom(targetFilename);
            DumpResource.Dump(newGroupIconResource);
        }
    }
}
