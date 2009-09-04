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
    public class DialogResourceTests
    {
        [Test]
        public void TestLoadDialogResources()
        {
            string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll");
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                foreach (DialogResource rc in ri[Kernel32.ResourceTypes.RT_DIALOG])
                {
                    Console.WriteLine("DialogResource: {0}, {1}", rc.Name, rc.TypeName);
                    Console.WriteLine(rc);
                }
            }
        }

        private struct TestLoadDialogResourceTestDataEntry
        {
            private string _filename;
            private ResourceId _resourceId;

            public string Filename
            {
                get
                {
                    return _filename;
                }
            }

            public ResourceId ResourceId
            {
                get
                {
                    return _resourceId;
                }
            }

            public TestLoadDialogResourceTestDataEntry(string filename, ResourceId resourceId)
            {
                _filename = filename;
                _resourceId = resourceId;
            }
        }
        
        [Test]
        public void TestLoadDialogResource()
        {
            TestLoadDialogResourceTestDataEntry[] testdata = 
            {
                new TestLoadDialogResourceTestDataEntry(Path.Combine(
                    Environment.GetEnvironmentVariable("WINDIR"), "hteweb.dll"), 
                    new ResourceId(212)),
                new TestLoadDialogResourceTestDataEntry(Path.Combine(
                    Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll"), 
                    new ResourceId("GABRTDLG")),
                new TestLoadDialogResourceTestDataEntry(
                    Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll"), 
                    new ResourceId("STRINGINPUT")),
                new TestLoadDialogResourceTestDataEntry(
                    Path.Combine(Environment.SystemDirectory, "audiogenie2.dll"), 
                    new ResourceId(103)),
            };

            foreach (TestLoadDialogResourceTestDataEntry test in testdata)
            {
                Console.WriteLine("{0}: {1}", test.Filename, test.ResourceId);
                DialogResource rc = new DialogResource();
                rc.Name = test.ResourceId;
                rc.LoadFrom(test.Filename);
                Console.WriteLine("DialogResource: {0}, {1}", rc.Name, rc.TypeName);
                Console.WriteLine(rc);
            }
        }
    }
}
