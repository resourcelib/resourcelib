using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Vestris.ResourceLib;
using System.Reflection;
using System.Web;
using NUnit.Framework;
using System.ComponentModel;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class VersionResourceTests
    {
        [Test]
        public void TestLoadVersionResource()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
            Assert.IsTrue(File.Exists(filename));
            VersionResource versionResource = new VersionResource();
            versionResource.LoadFrom(filename, ResourceUtil.USENGLISHLANGID);
            DumpResource.Dump(versionResource);
        }

        [Test]
        public void TestLoadVersionResourceStrings()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
            Assert.IsTrue(File.Exists(filename));
            VersionResource versionResource = new VersionResource();
            versionResource.LoadFrom(filename, ResourceUtil.USENGLISHLANGID);
            DumpResource.Dump(versionResource);
        }

        [Test]
        public void TestLoadAndSaveVersionResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string filename = HttpUtility.UrlDecode(uri.AbsolutePath);
            Assert.IsTrue(File.Exists(filename));

            VersionResource versionResource = new VersionResource();
            versionResource.LoadFrom(filename, ResourceUtil.USENGLISHLANGID);
            DumpResource.Dump(versionResource); 

            versionResource.FileVersion = "1.2.3.4";
            versionResource.ProductVersion = "5.6.7.8";

            StringFileInfo stringFileInfo = (StringFileInfo)versionResource["StringFileInfo"];
            stringFileInfo["Comments"] = string.Format("{0}\0", Guid.NewGuid());
            stringFileInfo["NewValue"] = string.Format("{0}\0", Guid.NewGuid());

            VarFileInfo varFileInfo = (VarFileInfo)versionResource["VarFileInfo"];
            varFileInfo[0x409] = 1300;

            string targetFilename = Path.Combine(Path.GetTempPath(), "test.dll");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            versionResource.SaveTo(targetFilename);

            VersionResource newVersionResource = new VersionResource();
            newVersionResource.LoadFrom(targetFilename, ResourceUtil.USENGLISHLANGID);
            DumpResource.Dump(versionResource);

            Assert.AreEqual(newVersionResource.FileVersion, versionResource.FileVersion);
            Assert.AreEqual(newVersionResource.ProductVersion, versionResource.ProductVersion);

            StringFileInfo newStringFileInfo = (StringFileInfo)newVersionResource["StringFileInfo"];
            foreach (KeyValuePair<string, StringResource> stringResource in newStringFileInfo.Default.Strings)
            {
                Console.WriteLine("{0} = {1}", stringResource.Value.Key, stringResource.Value.StringValue);
                Assert.AreEqual(stringResource.Value.Value, stringFileInfo[stringResource.Key]);
            }

            VarFileInfo newVarFileInfo = (VarFileInfo)newVersionResource["VarFileInfo"];
            foreach (KeyValuePair<UInt16, UInt16> varResource in newVarFileInfo.Default.Languages)
            {
                Console.WriteLine("{0} = {1}", varResource.Key, varResource.Value);
                Assert.AreEqual(varResource.Value, varFileInfo[varResource.Key]);
            }
        }

        [Test]
        public void TestDeleteVersionResource()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "atl.dll");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            VersionResource versionResource = new VersionResource();
            versionResource.LoadFrom(targetFilename, ResourceUtil.USENGLISHLANGID);
            Console.WriteLine("Name: {0}", versionResource.Name);
            Console.WriteLine("Type: {0}", versionResource.Type);
            Console.WriteLine("Language: {0}", versionResource.Language);
            versionResource.DeleteFrom(targetFilename);
            try
            {
                versionResource.LoadFrom(targetFilename, ResourceUtil.USENGLISHLANGID);
                Assert.Fail("Expected that the deleted resource cannot be found");
            }
            catch (Win32Exception ex)
            {
                // expected exception
                Console.WriteLine("Expected exception: {0}", ex.Message);
            }
        }

        [Test]
        public void TestDeleteAndSaveVersionResource()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "atl.dll");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            VersionResource existingVersionResource = new VersionResource();
            existingVersionResource.LoadFrom(targetFilename, ResourceUtil.USENGLISHLANGID);
            DumpResource.Dump(existingVersionResource);
            existingVersionResource.DeleteFrom(targetFilename);

            VersionResource versionResource = new VersionResource();
            versionResource.FileVersion = existingVersionResource.FileVersion;
            versionResource.ProductVersion = existingVersionResource.ProductVersion;

            StringFileInfo stringFileInfo = new StringFileInfo();
            stringFileInfo["Comments"] = string.Format("{0}\0", Guid.NewGuid());
            stringFileInfo["NewValue"] = string.Format("{0}\0", Guid.NewGuid());
            versionResource["StringFileInfo"] = stringFileInfo;

            VarFileInfo varFileInfo = new VarFileInfo();
            varFileInfo[0x409] = 1200;
            versionResource["VarFileInfo"] = varFileInfo;

            versionResource.SaveTo(targetFilename);

            VersionResource newVersionResource = new VersionResource();
            newVersionResource.LoadFrom(targetFilename);
            DumpResource.Dump(newVersionResource);

            Assert.AreEqual(newVersionResource.FileVersion, versionResource.FileVersion);
            Assert.AreEqual(newVersionResource.ProductVersion, versionResource.ProductVersion);

            StringFileInfo newStringFileInfo = (StringFileInfo)newVersionResource["StringFileInfo"];
            foreach (KeyValuePair<string, StringResource> stringResource in newStringFileInfo.Default.Strings)
            {
                Assert.AreEqual(stringResource.Value.Value, stringFileInfo[stringResource.Key]);
            }

            VarFileInfo newVarFileInfo = (VarFileInfo)newVersionResource["VarFileInfo"];
            foreach (KeyValuePair<UInt16, UInt16> varResource in newVarFileInfo.Default.Languages)
            {
                Assert.AreEqual(varResource.Value, varFileInfo[varResource.Key]);
            }
        }

        [Test]
        public void TestLoadAndSaveOriginalVersionResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string filename = HttpUtility.UrlDecode(uri.AbsolutePath);
            Assert.IsTrue(File.Exists(filename));

            string targetFilename = Path.Combine(Path.GetTempPath(), "test.dll");
            Console.WriteLine(targetFilename);
            File.Copy(filename, targetFilename, true);

            byte[] data = VersionResource.LoadBytesFrom(filename);
            VersionResource.SaveTo(targetFilename, data);
        }

        [Test]
        public void TestVersionResourceBytes()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string filename = HttpUtility.UrlDecode(uri.AbsolutePath);
            Assert.IsTrue(File.Exists(filename));

            VersionResource versionResource = new VersionResource();
            versionResource.LoadFrom(filename, 
                ResourceUtil.USENGLISHLANGID);
            Console.WriteLine("File version: {0}", versionResource.FileVersion);

            byte[] currentBytes = VersionResource.LoadBytesFrom(filename, 
                ResourceUtil.USENGLISHLANGID);
            byte[] newBytes = versionResource.WriteAndGetBytes();

            Console.WriteLine("Current: {0}:{1}", currentBytes, currentBytes.Length);
            Console.WriteLine("New: {0}:{1}", newBytes, newBytes.Length);

            StringBuilder currentString = new StringBuilder();
            StringBuilder newString = new StringBuilder();

            for (int i = 0; i < Math.Min(currentBytes.Length, newBytes.Length); i++)
            {
                if (char.IsLetterOrDigit((char)newBytes[i]))
                    newString.Append((char)newBytes[i]);

                if (char.IsLetterOrDigit((char)currentBytes[i]))
                    currentString.Append((char)currentBytes[i]);

                if (currentBytes[i] != newBytes[i])
                {
                    Console.WriteLine(currentString.ToString());
                    Console.WriteLine(newString.ToString());
                }

                Assert.AreEqual(currentBytes[i], newBytes[i], string.Format("Error at offset {0}", i));
            }
        }
    }
}
