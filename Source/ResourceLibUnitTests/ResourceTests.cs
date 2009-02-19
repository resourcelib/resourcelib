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
                            Dictionary<string, StringTableResource>.Enumerator stringTableResourceEnumerator = versionResource.StringTableResources.GetEnumerator();
                            while (stringTableResourceEnumerator.MoveNext())
                            {
                                StringTableResource stringTableResource = stringTableResourceEnumerator.Current.Value;
                                Console.WriteLine("  {0}", stringTableResource.BlockInfo.szKey);
                                Dictionary<string, string>.Enumerator stringEnumerator = stringTableResource.Strings.GetEnumerator();
                                while (stringEnumerator.MoveNext())
                                {
                                    Console.WriteLine("  {0} = {1}",
                                        stringEnumerator.Current.Key,
                                        stringEnumerator.Current.Value);
                                }
                            }
                        }
                    }
                }

                //VersionResource vr = (VersionResource) vi.Resources["16"][0];
                //Console.WriteLine("Version: {0}", vr.FileVersion);
            }
        }
    }
}
