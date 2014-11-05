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
        [Test]
        public void TestLoadSave()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string atldll = Path.Combine(Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath)), "Binaries\\atl.dll");
            Assert.IsTrue(File.Exists(atldll));
            string targetFilename = Path.Combine(Path.GetTempPath(), "genericResourceTestLoadSave.dll");
            Console.WriteLine(targetFilename);
            File.Copy(atldll, targetFilename, true);
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
