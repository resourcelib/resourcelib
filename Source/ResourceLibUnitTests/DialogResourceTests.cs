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

        [Test]
        public void TestLoadDialogResource()
        {
            {
                string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll");
                DialogResource rc = new DialogResource();
                rc.Name = new ResourceId("STRINGINPUT");
                rc.LoadFrom(filename);
                Console.WriteLine("DialogResource: {0}, {1}", rc.Name, rc.TypeName);
                Console.WriteLine(rc);
            }

            {
                string filename = Path.Combine(Environment.SystemDirectory, "audiogenie2.dll");
                DialogResource rc = new DialogResource();
                rc.Name = new ResourceId(103);
                rc.LoadFrom(filename);
                Console.WriteLine("DialogResource: {0}, {1}", rc.Name, rc.TypeName);
                Console.WriteLine(rc);
            }
        }
    }
}
