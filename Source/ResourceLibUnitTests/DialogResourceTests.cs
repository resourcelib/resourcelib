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
        private struct TestLoadDialogResourceTestDataEntry
        {
            private string _filename;
            private ResourceId _resourceId;
            private int _numberOfDialogControls;

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

            public int NumberOfDialogControls
            {
                get
                {
                    return _numberOfDialogControls;
                }
            }

            public TestLoadDialogResourceTestDataEntry(
                string filename, ResourceId resourceId, int numberOfDialogControls)
            {
                _filename = filename;
                _resourceId = resourceId;
                _numberOfDialogControls = numberOfDialogControls;
            }
        }

        private List<TestLoadDialogResourceTestDataEntry> testdata = new List<TestLoadDialogResourceTestDataEntry>();

        public DialogResourceTests()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));

            testdata.Add(
                new TestLoadDialogResourceTestDataEntry(
                    Path.Combine(uriPath, @"Binaries\gutils.dll"),
                    new ResourceId("GABRTDLG"), 3));
            testdata.Add(
                new TestLoadDialogResourceTestDataEntry(
                    Path.Combine(uriPath, @"Binaries\gutils.dll"),
                    new ResourceId("STRINGINPUT"),
                    4));
            testdata.Add(
                new TestLoadDialogResourceTestDataEntry(
                    Path.Combine(uriPath, "Binaries\\acppage.dll"),
                    new ResourceId(101),
                    12));
            testdata.Add(
                new TestLoadDialogResourceTestDataEntry(
                    Path.Combine(uriPath, "Binaries\\acppage.dll"),
                    new ResourceId(5011),
                    13));
        }

        [Test]
        public void TestLoadDialogResources()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            string filename = Path.Combine(uriPath, @"Binaries\gutils.dll");
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                foreach (DialogResource rc in ri[Kernel32.ResourceTypes.RT_DIALOG])
                {
                    Console.WriteLine("DialogResource: {0}, {1}", rc.Name, rc.TypeName);
                    Console.WriteLine(rc);
                }
                // two dialogs
                Assert.AreEqual(2, ri[Kernel32.ResourceTypes.RT_DIALOG].Count);
                DialogResource dlg = (DialogResource) ri[Kernel32.ResourceTypes.RT_DIALOG][0];
                Assert.AreEqual("GABRTDLG", dlg.Name.Name);
                // the first one is called "Printing"
                DialogTemplate printingTemplate = (DialogTemplate) dlg.Template;
                Assert.AreEqual(38, printingTemplate.x);
                Assert.AreEqual(18, printingTemplate.y);
                Assert.AreEqual(128, printingTemplate.cx);
                Assert.AreEqual(83, printingTemplate.cy);
                Assert.AreEqual(3, printingTemplate.ControlCount);
                Assert.AreEqual(3, printingTemplate.Controls.Count);
                Assert.AreEqual("Printing", printingTemplate.Caption);
                Assert.AreEqual("MS Shell Dlg", printingTemplate.TypeFace);
                Assert.AreEqual(8, printingTemplate.PointSize);
                // the first control is called "Printing Table"
                DialogTemplateControl printingTable = (DialogTemplateControl) printingTemplate.Controls[0];
                Assert.AreEqual("Printing Table", printingTable.CaptionId.Name);
                Assert.AreEqual(23, printingTable.x);
                Assert.AreEqual(18, printingTable.y);
                Assert.AreEqual(87, printingTable.cx);
                Assert.AreEqual(8, printingTable.cy);
                Assert.AreEqual(101, printingTable.Id);
            }
        }

        [Test]
        public void TestLoadDialogResource()
        {
            foreach (TestLoadDialogResourceTestDataEntry test in testdata)
            {
                Console.WriteLine("{0}: {1}", test.Filename, test.ResourceId);
                DialogResource rc = new DialogResource();
                rc.Name = test.ResourceId;
                rc.LoadFrom(test.Filename);
                Console.WriteLine("DialogResource: {0}, {1}", rc.Name, rc.TypeName);
                Console.WriteLine(rc);
                Assert.AreEqual(test.NumberOfDialogControls, rc.Template.ControlCount);
                Assert.AreEqual(test.NumberOfDialogControls, rc.Template.Controls.Count);
            }
        }

        [Test]
        public void TestCompareDialogResourceBytes()
        {
            foreach (TestLoadDialogResourceTestDataEntry test in testdata)
            {
                Console.WriteLine(test.Filename);
                DialogResource sourceDialog = new DialogResource();
                GenericResource genericResource = new GenericResource(
                    new ResourceId(Kernel32.ResourceTypes.RT_DIALOG),
                    test.ResourceId,
                    ResourceUtil.USENGLISHLANGID);
                genericResource.LoadFrom(test.Filename);
                sourceDialog.Name = test.ResourceId;
                sourceDialog.LoadFrom(test.Filename);
                byte[] data = sourceDialog.WriteAndGetBytes();
                ByteUtils.CompareBytes(genericResource.Data, data);
            }
        }

        [Test]
        public void TestAddDialogResource()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            string gutilsdll = Path.Combine(uriPath, @"Binaries\gutils.dll");
            int dialogsBefore = 0;
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(gutilsdll);
                dialogsBefore = ri.Resources[new ResourceId(Kernel32.ResourceTypes.RT_DIALOG)].Count;
            }
            string targetFilename = Path.Combine(Path.GetTempPath(), "testAddDialogResource.dll");
            File.Copy(gutilsdll, targetFilename, true);
            // copy an existing dialog inside gutils.dll
            DialogResource sourceDialog = new DialogResource();
            sourceDialog.Name = new ResourceId("GABRTDLG");
            sourceDialog.LoadFrom(gutilsdll);
            sourceDialog.Name = new ResourceId("NEWGABRTDLG");            
            Console.WriteLine(targetFilename);
            sourceDialog.SaveTo(targetFilename);
            // check that the dialog was written
            sourceDialog.LoadFrom(targetFilename);
            DumpResource.Dump(sourceDialog);
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(targetFilename);
                int dialogsAfter = ri.Resources[new ResourceId(Kernel32.ResourceTypes.RT_DIALOG)].Count;
                DumpResource.Dump(ri);
                Assert.AreEqual(dialogsBefore + 1, dialogsAfter);
            }
        }
    }
}
