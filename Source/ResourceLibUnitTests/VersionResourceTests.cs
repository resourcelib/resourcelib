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

            StringFileInfo testedStringFileInfo = (StringFileInfo)newVersionResource["StringFileInfo"];
            foreach (KeyValuePair<string, StringResource> stringResource in testedStringFileInfo.Default.Strings)
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
        public void TestVersionConstructorBytes()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
            Assert.IsTrue(File.Exists(filename));
            VersionResource existingVersionResource = new VersionResource();
            Console.WriteLine("Loading {0}", filename);
            existingVersionResource.LoadFrom(filename, ResourceUtil.USENGLISHLANGID);
            DumpResource.Dump(existingVersionResource);

            VersionResource versionResource = new VersionResource();
            versionResource.FileVersion = existingVersionResource.FileVersion;
            versionResource.ProductVersion = existingVersionResource.ProductVersion;

            StringFileInfo existingVersionResourceStringFileInfo = (StringFileInfo)existingVersionResource["StringFileInfo"];
            VarFileInfo existingVersionResourceVarFileInfo = (VarFileInfo)existingVersionResource["VarFileInfo"];

            // copy string resources, data only
            StringFileInfo stringFileInfo = new StringFileInfo();
            versionResource["StringFileInfo"] = stringFileInfo;
            {
                Dictionary<string, StringTable>.Enumerator enumerator = existingVersionResourceStringFileInfo.Strings.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    StringTable stringTable = new StringTable(enumerator.Current.Key);
                    stringFileInfo.Strings.Add(enumerator.Current.Key, stringTable);
                    Dictionary<string, StringResource>.Enumerator resourceEnumerator = enumerator.Current.Value.Strings.GetEnumerator();
                    while (resourceEnumerator.MoveNext())
                    {
                        StringResource stringResource = new StringResource(resourceEnumerator.Current.Key);
                        stringResource.Value = resourceEnumerator.Current.Value.Value;
                        stringTable.Strings.Add(resourceEnumerator.Current.Key, stringResource);
                    }
                }
            }

            // copy var resources, data only
            VarFileInfo varFileInfo = new VarFileInfo();
            versionResource["VarFileInfo"] = varFileInfo;
            {
                Dictionary<string, VarTable>.Enumerator enumerator = existingVersionResourceVarFileInfo.Vars.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    VarTable varTable = new VarTable(enumerator.Current.Key);
                    varFileInfo.Vars.Add(enumerator.Current.Key, varTable);
                    Dictionary<ushort, ushort>.Enumerator translationEnumerator = enumerator.Current.Value.Languages.GetEnumerator();
                    while (translationEnumerator.MoveNext())
                    {
                        varTable.Languages.Add(translationEnumerator.Current.Key, translationEnumerator.Current.Value);
                    }
                }
            }

            CompareBytes(existingVersionResource.WriteAndGetBytes(), versionResource.WriteAndGetBytes());
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

            StringFileInfo existingVersionResourceStringFileInfo = (StringFileInfo)existingVersionResource["StringFileInfo"];
            VarFileInfo existingVersionResourceVarFileInfo = (VarFileInfo)existingVersionResource["VarFileInfo"];

            // copy string resources, data only
            StringFileInfo stringFileInfo = new StringFileInfo();
            versionResource["StringFileInfo"] = stringFileInfo;
            {
                Dictionary<string, StringTable>.Enumerator enumerator = existingVersionResourceStringFileInfo.Strings.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    StringTable stringTable = new StringTable(enumerator.Current.Key);
                    stringFileInfo.Strings.Add(enumerator.Current.Key, stringTable);
                    Dictionary<string, StringResource>.Enumerator resourceEnumerator = enumerator.Current.Value.Strings.GetEnumerator();
                    while (resourceEnumerator.MoveNext())
                    {
                        StringResource stringResource = new StringResource(resourceEnumerator.Current.Key);
                        stringResource.Value = resourceEnumerator.Current.Value.Value;
                        stringTable.Strings.Add(resourceEnumerator.Current.Key, stringResource);
                    }
                }
            }

            // copy var resources, data only
            VarFileInfo varFileInfo = new VarFileInfo();
            versionResource["VarFileInfo"] = varFileInfo;
            {
                Dictionary<string, VarTable>.Enumerator enumerator = existingVersionResourceVarFileInfo.Vars.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    VarTable varTable = new VarTable(enumerator.Current.Key);
                    varFileInfo.Vars.Add(enumerator.Current.Key, varTable);
                    Dictionary<ushort, ushort>.Enumerator translationEnumerator = enumerator.Current.Value.Languages.GetEnumerator();
                    while (translationEnumerator.MoveNext())
                    {
                        varTable.Languages.Add(translationEnumerator.Current.Key, translationEnumerator.Current.Value);
                    }
                }
            }

            versionResource.SaveTo(targetFilename);
            Console.WriteLine("Reloading {0}", targetFilename);

            VersionResource newVersionResource = new VersionResource();
            newVersionResource.LoadFrom(targetFilename);
            DumpResource.Dump(newVersionResource);

            Assert.AreEqual(newVersionResource.FileVersion, versionResource.FileVersion);
            Assert.AreEqual(newVersionResource.ProductVersion, versionResource.ProductVersion);

            StringFileInfo testedStringFileInfo = (StringFileInfo)newVersionResource["StringFileInfo"];
            foreach (KeyValuePair<string, StringResource> stringResource in testedStringFileInfo.Default.Strings)
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

            byte[] expectedBytes = VersionResource.LoadBytesFrom(filename, 
                ResourceUtil.USENGLISHLANGID);
            byte[] testedBytes = versionResource.WriteAndGetBytes();

            CompareBytes(expectedBytes, testedBytes);            
        }

        private void CompareBytes(byte[] expectedBytes, byte[] testedBytes)
        {
            Console.WriteLine("Expected: {0}:{1}", expectedBytes, expectedBytes.Length);
            Console.WriteLine("Tested: {0}:{1}", testedBytes, testedBytes.Length);

            StringBuilder expectedString = new StringBuilder();
            StringBuilder testedString = new StringBuilder();

            int errors = 0;
            for (int i = 0; i < Math.Min(expectedBytes.Length, testedBytes.Length); i++)
            {
                if (char.IsLetterOrDigit((char)testedBytes[i]))
                    testedString.Append((char)testedBytes[i]);
                else if (testedBytes[i] != 0)
                    testedString.AppendFormat("[{0}]", (int)testedBytes[i]);

                if (char.IsLetterOrDigit((char)expectedBytes[i]))
                    expectedString.Append((char)expectedBytes[i]);
                else if (expectedBytes[i] != 0)
                    expectedString.AppendFormat("[{0}]", (int)expectedBytes[i]);

                if (expectedBytes[i] != testedBytes[i])
                {
                    Console.WriteLine(expectedString.ToString());
                    Console.WriteLine(testedString.ToString());
                }

                if (expectedBytes[i] != testedBytes[i])
                {
                    Console.WriteLine(string.Format("Error at offset {0}, expected {1} got {2}",
                        i, expectedBytes[i], testedBytes[i]));
                    errors++;
                }
            }

            Assert.IsTrue(errors == 0, "Errors in binary comparisons.");
        }
    }
}
