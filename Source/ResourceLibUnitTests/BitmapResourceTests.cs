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
        //[Test]
        //public void TestLoadAndSaveBitmapResource()
        //{
        //    Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
        //    string bitmap1filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Bitmaps\\Bitmap1.bmp");
        //    Assert.IsTrue(File.Exists(bitmap1filename));
        //    Image imageFile = Bitmap.FromFile(bitmap1filename);
        //    Console.WriteLine("{0}: {1}x{2}", Path.GetFileName(bitmap1filename), imageFile.Width, imageFile.Height);

        //    string filename = HttpUtility.UrlDecode(uri.AbsolutePath);
        //    Assert.IsTrue(File.Exists(filename));
        //    string targetFilename = Path.Combine(Path.GetTempPath(), "testLoadAndSaveBitmapResource.exe");
        //    File.Copy(filename, targetFilename, true);
        //    Console.WriteLine(targetFilename);

        //    BitmapResource bitmapResource = new BitmapResource();
        //    bitmapResource.Name = new ResourceId("RESOURCELIB");
        //    bitmapResource.Bitmap = new DeviceIndependentBitmap(bitmap1filename);
        //    Console.WriteLine("DIB: {0}x{1}", bitmapResource.Bitmap.Image.Width, bitmapResource.Bitmap.Image.Height);
        //    bitmapResource.SaveTo(targetFilename);

        //    Console.WriteLine("Written BitmapResource:");
        //    BitmapResource newBitmapResource = new BitmapResource();
        //    newBitmapResource.Name = new ResourceId("RESOURCELIB");
        //    newBitmapResource.LoadFrom(targetFilename);
        //    DumpResource.Dump(newBitmapResource);
        //}

        //[Test]
        //public void TestLoadAndResaveBitmapResource()
        //{
        //    string sourceFilename = Path.Combine(Environment.SystemDirectory, "mspaint.exe");
        //    Assert.IsTrue(File.Exists(sourceFilename));
        //    string targetFilename = Path.Combine(Path.GetTempPath(), "testLoadAndResaveBitmapResource.exe");
        //    File.Copy(sourceFilename, targetFilename, true);
        //    Console.WriteLine(targetFilename);

        //    using (ResourceInfo ri = new ResourceInfo())
        //    {
        //        ri.Load(sourceFilename);
        //        BitmapResource rc = (BitmapResource) ri[Kernel32.ResourceTypes.RT_BITMAP][0];
        //        Console.WriteLine("Rewriting {0}", rc.Name);
        //        rc.SaveTo(targetFilename);
        //    }
        //}

        //[Test]
        //public void TestReplaceBitmapResource()
        //{
        //    Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
        //    string bitmap1filename = Path.Combine(Path.GetName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Bitmaps\\Bitmap1.ico");
        //    Assert.IsTrue(File.Exists(bitmap1filename));
        //    BitmapFile bitmapFile = new BitmapFile(bitmap1filename);

        //    Console.WriteLine("{0}: {1}", Path.GetFileName(bitmap1filename), bitmapFile.Type);
        //    foreach (BitmapFileBitmap bitmap in bitmapFile.Bitmaps)
        //    {
        //        Console.WriteLine(" {0}", bitmap.ToString());
        //    }

        //    Console.WriteLine("Converted BitmapResource:");
        //    BitmapResource bitmapResource = new BitmapResource(bitmapFile);
        //    bitmapResource.Language = ResourceUtil.USENGLISHLANGID;
        //    DumpResource.Dump(bitmapResource);
        //    Assert.AreEqual(bitmapFile.Bitmaps.Count, bitmapResource.Bitmaps.Count);

        //    string filename = Path.Combine(Environment.System, "write.exe");
        //    Assert.IsTrue(File.Exists(filename));
        //    string targetFilename = Path.Combine(Path.GetTempPath(), "write.exe");
        //    File.Copy(filename, targetFilename, true);
        //    Console.WriteLine(targetFilename);
        //    bitmapResource.SaveTo(targetFilename);

        //    Console.WriteLine("Written BitmapResource:");
        //    BitmapResource newBitmapResource = new BitmapResource();
        //    newBitmapResource.LoadFrom(targetFilename);
        //    DumpResource.Dump(newBitmapResource);
        //}
    }
}
