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
        private static readonly object[] TestAssemblies =
        {
            new object[] { "atl.dll", 1033 },
            new object[] { "ClassLibrary_NET2.0.dll", 0 },
            new object[] { "ClassLibrary_NET3.0.dll", 0 },
            new object[] { "ClassLibrary_NET3.5.dll", 0 },
            new object[] { "ClassLibrary_NET3.5ClientProfile.dll", 0 },
            new object[] { "ClassLibrary_NET4.0.dll", 0 },
            new object[] { "ClassLibrary_NET4.0ClientProfile.dll", 0 },
            new object[] { "ClassLibrary_NET4.5.dll", 0 },
            new object[] { "ClassLibrary_NET4.5.1.dll", 0 },
        };

        [TestCaseSource("TestAssemblies")]
        public void TestLoadVersionResource(string binaryName, int language)
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\" + binaryName);
            Assert.IsTrue(File.Exists(filename));
            VersionResource versionResource = new VersionResource();
            versionResource.Language = ResourceUtil.USENGLISHLANGID;
            versionResource.LoadFrom(filename);
            DumpResource.Dump(versionResource);
            AssertOneVersionResource(filename);
        }

        [TestCaseSource("TestAssemblies")]
        public void TestLoadVersionResourceStrings(string binaryName, int language)
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\" + binaryName);
            Assert.IsTrue(File.Exists(filename));
            VersionResource versionResource = new VersionResource();
            versionResource.Language = ResourceUtil.USENGLISHLANGID;
            versionResource.LoadFrom(filename);
            DumpResource.Dump(versionResource);
            AssertOneVersionResource(filename);
        }

        [TestCaseSource("TestAssemblies")]
        public void TestLoadAndSaveVersionResource(string binaryName, int language)
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\" + binaryName);
            Assert.IsTrue(File.Exists(filename));

            VersionResource versionResource = new VersionResource();
            versionResource.Language = (ushort)language;
            versionResource.LoadFrom(filename);
            DumpResource.Dump(versionResource);

            versionResource.FileVersion = "1.2.3.4";
            versionResource.ProductVersion = "5.6.7.8";
            versionResource.FileFlags = 0x2 | 0x8;  // private and prerelease

            StringFileInfo stringFileInfo = (StringFileInfo)versionResource["StringFileInfo"];
            stringFileInfo["Comments"] = string.Format("{0}\0", Guid.NewGuid());
            stringFileInfo["NewValue"] = string.Format("{0}\0", Guid.NewGuid());

            VarFileInfo varFileInfo = (VarFileInfo)versionResource["VarFileInfo"];
            varFileInfo[ResourceUtil.USENGLISHLANGID] = 1300;

            string targetFilename = Path.Combine(Path.GetTempPath(), "test.dll");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            versionResource.SaveTo(targetFilename);

            VersionResource newVersionResource = new VersionResource();
            newVersionResource.Language = (ushort)language;
            newVersionResource.LoadFrom(targetFilename);
            DumpResource.Dump(versionResource);

            AssertOneVersionResource(targetFilename);
            Assert.AreEqual(newVersionResource.FileVersion, versionResource.FileVersion);
            Assert.AreEqual(newVersionResource.ProductVersion, versionResource.ProductVersion);
            Assert.AreEqual(newVersionResource.FileFlags, versionResource.FileFlags);

            StringFileInfo testedStringFileInfo = (StringFileInfo)newVersionResource["StringFileInfo"];
            foreach (KeyValuePair<string, StringTableEntry> stringResource in testedStringFileInfo.Default.Strings)
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

        [TestCaseSource("TestAssemblies")]
        public void TestDeleteVersionResource(string binaryName, int language)
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\" + binaryName);
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "testDeleteVersionResource.dll");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            VersionResource versionResource = new VersionResource();
            versionResource.Language = (ushort)language;
            versionResource.LoadFrom(targetFilename);
            Console.WriteLine("Name: {0}", versionResource.Name);
            Console.WriteLine("Type: {0}", versionResource.Type);
            Console.WriteLine("Language: {0}", versionResource.Language);
            versionResource.DeleteFrom(targetFilename);
            try
            {
                versionResource.LoadFrom(targetFilename);
                Assert.Fail("Expected that the deleted resource cannot be found");
            }
            catch (Win32Exception ex)
            {
                // expected exception
                Console.WriteLine("Expected exception: {0}", ex.Message);
            }

            AssertNoVersionResource(targetFilename);

            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(targetFilename);
                DumpResource.Dump(ri);
            }
        }

        [TestCaseSource("TestAssemblies")]
        public void TestDeepCopyBytes(string binaryName, int language)
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\" + binaryName);
            Assert.IsTrue(File.Exists(filename));
            VersionResource existingVersionResource = new VersionResource();
            existingVersionResource.Language = (ushort)language;
            Console.WriteLine("Loading {0}", filename);
            existingVersionResource.LoadFrom(filename);
            DumpResource.Dump(existingVersionResource);

            VersionResource versionResource = new VersionResource();
            versionResource.FileVersion = existingVersionResource.FileVersion;
            versionResource.ProductVersion = existingVersionResource.ProductVersion;

            StringFileInfo existingVersionResourceStringFileInfo = (StringFileInfo)existingVersionResource["StringFileInfo"];
            VarFileInfo existingVersionResourceVarFileInfo = (VarFileInfo)existingVersionResource["VarFileInfo"];

            // copy string resources, data only
            StringFileInfo stringFileInfo = new StringFileInfo();
            {
                Dictionary<string, StringTable>.Enumerator enumerator = existingVersionResourceStringFileInfo.Strings.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    StringTable stringTable = new StringTable(enumerator.Current.Key);
                    stringFileInfo.Strings.Add(enumerator.Current.Key, stringTable);
                    Dictionary<string, StringTableEntry>.Enumerator resourceEnumerator = enumerator.Current.Value.Strings.GetEnumerator();
                    while (resourceEnumerator.MoveNext())
                    {
                        StringTableEntry stringResource = new StringTableEntry(resourceEnumerator.Current.Key);
                        stringResource.Value = resourceEnumerator.Current.Value.Value;
                        stringTable.Strings.Add(resourceEnumerator.Current.Key, stringResource);
                    }
                }
            }

            // copy var resources, data only
            VarFileInfo varFileInfo = new VarFileInfo();
            {
                Dictionary<string, VarTable>.Enumerator enumerator = existingVersionResourceVarFileInfo.Vars.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    VarTable varTable = new VarTable(enumerator.Current.Key);
                    varFileInfo.Vars.Add(enumerator.Current.Key, varTable);
                    Dictionary<UInt16, UInt16>.Enumerator translationEnumerator = enumerator.Current.Value.Languages.GetEnumerator();
                    while (translationEnumerator.MoveNext())
                    {
                        varTable.Languages.Add(translationEnumerator.Current.Key, translationEnumerator.Current.Value);
                    }
                }
            }

            bool firstResourceIsStringFileInfo = existingVersionResource[0] == existingVersionResourceStringFileInfo;
            if (firstResourceIsStringFileInfo)
            {
                versionResource["StringFileInfo"] = stringFileInfo;
                versionResource["VarFileInfo"] = varFileInfo;
            }
            else
            {
                versionResource["VarFileInfo"] = varFileInfo;
                versionResource["StringFileInfo"] = stringFileInfo;
            }

            ByteUtils.CompareBytes(existingVersionResource.WriteAndGetBytes(), versionResource.WriteAndGetBytes());
        }

        [TestCaseSource("TestAssemblies")]
        public void TestDeleteDeepCopyAndSaveVersionResource(string binaryName, int language)
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\" + binaryName);
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), binaryName);
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            VersionResource existingVersionResource = new VersionResource();
            existingVersionResource.Language = (ushort)language;
            existingVersionResource.LoadFrom(targetFilename);
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
                    Dictionary<string, StringTableEntry>.Enumerator resourceEnumerator = enumerator.Current.Value.Strings.GetEnumerator();
                    while (resourceEnumerator.MoveNext())
                    {
                        StringTableEntry stringResource = new StringTableEntry(resourceEnumerator.Current.Key);
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
                    Dictionary<UInt16, UInt16>.Enumerator translationEnumerator = enumerator.Current.Value.Languages.GetEnumerator();
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

            AssertOneVersionResource(targetFilename);
            Assert.AreEqual(newVersionResource.FileVersion, versionResource.FileVersion);
            Assert.AreEqual(newVersionResource.ProductVersion, versionResource.ProductVersion);

            StringFileInfo testedStringFileInfo = (StringFileInfo)newVersionResource["StringFileInfo"];
            foreach (KeyValuePair<string, StringTableEntry> stringResource in testedStringFileInfo.Default.Strings)
            {
                Assert.AreEqual(stringResource.Value.Value, stringFileInfo[stringResource.Key]);
            }

            VarFileInfo newVarFileInfo = (VarFileInfo)newVersionResource["VarFileInfo"];
            foreach (KeyValuePair<UInt16, UInt16> varResource in newVarFileInfo.Default.Languages)
            {
                Assert.AreEqual(varResource.Value, varFileInfo[varResource.Key]);
            }
        }

        [TestCaseSource("TestAssemblies")]
        public void TestDeleteAndSaveVersionResource(string binaryName, int language)
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\" + binaryName);
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "testDeleteAndSaveVersionResource.dll");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            VersionResource existingVersionResource = new VersionResource();
            existingVersionResource.Language = (ushort)language;
            existingVersionResource.DeleteFrom(targetFilename);

            VersionResource versionResource = new VersionResource();
            versionResource.FileVersion = "1.2.3.4";
            versionResource.ProductVersion = "4.5.6.7";

            StringFileInfo stringFileInfo = new StringFileInfo();
            versionResource[stringFileInfo.Key] = stringFileInfo;
            StringTable stringFileInfoStrings = new StringTable(); // "040904b0"
            stringFileInfoStrings.LanguageID = (ushort)language;
            stringFileInfoStrings.CodePage = 1200;
            Assert.AreEqual(language, stringFileInfoStrings.LanguageID);
            Assert.AreEqual(1200, stringFileInfoStrings.CodePage);
            stringFileInfo.Strings.Add(stringFileInfoStrings.Key, stringFileInfoStrings);
            stringFileInfoStrings["ProductName"] = "ResourceLib";
            stringFileInfoStrings["FileDescription"] = "File updated by ResourceLib\0";
            stringFileInfoStrings["CompanyName"] = "Vestris Inc.";
            stringFileInfoStrings["LegalCopyright"] = "All Rights Reserved\0";
            stringFileInfoStrings["EmptyValue"] = "";
            stringFileInfoStrings["Comments"] = string.Format("{0}\0", Guid.NewGuid());
            stringFileInfoStrings["ProductVersion"] = string.Format("{0}\0", versionResource.ProductVersion);

            VarFileInfo varFileInfo = new VarFileInfo();
            versionResource[varFileInfo.Key] = varFileInfo;
            VarTable varFileInfoTranslation = new VarTable("Translation");
            varFileInfo.Vars.Add(varFileInfoTranslation.Key, varFileInfoTranslation);
            varFileInfoTranslation[ResourceUtil.USENGLISHLANGID] = 1300;

            versionResource.SaveTo(targetFilename);
            Console.WriteLine("Reloading {0}", targetFilename);

            VersionResource newVersionResource = new VersionResource();
            newVersionResource.LoadFrom(targetFilename);
            DumpResource.Dump(newVersionResource);

            AssertOneVersionResource(targetFilename);
            Assert.AreEqual(newVersionResource.FileVersion, versionResource.FileVersion);
            Assert.AreEqual(newVersionResource.ProductVersion, versionResource.ProductVersion);
        }

        [Test]
        public void TestLoadNeutralDeleteEnglishResource()
        {
            // the 6to4svc.dll has an English version info strings resource that is loaded via netural
            VersionResource vr = new VersionResource();
            string testDll = Path.Combine(Path.GetTempPath(), "testLoadNeutralDeleteEnglishResource.dll");
            Console.WriteLine(testDll);
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string dll = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\6to4svc.dll");
            File.Copy(dll, testDll, true);
            vr.LoadFrom(testDll);
            Assert.AreEqual(1033, vr.Language);
            vr.DeleteFrom(testDll);
        }

        [Test]
        public void TestLoadDeleteGreekResource()
        {
            // the 6to4svcgreek.dll has a Greek version info strings resource
            VersionResource vr = new VersionResource();
            vr.Language = 1032;
            string testDll = Path.Combine(Path.GetTempPath(), "testLoadDeleteGreekResource.dll");
            Console.WriteLine(testDll);
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string dll = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\6to4svcgreek.dll");
            File.Copy(dll, testDll, true);
            vr.LoadFrom(testDll);
            DumpResource.Dump(vr);
            Assert.AreEqual(1032, vr.Language);
            vr.DeleteFrom(testDll);
        }

        [Test]
        public void TestDoubleNullTerminator()
        {
            StringTableEntry sr = new StringTableEntry("dummy");
            string guid = Guid.NewGuid().ToString();
            sr.Value = guid;
            Assert.AreEqual(guid + '\0', sr.Value);
            Assert.AreEqual(guid, sr.StringValue);
            Assert.AreEqual(guid.Length + 1, sr.Header.wValueLength);
            sr.Value = guid + '\0';
            Assert.AreEqual(guid + '\0', sr.Value);
            Assert.AreEqual(guid, sr.StringValue);
            Assert.AreEqual(guid.Length + 1, sr.Header.wValueLength);
            sr.Value = guid + "\0\0";
            Assert.AreEqual(guid + "\0\0", sr.Value);
            Assert.AreEqual(guid + '\0', sr.StringValue);
            Assert.AreEqual(guid.Length + 2, sr.Header.wValueLength);
            sr.Value = '\0' + guid;
            Assert.AreEqual('\0' + guid + '\0', sr.Value);
            Assert.AreEqual('\0' + guid, sr.StringValue);
            Assert.AreEqual(guid.Length + 2, sr.Header.wValueLength);
        }

        #region Helper methods

        private static void AssertOneVersionResource(string fileName)
        {
            Assert.AreEqual(
                1,
                GetOccurrencesOf("StringFileInfo", fileName),
                "More than one StringFileInfo block found.");

            Assert.AreEqual(
                1,
                GetOccurrencesOf("VarFileInfo", fileName),
                "More than one VarFileInfo block found.");
        }

        private static void AssertNoVersionResource(string fileName)
        {
            Assert.AreEqual(
                0,
                GetOccurrencesOf("StringFileInfo", fileName),
                "StringFileInfo block found.");

            Assert.AreEqual(
                0,
                GetOccurrencesOf("VarFileInfo", fileName),
                "VarFileInfo block found.");
        }

        private static int GetOccurrencesOf(string text, string fileName)
        {
            byte[] contentData = File.ReadAllBytes(fileName);

            var chars = new char[contentData.Length / sizeof(char)];
            Buffer.BlockCopy(contentData, 0, chars, 0, contentData.Length);
            var content = new string(chars);

            int occurrences = 0;
            int index = -1;

            while ((index = content.IndexOf(text, index + 1, StringComparison.InvariantCulture)) != -1)
            {
                occurrences++;
            }

            return occurrences;
        }

        #endregion
    }
}