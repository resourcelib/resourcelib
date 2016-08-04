using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Vestris.ResourceLib;

namespace Res_sample
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");

            //Predicate pReload to search for Accelerator with key F5 && command 41061 (=reload Explorer)
            Predicate<Accelerator> pReload = (Accelerator a) => { return a.Key == "VK_F5" && a.Command == 41061; };
            using (ResourceInfo ri = new ResourceInfo())
            {
                ri.Load(filename);
                foreach (AcceleratorResource rc in ri[Kernel32.ResourceTypes.RT_ACCELERATOR])
                {
                    Console.WriteLine("Current AcceleratorResource: {0}, {1}", rc.Name, rc.TypeName);
                    Console.WriteLine(rc+"\n------------------------------------\n");

                    //creating new Accelerator newAC, adding Key, Command and Flags
                    Accelerator newAC = new Accelerator();
                    newAC.Key = "J";
                    newAC.Command = 41008;
                    newAC.Flags = User32.AcceleratorVirtualKey.VIRTKEY | User32.AcceleratorVirtualKey.NOINVERT | User32.AcceleratorVirtualKey.CONTROL;

                    try
                    {
                        //searching pReload and copying it to tst Accelerator
                        Accelerator tst = rc.Accelerators.Find(pReload);

                        //saving index of Accelerator, then removing from AcceleratorResource rc
                        int loc = rc.Accelerators.FindIndex(pReload);
                        rc.Accelerators.Remove(tst);

                        //changing Accelerator key to NUMPAD9, adding CONTROL-Flag and re-adding it to resource at previous position
                        tst.Key = "VK_NUMPAD9";
                        tst.addFlag(User32.AcceleratorVirtualKey.CONTROL);
                        rc.Accelerators.Insert(loc, tst);

                        //adding newAC to end of AcceleratorResource
                        rc.Accelerators.Add(newAC);

                        Console.WriteLine("Modified AcceleratorResource: "+rc.Name);
                        Console.WriteLine(rc);
                        Console.ReadLine();
                    }
                    catch(Exception e)
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