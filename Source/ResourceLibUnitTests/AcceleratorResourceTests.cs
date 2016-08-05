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
    public class AcceleratorResourceTests
    {
        [Test]
        public void TestLoadAcceleratorResources()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string uriPath = Path.GetDirectoryName(HttpUtility.UrlDecode(uri.AbsolutePath));
            string filename = Path.Combine(uriPath, @"Binaries\custom.exe");
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                Assert.AreEqual(2, ri[Kernel32.ResourceTypes.RT_ACCELERATOR].Count);
                foreach (AcceleratorResource rc in ri[Kernel32.ResourceTypes.RT_ACCELERATOR])
                {
                    Console.WriteLine("AcceleratorResource: {0}, {1}", rc.Name, rc.TypeName);
                    DumpResource.Dump(rc);
                }
                Assert.AreEqual(109, ri[Kernel32.ResourceTypes.RT_ACCELERATOR][0].Name.Id.ToInt64());
                Assert.AreEqual(110, ri[Kernel32.ResourceTypes.RT_ACCELERATOR][1].Name.Id.ToInt64());
            }
        }

        [Test]
        public void TestAccelerator()
        {
            string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");

            Predicate<Accelerator> pReload = (Accelerator a) => { return a.Key == "VK_F5" && a.Command == 41061; };
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                foreach (AcceleratorResource rc in ri[Kernel32.ResourceTypes.RT_ACCELERATOR])
                {
                    Console.WriteLine("Current AcceleratorResource: {0}, {1}", rc.Name, rc.TypeName);
                    Console.WriteLine(rc + "\n------------------------------------\n");

                    Accelerator newAC = new Accelerator();
                    newAC.Key = "J";
                    newAC.Command = 41008;
                    newAC.Flags = User32.AcceleratorVirtualKey.VIRTKEY | User32.AcceleratorVirtualKey.NOINVERT | User32.AcceleratorVirtualKey.CONTROL;

                    try
                    {
                        Accelerator tst = rc.Accelerators.Find(pReload);
                        int loc = rc.Accelerators.FindIndex(pReload);
                        rc.Accelerators.Remove(tst);

                        tst.Key = "VK_NUMPAD9";
                        tst.addFlag(User32.AcceleratorVirtualKey.CONTROL);
                        tst.removeFlag(User32.AcceleratorVirtualKey.ALT | User32.AcceleratorVirtualKey.NOINVERT);
                        rc.Accelerators.Insert(loc, tst);
                        rc.Accelerators.Add(newAC);

                        List<Accelerator> acList = new List<Accelerator>();

                        Accelerator acA = new Accelerator();
                        acA.Command = 413;
                        acList.Add(acA);
                        acList[0].addFlag(User32.AcceleratorVirtualKey.SHIFT);
                        acList[0].Key = "VK_F1";

                        Accelerator acB = new Accelerator();
                        acB.Flags = User32.AcceleratorVirtualKey.VIRTKEY | User32.AcceleratorVirtualKey.NOINVERT;
                        acB.Key = "Z";
                        acList.Add(acB);

                        rc.Accelerators.AddRange(acList);
                        Console.WriteLine("Modified AcceleratorResource: " + rc.Name);
                        Console.WriteLine(rc);
                        Console.ReadLine();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.ToString());
                        Console.ReadLine();
                        Environment.Exit(-1);
                    }
                    ri.Unload();
                }
            }
        }
    }
}