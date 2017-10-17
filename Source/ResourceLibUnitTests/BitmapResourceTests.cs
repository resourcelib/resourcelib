#if !NETSTANDARD
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
    public class BitmapResourceTests
    {
        [Test]
        public void TestLoadBitmapResources()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "msftedit.dll");
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                foreach(BitmapResource rc in ri[Kernel32.ResourceTypes.RT_BITMAP])
                {
                    Console.WriteLine("BitmapResource: {0}, {1}", rc.Name, rc.TypeName);
                    Console.WriteLine("DIB: {0}x{1} {2}", rc.Bitmap.Image.Width, rc.Bitmap.Image.Height, 
                        rc.Bitmap.Header.PixelFormatString);
                }
            }
        }

        [Test]
        public void TestLoadBitmapResource()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "msftedit.dll");
            BitmapResource rc = new BitmapResource();
            rc.Name = new ResourceId(125);
            rc.LoadFrom(filename);
            Console.WriteLine("BitmapResource: {0}, {1}", rc.Name, rc.TypeName);
            Console.WriteLine("DIB: {0}x{1} {2}", rc.Bitmap.Image.Width, rc.Bitmap.Image.Height, rc.Bitmap.Header.PixelFormatString);
        }

        [Test]
        public void TestLoadAndSaveBitmapResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string bitmapsdirectory = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Bitmaps");
            foreach (string bitmapfilename in Directory.GetFiles(bitmapsdirectory))
            {
                if (!bitmapfilename.EndsWith(".bmp"))
                    continue;

                Assert.IsTrue(File.Exists(bitmapfilename));
                Image imageFile = Bitmap.FromFile(bitmapfilename);
                Console.WriteLine("{0}: {1}x{2}", Path.GetFileName(bitmapfilename), imageFile.Width, imageFile.Height);

                string filename = HttpUtility.UrlDecode(uri.AbsolutePath);
                Assert.IsTrue(File.Exists(filename));
                string targetFilename = Path.Combine(Path.GetTempPath(), "testLoadAndSaveBitmapResource.exe");
                File.Copy(filename, targetFilename, true);
                Console.WriteLine(targetFilename);

                BitmapResource bitmapResource = new BitmapResource();
                bitmapResource.Name = new ResourceId("RESOURCELIB");
                BitmapFile bitmapFile = new BitmapFile(bitmapfilename);
                bitmapResource.Bitmap = bitmapFile.Bitmap;
                Console.WriteLine("DIB: {0}x{1}", bitmapResource.Bitmap.Image.Width, bitmapResource.Bitmap.Image.Height);
                bitmapResource.SaveTo(targetFilename);

                Console.WriteLine("Written BitmapResource:");
                BitmapResource newBitmapResource = new BitmapResource();
                newBitmapResource.Name = new ResourceId("RESOURCELIB");
                newBitmapResource.LoadFrom(targetFilename);
                DumpResource.Dump(newBitmapResource);

                Image newBitmapResourceImage = newBitmapResource.Bitmap.Image;
                Assert.AreEqual(imageFile.Width, newBitmapResourceImage.Width);
                Assert.AreEqual(imageFile.Height, newBitmapResourceImage.Height);
            }
        }

        [Test]
        public void TestReplaceBitmapResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string bitmapsdirectory = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Bitmaps");
            foreach (string bitmapfilename in Directory.GetFiles(bitmapsdirectory))
            {
                if (!bitmapfilename.EndsWith(".bmp"))
                    continue;

                Image imageFile = Bitmap.FromFile(bitmapfilename);
                Console.WriteLine("{0}: {1}x{2}", Path.GetFileName(bitmapfilename), imageFile.Width, imageFile.Height);

                string filename = Path.Combine(Environment.SystemDirectory, "msftedit.dll");
                Assert.IsTrue(File.Exists(filename));
                string targetFilename = Path.Combine(Path.GetTempPath(), "testLoadAndSaveBitmapResource.exe");
                File.Copy(filename, targetFilename, true);
                Console.WriteLine(targetFilename);

                BitmapResource bitmapResource = new BitmapResource();
                using (ResourceInfo targetFilenameResources = new ResourceInfo())
                {
                    targetFilenameResources.Load(targetFilename);
                    Resource existingBitmapResource = targetFilenameResources[Kernel32.ResourceTypes.RT_BITMAP][0];
                    bitmapResource.Name = existingBitmapResource.Name;
                    bitmapResource.Language = existingBitmapResource.Language;
                    Console.WriteLine("Replacing {0}", bitmapResource.Name);
                }

                BitmapFile bitmapFile = new BitmapFile(bitmapfilename);
                bitmapResource.Bitmap = bitmapFile.Bitmap;
                Console.WriteLine("DIB: {0}x{1}", bitmapResource.Bitmap.Image.Width, bitmapResource.Bitmap.Image.Height);
                bitmapResource.SaveTo(targetFilename);

                Console.WriteLine("Written BitmapResource:");
                BitmapResource newBitmapResource = new BitmapResource();
                newBitmapResource.Name = bitmapResource.Name;
                newBitmapResource.LoadFrom(targetFilename);
                DumpResource.Dump(newBitmapResource);

                Image newBitmapResourceImage = newBitmapResource.Bitmap.Image;
                Assert.AreEqual(imageFile.Width, newBitmapResourceImage.Width);
                Assert.AreEqual(imageFile.Height, newBitmapResourceImage.Height);
            }
        }
    }
}
#endif
