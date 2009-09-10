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
    public class MenuResourceTests
    {
        [Test]
        public void TestLoadMenuResources()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            string filename = Path.Combine(uriPath, @"Binaries\custom.exe");
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                Assert.AreEqual(2, ri[Kernel32.ResourceTypes.RT_MENU].Count);
                foreach (MenuResource rc in ri[Kernel32.ResourceTypes.RT_MENU])
                {
                    Console.WriteLine("MenuResource: {0}, {1}", rc.Name, rc.TypeName);
                    DumpResource.Dump(rc);
                }
                Assert.AreEqual(101, ri[Kernel32.ResourceTypes.RT_MENU][0].Name.Id.ToInt32());
                Assert.AreEqual(102, ri[Kernel32.ResourceTypes.RT_MENU][1].Name.Id.ToInt32());
            }
        }

        [Test]
        public void TestLoadMenuResourcesEx()
        {
            string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                foreach (MenuResource rc in ri[Kernel32.ResourceTypes.RT_MENU])
                {
                    Console.WriteLine("MenuResource: {0}, {1}", rc.Name, rc.TypeName);
                    DumpResource.Dump(rc);
                }
            }
        }

        //[Test]
        //public void TestLoadMixedMenuResources()
        //{
        //    Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
        //    string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
        //    string filename = Path.Combine(uriPath, @"C:\Windows\explorer.exe");
        //    using (ResourceInfo ri = new ResourceInfo())
        //    {
        //        ri.Load(filename);
        //        foreach (MenuResource rc in ri[Kernel32.ResourceTypes.RT_MENU])
        //        {
        //            Console.WriteLine("MenuResource: {0}, {1}", rc.Name, rc.TypeName);
        //            DumpResource.Dump(rc);
        //        }
        //    }
        //}
    }
}
