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
    public class CursorResourceTests
    {
        [Test]
        public void TestLoadCursorResources()
        {
            // gutils.dll has two cursors
            string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll");
            Assert.IsTrue(File.Exists(filename));
            string[] cursorNames = { "HORZLINE", "VERTLINE" };
            foreach (string cursorName in cursorNames)
            {
                CursorDirectoryResource cursorDirectoryResource = new CursorDirectoryResource();
                cursorDirectoryResource.Name = new ResourceId(cursorName);
                cursorDirectoryResource.LoadFrom(filename);
                Assert.AreEqual(cursorDirectoryResource.Icons.Count, 1);
                Assert.AreEqual(cursorDirectoryResource.Icons[0].HotspotX, 16);
                Assert.AreEqual(cursorDirectoryResource.Icons[0].HotspotY, 16);
                DumpResource.Dump(cursorDirectoryResource);
            }
        }

        [Test]
        public void TestLoadAndSaveCursorResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string filename = HttpUtility.UrlDecode(uri.AbsolutePath);
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "testLoadAndSaveCursorResource.exe");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);

            string gutilsfilename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll");
            Assert.IsTrue(File.Exists(gutilsfilename));
            string[] cursorNames = { "HORZLINE", "VERTLINE" };
            foreach (string cursorName in cursorNames)
            {
                CursorDirectoryResource cursorDirectoryResource = new CursorDirectoryResource();
                cursorDirectoryResource.Name = new ResourceId(cursorName);
                cursorDirectoryResource.Language = ResourceUtil.USENGLISHLANGID;
                cursorDirectoryResource.LoadFrom(gutilsfilename);
                cursorDirectoryResource.SaveTo(targetFilename);
            }

            Console.WriteLine("Written CursorDirectoryResource:");
            foreach (string cursorName in cursorNames)
            {
                CursorDirectoryResource newCursorDirectoryResource = new CursorDirectoryResource();
                newCursorDirectoryResource.Name = new ResourceId(cursorName);
                newCursorDirectoryResource.Language = ResourceUtil.USENGLISHLANGID;
                newCursorDirectoryResource.LoadFrom(targetFilename);
                DumpResource.Dump(newCursorDirectoryResource);
            }
        }

        [Test]
        public void TestCompareCursor1()
        {
            // load from gutils
            string gutilsfilename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll");
            Assert.IsTrue(File.Exists(gutilsfilename));
            CursorDirectoryResource cursorDirectoryResource = new CursorDirectoryResource();
            cursorDirectoryResource.Name = new ResourceId("HORZLINE");
            cursorDirectoryResource.Language = ResourceUtil.USENGLISHLANGID;
            cursorDirectoryResource.LoadFrom(gutilsfilename);
            // load from cursor1.cur
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string cursor1filename = Path.Combine(Path.GetDirectoryName(
                HttpUtility.UrlDecode(uri.AbsolutePath)), "Cursors\\Cursor1.cur");
            Assert.IsTrue(File.Exists(cursor1filename));
            IconFile cursorFile = new IconFile(cursor1filename);
            Assert.AreEqual(cursorDirectoryResource.Icons[0].Image.Data.Length, 
                cursorFile.Icons[0].Image.Data.Length + 4);
        }

        [Test]
        public void TestAddCursorResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string cursor1filename = Path.Combine(Path.GetDirectoryName(
                HttpUtility.UrlDecode(uri.AbsolutePath)), "Cursors\\Cursor1.cur");
            Assert.IsTrue(File.Exists(cursor1filename));
            IconFile cursorFile = new IconFile(cursor1filename);

            Console.WriteLine("{0}: {1}", Path.GetFileName(cursor1filename), cursorFile.Type);
            foreach (IconFileIcon cursor in cursorFile.Icons)
            {
                Console.WriteLine(" {0}", cursor.ToString());
            }

            Console.WriteLine("Converted CursorDirectoryResource:");
            CursorDirectoryResource cursorDirectoryResource = new CursorDirectoryResource(cursorFile);
            Assert.AreEqual(16, cursorDirectoryResource.Icons[0].HotspotX);
            Assert.AreEqual(16, cursorDirectoryResource.Icons[0].HotspotY);
            cursorDirectoryResource.Name = new ResourceId("RESOURCELIB");
            cursorDirectoryResource.Language = ResourceUtil.USENGLISHLANGID;
            cursorDirectoryResource.Icons[0].HotspotX = 12;
            cursorDirectoryResource.Icons[0].HotspotY = 12;
            cursorDirectoryResource.Icons[0].Id = 3;
            cursorDirectoryResource.Icons[0].Language = ResourceUtil.USENGLISHLANGID;
            DumpResource.Dump(cursorDirectoryResource);
            Assert.AreEqual(cursorFile.Icons.Count, cursorDirectoryResource.Icons.Count);

            string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll");
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "testReplaceCursorResource.dll");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            cursorDirectoryResource.SaveTo(targetFilename);

            Console.WriteLine("Written CursorDirectoryResource:");
            CursorDirectoryResource newCursorDirectoryResource = new CursorDirectoryResource();
            newCursorDirectoryResource.Name = new ResourceId("RESOURCELIB");
            newCursorDirectoryResource.LoadFrom(targetFilename);
            Assert.AreEqual(1, newCursorDirectoryResource.Icons.Count);
            Assert.AreEqual(cursorFile.Icons[0].Image.Size + 4, newCursorDirectoryResource.Icons[0].Image.Size);
            DumpResource.Dump(newCursorDirectoryResource);

            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(targetFilename);
                DumpResource.Dump(ri);
            }
        }
    }
}
