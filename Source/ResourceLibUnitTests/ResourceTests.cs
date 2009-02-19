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
            VersionResource versionResource = new VersionResource(filename);
            Console.WriteLine("File version: {0}", versionResource.FileVersion);
            Console.WriteLine("Product version: {0}", versionResource.ProductVersion);
            Dictionary<string, ResourceTable>.Enumerator resourceEnumerator = versionResource.Resources.GetEnumerator();
            while (resourceEnumerator.MoveNext())
            {
                DumpResource(resourceEnumerator.Current.Value);
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

            VersionResource versionResource = new VersionResource(filename);
            Console.WriteLine("File version: {0}", versionResource.FileVersion);
            Console.WriteLine("Product version: {0}", versionResource.ProductVersion);
            versionResource.FileVersion = "1.2.3.4";

            string targetFilename = Path.Combine(Path.GetTempPath(), "test.dll");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            versionResource.Save(targetFilename);

            VersionResource newVersionResource = new VersionResource(targetFilename);
            Console.WriteLine("File version: {0}", newVersionResource.FileVersion);
            Console.WriteLine("Product version: {0}", newVersionResource.ProductVersion);
        }

        [Test]
        public void TestVersionResourceBytes()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string filename = HttpUtility.UrlDecode(uri.AbsolutePath);
            Assert.IsTrue(File.Exists(filename));

            VersionResource versionResource = new VersionResource(filename);
            Console.WriteLine("File version: {0}", versionResource.FileVersion);

            byte[] currentBytes = versionResource.ReadBytes; // ResourceUtil.GetBytes<Kernel32.VS_VERSIONINFO>(versionResource.VersionInfo);
            byte[] newBytes = versionResource.GetBytes();

            Console.WriteLine("Current: {0}:{1}", currentBytes, currentBytes.Length);
            Console.WriteLine("New: {0}:{1}", newBytes, newBytes.Length);

            // compare the first VS_VERSIONINFO bytes
            // \todo skipping two bytes, size isn't the same until all fields are written

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

            // \todo: comapre byte-to-byte
        }

    }
}
