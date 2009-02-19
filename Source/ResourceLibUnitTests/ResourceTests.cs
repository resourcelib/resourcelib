using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Vestris.ResourceLib;
using System.Reflection;
using System.Web;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class VersionInfoTests
    {
        [Test]
        public void TestLoad()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
            Assert.IsTrue(File.Exists(filename));
            using (ResourceInfo vi = new ResourceInfo())
            {
                vi.Load(filename);

                foreach (string type in vi.ResourceTypes)
                {
                    Console.WriteLine(type);
                    foreach (Resource resource in vi.Resources[type])
                    {
                        Console.WriteLine(" {0} ({1}) - {2} byte(s)", resource.Name, resource.Language, resource.Size);

                        if (resource is VersionResource)
                        {
                            VersionResource versionResource = (VersionResource) resource;
                            Console.WriteLine("  {0}", versionResource.FileVersion);
                            Dictionary<string, ResourceTable>.Enumerator stringTableResourceEnumerator = versionResource.Resources.GetEnumerator();
                            while (stringTableResourceEnumerator.MoveNext())
                            {
                                Console.WriteLine("Dump of {0}:", stringTableResourceEnumerator.Current.Key);
                                DumpResource(stringTableResourceEnumerator.Current.Value);
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void TestLoadVersionResource()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
            Assert.IsTrue(File.Exists(filename));
            VersionResource versionResource = new VersionResource();
            versionResource.LoadFrom(filename);
            Console.WriteLine("File version: {0}", versionResource.FileVersion);
            Console.WriteLine("Product version: {0}", versionResource.ProductVersion);
            Dictionary<string, ResourceTable>.Enumerator resourceEnumerator = versionResource.Resources.GetEnumerator();
            while (resourceEnumerator.MoveNext())
            {
                DumpResource(resourceEnumerator.Current.Value);
            }
        }

        [Test]
        public void TestLoadVersionResourceStrings()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
            Assert.IsTrue(File.Exists(filename));
            VersionResource versionResource = new VersionResource();
            versionResource.LoadFrom(filename);
            Console.WriteLine("File version: {0}", versionResource.FileVersion);
            StringFileInfo stringFileInfo = (StringFileInfo) versionResource["StringFileInfo"];
            foreach (KeyValuePair<string, StringResource> stringResource in stringFileInfo.Default.Strings)
            {
                Console.WriteLine("{0} = {1}", stringResource.Value.Key, stringResource.Value.StringValue);
            }
        }

        private void DumpResource(ResourceTable rc)
        {
            if (rc is StringFileInfo)
                DumpResource(rc as StringFileInfo);
            else if (rc is VarFileInfo)
                DumpResource(rc as VarFileInfo);
            else if (rc is StringTable)
                DumpResource(rc as StringTable);
            else if (rc is VarTable)
                DumpResource(rc as VarTable);
        }

        private void DumpResource(StringFileInfo resource)
        {
            Console.WriteLine("StringFileInfo: {0}", resource.Key);
            Dictionary<string, StringTable>.Enumerator enumerator = resource.Strings.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DumpResource(enumerator.Current.Value);
            }
        }

        private void DumpResource(VarFileInfo resource)
        {
            Console.WriteLine("VarFileInfo: {0}", resource.Key);
            Dictionary<string, VarTable>.Enumerator enumerator = resource.Vars.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DumpResource(enumerator.Current.Value);
            }
        }

        private void DumpResource(StringTable stringTableResource)
        {
            Console.WriteLine("StringTableResource: {0}", stringTableResource.Key);
            Dictionary<string, StringResource>.Enumerator stringEnumerator = stringTableResource.Strings.GetEnumerator();
            while (stringEnumerator.MoveNext())
            {
                Console.WriteLine(" {0} = {1}",
                    stringEnumerator.Current.Key,
                    stringEnumerator.Current.Value.StringValue);
            }
        }

        private void DumpResource(VarTable varTableResource)
        {
            Console.WriteLine("VarTableResource: {0}", varTableResource.Key);
            Dictionary<UInt16, UInt16>.Enumerator langEnumerator = varTableResource.Languages.GetEnumerator();
            while (langEnumerator.MoveNext())
            {
                Console.WriteLine(" 0x{0:X} => {1}", langEnumerator.Current.Key, langEnumerator.Current.Value);
            }
        }

        [Test]
        public void TestLoadAndSaveVersionResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string filename = HttpUtility.UrlDecode(uri.AbsolutePath);
            Assert.IsTrue(File.Exists(filename));

            VersionResource versionResource = new VersionResource();
            versionResource.LoadFrom(filename);
            Console.WriteLine("File version: {0}", versionResource.FileVersion);
            Console.WriteLine("Product version: {0}", versionResource.ProductVersion);
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
            newVersionResource.LoadFrom(targetFilename);
            Console.WriteLine("File version: {0}", newVersionResource.FileVersion);
            Console.WriteLine("Product version: {0}", newVersionResource.ProductVersion);

            Assert.AreEqual(newVersionResource.FileVersion, versionResource.FileVersion);
            Assert.AreEqual(newVersionResource.ProductVersion, versionResource.ProductVersion);
            
            StringFileInfo newStringFileInfo = (StringFileInfo) newVersionResource["StringFileInfo"];
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
            versionResource.LoadFrom(filename);
            Console.WriteLine("File version: {0}", versionResource.FileVersion);

            byte[] currentBytes = VersionResource.LoadBytesFrom(filename);
            byte[] newBytes = versionResource.WriteAndGetBytes();

            Console.WriteLine("Current: {0}:{1}", currentBytes, currentBytes.Length);
            Console.WriteLine("New: {0}:{1}", newBytes, newBytes.Length);

            StringBuilder currentString = new StringBuilder();
            StringBuilder newString = new StringBuilder();

            for (int i = 0; i < Math.Min(currentBytes.Length, newBytes.Length); i++)
            {
                if (char.IsLetterOrDigit((char) newBytes[i]))
                    newString.Append((char) newBytes[i]);

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
