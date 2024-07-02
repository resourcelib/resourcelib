using System;
using System.Collections.Generic;
using System.Text;
using Vestris.ResourceLib;
using System.IO;
using System.Web;
using NUnit.Framework;
using System.Reflection;
using System.Drawing;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class StringResourceTests
    {
        [Test]
        public void TestLoadStringResources()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            string filename = Path.Combine(uriPath, @"Binaries\gutils.dll");
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                Assert.AreEqual(1, ri[Kernel32.ResourceTypes.RT_STRING].Count);
                foreach (StringResource rc in ri[Kernel32.ResourceTypes.RT_STRING])
                {
                    Console.WriteLine("StringResource: {0}, {1}", rc.Name, rc.TypeName);
                    DumpResource.Dump(rc);
                }
            }
        }

        [Test]
        public void TestLoadStringResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            string filename = Path.Combine(uriPath, @"Binaries\gutils.dll");
            StringResource sr = new StringResource();
            sr.Name = new ResourceId(StringResource.GetBlockId(402));
            sr.LoadFrom(filename);
            Assert.AreEqual("Out Of Memory", sr[402]);
        }

        [Test]
        public void TestSaveStringResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().Location);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            string sourceFilename = Path.Combine(uriPath, @"Binaries\gutils.dll");
            string targetFilename = Path.Combine(Path.GetTempPath(), "testSaveStringResource.dll");
            File.Copy(sourceFilename, targetFilename, true);
            // a new resource
            StringResource sr = new StringResource();
            sr.Name = new ResourceId(StringResource.GetBlockId(1256));
            sr[1256] = Guid.NewGuid().ToString();
            sr.SaveTo(targetFilename);
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(targetFilename);
                Assert.AreEqual(2, ri[Kernel32.ResourceTypes.RT_STRING].Count);
                Assert.AreEqual(StringResource.GetBlockId(1256), ri[Kernel32.ResourceTypes.RT_STRING][1].Name.Id.ToInt64());
                Assert.AreEqual(sr[1256], ((StringResource) ri[Kernel32.ResourceTypes.RT_STRING][1])[1256]);
                foreach (StringResource rc in ri[Kernel32.ResourceTypes.RT_STRING])
                {
                    Console.WriteLine("StringResource: {0}, {1}", rc.Name, rc.TypeName);
                    DumpResource.Dump(rc);
                }
            }
        }
    }
}
