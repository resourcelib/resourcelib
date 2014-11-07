using System;
using System.Collections.Generic;
using System.Text;
using Vestris.ResourceLib;
using System.IO;
using System.Web;
using NUnit.Framework;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class GenericResourceTests
    {
        [TestCase("atl.dll")]
        [TestCase("ConsoleApplication_NET4.5.exe")]
        public void TestLoadSave(string binaryName)
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string filename = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\" + binaryName);
            Assert.IsTrue(File.Exists(filename));
            string targetFilename = Path.Combine(Path.GetTempPath(), "genericResourceTestLoadSave.dll");
            Console.WriteLine(targetFilename);
            File.Copy(filename, targetFilename, true);
            // write the resource to a binary
            GenericResource genericResource = new GenericResource(
                new ResourceId("TESTTYPE"), new ResourceId("TESTNAME"), ResourceUtil.USENGLISHLANGID);
            genericResource.Data = Guid.NewGuid().ToByteArray();
            genericResource.SaveTo(targetFilename);
            // reload and compare
            GenericResource newGenericResource = new GenericResource(
                new ResourceId("TESTTYPE"), new ResourceId("TESTNAME"), ResourceUtil.USENGLISHLANGID);
            newGenericResource.LoadFrom(targetFilename);
            ByteUtils.CompareBytes(newGenericResource.Data, genericResource.Data);
        }
    }
}
