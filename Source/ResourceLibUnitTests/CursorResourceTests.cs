using System;
using System.Collections.Generic;
using System.Text;
using Vestris.ResourceLib;
using System.IO;
using System.Web;
using NUnit.Framework;
using System.Reflection;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class CursorResourceTests
    {
        [Test]
        public void TestLoadCursorResources()
        {
            string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll");
            Assert.IsTrue(File.Exists(filename));
            CursorDirectoryResource cursorDirectoryResource = new CursorDirectoryResource();
            cursorDirectoryResource.Name = new ResourceId("HORZLINE");
            cursorDirectoryResource.LoadFrom(filename);
            DumpResource.Dump(cursorDirectoryResource);
        }
    }
}