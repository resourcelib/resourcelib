using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Vestris.ResourceLib;

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
                            Dictionary<string, Resource>.Enumerator stringTableResourceEnumerator = versionResource.Resources.GetEnumerator();
                            while (stringTableResourceEnumerator.MoveNext())
                            {
                                DumpResource(stringTableResourceEnumerator.Current.Value);
                            }
                        }
                    }
                }

                //VersionResource vr = (VersionResource) vi.Resources["16"][0];
                //Console.WriteLine("Version: {0}", vr.FileVersion);
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
            Dictionary<string, Resource>.Enumerator resourceEnumerator = versionResource.Resources.GetEnumerator();
            while (resourceEnumerator.MoveNext())
            {
                DumpResource(resourceEnumerator.Current.Value);
            }
        }

        private void DumpResource(Resource rc)
        {
            if (rc is StringTableResource)
                DumpResource(rc as StringTableResource);
            else if (rc is VariableTableResource)
                DumpResource(rc as VariableTableResource);
        }

        private void DumpResource(StringTableResource stringTableResource)
        {
            Console.WriteLine("StringTableResource: {0}", stringTableResource.BlockKey);
            Dictionary<string, string>.Enumerator stringEnumerator = stringTableResource.Strings.GetEnumerator();
            while (stringEnumerator.MoveNext())
            {
                Console.WriteLine(" {0} = {1}",
                    stringEnumerator.Current.Key,
                    stringEnumerator.Current.Value);
            }
        }

        private void DumpResource(VariableTableResource varTableResource)
        {
            Console.WriteLine("VariableTableResource: {0}", varTableResource.BlockKey);
            Dictionary<UInt16, UInt16>.Enumerator langEnumerator = varTableResource.Languages.GetEnumerator();
            while (langEnumerator.MoveNext())
            {
                Console.WriteLine(" 0x{0:X} => {1}", langEnumerator.Current.Key, langEnumerator.Current.Value);
            }
        }
    }
}
