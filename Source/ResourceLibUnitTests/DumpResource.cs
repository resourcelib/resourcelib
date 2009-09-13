using System;
using System.Collections.Generic;
using System.Text;
using Vestris.ResourceLib;
using System.Globalization;
using System.Drawing;
using System.IO;

namespace Vestris.ResourceLibUnitTests
{
    public abstract class DumpResource
    {
        public static void Dump(ResourceInfo ri)
        {
            Console.WriteLine("Resources: ");
            Dictionary<ResourceId, List<Resource>>.Enumerator riEnumerator = ri.Resources.GetEnumerator();
            while (riEnumerator.MoveNext())
            {
                Console.WriteLine("Type: {0} ({1})", riEnumerator.Current.Key, 
                    riEnumerator.Current.Key.IsIntResource() ? 
                        riEnumerator.Current.Key.ResourceType.ToString() 
                        : riEnumerator.Current.Key.Name);
                foreach (Resource r in riEnumerator.Current.Value)
                {
                    Dump(r);
                }
            }
        }

        public static void Dump(Resource rc)
        {            
            Console.WriteLine("Resource: {0} of type {1}, {2} byte(s) [{3}]",
                rc.Name, rc.TypeName, rc.Size,
                rc.Language == ResourceUtil.NEUTRALLANGID 
                    ? "Neutral" 
                    : CultureInfo.GetCultureInfo(rc.Language).Name);

            if (rc is VersionResource)
                Dump(rc as VersionResource);
            else if (rc is IconDirectoryResource)
                Dump(rc as IconDirectoryResource);
            else if (rc is CursorDirectoryResource)
                Dump(rc as CursorDirectoryResource);
            else if (rc is IconImageResource)
                Dump(rc as IconImageResource);
            else if (rc is ManifestResource)
                Dump(rc as ManifestResource);
            else if (rc is IconResource)
                Dump(rc as IconResource);
            else if (rc is CursorResource)
                Dump(rc as CursorResource);
            else if (rc is BitmapResource)
                Dump(rc as BitmapResource);
            else if (rc is DialogResource)
                Dump(rc as DialogResource);
            else if (rc is MenuResource)
                Dump(rc as MenuResource);
            if (rc is AcceleratorResource)
                Dump(rc as AcceleratorResource);
        }

        public static void Dump(VersionResource rc)
        {
            Console.WriteLine(rc.ToString());
        }

        public static void Dump(ManifestResource rc)
        {
            Console.WriteLine("Manifest type: {0}", rc.ManifestType);
            Console.WriteLine(rc.Manifest.OuterXml);
        }

        public static void Dump(IconDirectoryResource rc)
        {
            Console.WriteLine(" IconDirectoryResource: {0}, {1}", rc.Name, rc.TypeName);
            foreach (IconResource icon in rc.Icons)
            {
                Dump(icon);
            }
        }

        public static void Dump(CursorDirectoryResource rc)
        {
            Console.WriteLine(" CursorDirectoryResource: {0}, {1}", rc.Name, rc.TypeName);
            foreach (CursorResource cursor in rc.Icons)
            {
                Dump(cursor);
            }
        }

        public static void Dump(CursorResource rc)
        {
            Console.WriteLine(" Cursor {0}: {1} ({2} byte(s)), hotspot @ {3}:{4}",
                rc.Header.nID, rc.ToString(), rc.ImageSize, rc.HotspotX, rc.HotspotY);
        }

        public static void Dump(IconResource rc)
        {
            Console.WriteLine(" Icon {0}: {1} ({2} byte(s))",
                rc.Header.nID, rc.ToString(), rc.ImageSize);
        }

        public static void Dump(IconImageResource rc)
        {
            Console.WriteLine("Image: {0}x{1}", rc.Width, rc.Height);
            Console.WriteLine("Image size: {0}", rc.ImageSize);
            Console.WriteLine("Pixel format: {0}", rc.PixelFormatString);
        }

        public static void Dump(BitmapResource rc)
        {
            Console.WriteLine("Image: {0}x{1}, {2}",
                rc.Bitmap.Header.biWidth, rc.Bitmap.Header.biHeight, rc.Bitmap.Header.PixelFormatString);

            Console.Write(" Mask: {0}x{1}", rc.Bitmap.Mask.Width, rc.Bitmap.Mask.Height);
            Console.Write(" Color: {0}x{1}", rc.Bitmap.Color.Width, rc.Bitmap.Color.Height);
            Console.WriteLine(" Image: {0}x{1}", rc.Bitmap.Image.Width, rc.Bitmap.Image.Height);
        }

        public static void Dump(DialogResource rc)
        {
            Console.WriteLine(rc.ToString());
        }

        public static void Dump(MenuResource rc)
        {
            Console.WriteLine(rc.ToString());
        }

        public static void Dump(AcceleratorResource rc)
        {
            Console.WriteLine(rc.ToString());
        }
    }
}
