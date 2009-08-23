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
            varFileInfo[ResourceUtil.USENGLISHLANGID] = 1300;

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
        public void TestDeepCopyBytes()
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
                    Dictionary<UInt16, UInt16>.Enumerator translationEnumerator = enumerator.Current.Value.Languages.GetEnumerator();
                    while (translationEnumerator.MoveNext())
                    {
                        varTable.Languages.Add(translationEnumerator.Current.Key, translationEnumerator.Current.Value);
                    }
                }
            }

            ByteUtils.CompareBytes(existingVersionResource.WriteAndGetBytes(), versionResource.WriteAndGetBytes());
        }

        [Test]
        public void TestDeleteDeepCopyAndSaveVersionResource()
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
        public void TestDeleteAndSaveVersionResource()
        {
            string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "atl.dll");
            File.Copy(filename, targetFilename, true);
            Console.WriteLine(targetFilename);
            VersionResource existingVersionResource = new VersionResource();
            existingVersionResource.DeleteFrom(targetFilename);

            VersionResource versionResource = new VersionResource();
            versionResource.FileVersion = "1.2.3.4";
            versionResource.ProductVersion = "4.5.6.7";

            StringFileInfo stringFileInfo = new StringFileInfo();
            versionResource[stringFileInfo.Key] = stringFileInfo;
            StringTable stringFileInfoStrings = new StringTable(); // "040904b0"
            stringFileInfoStrings.LanguageID = 1033;
            stringFileInfoStrings.CodePage = 1200;
            Assert.AreEqual(1033, stringFileInfoStrings.LanguageID);
            Assert.AreEqual(1200, stringFileInfoStrings.CodePage);
            stringFileInfo.Strings.Add(stringFileInfoStrings.Key, stringFileInfoStrings);
            stringFileInfoStrings["ProductName"] = "ResourceLib\0";
            stringFileInfoStrings["FileDescription"] = "File updated by ResourceLib\0";
            stringFileInfoStrings["CompanyName"] = "Vestris Inc.\0";
            stringFileInfoStrings["LegalCopyright"] = "All Rights Reserved\0";
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

            Assert.AreEqual(newVersionResource.FileVersion, versionResource.FileVersion);
            Assert.AreEqual(newVersionResource.ProductVersion, versionResource.ProductVersion);
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

            ByteUtils.CompareBytes(expectedBytes, testedBytes);            
        }

        [Test]
        public void TestLoadNeutralDeleteEnglishResource()
        {
            // the 6to4svc.dll has an English version info strings resource that is loaded via netural            
            VersionResource vr = new VersionResource();
            string testDll = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".dll");
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string dll = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\6to4svc.dll");
            File.Copy(dll, testDll);
            try
            {
                vr.LoadFrom(testDll);
                Assert.AreEqual(1033, vr.Language);
                vr.DeleteFrom(testDll);
            }
            finally
            {
                File.Delete(testDll);
            }
        }
    }
}
