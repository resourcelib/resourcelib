using System;
using System.Collections.Generic;
using System.Text;
using Vestris.ResourceLib;

namespace Vestris.ResourceLibUnitTests
{
    public abstract class DumpResource
    {
        public static void Dump(ResourceInfo ri)
        {
            Console.WriteLine("Resource types: ");
            foreach (string type in ri.ResourceTypes)
            {
                Console.WriteLine(type);
            }

            Console.WriteLine("Resources: ");
            Dictionary<ResourceId, List<Resource>>.Enumerator riEnumerator = ri.Resources.GetEnumerator();
            while (riEnumerator.MoveNext())
            {
                Console.WriteLine("Type: {0}", riEnumerator.Current.Key);
                foreach (Resource r in riEnumerator.Current.Value)
                {
                    Dump(r);
                }
            }
        }

        public static void Dump(Resource rc)
        {
            Console.WriteLine("Resource: {0} of type {1}, {2} byte(s)", rc.Name, rc.Type, rc.Size);
            if (rc is VersionResource)
            {
                Dump(rc as VersionResource);
            }
            else if (rc is IconResource)
            {
                Dump(rc as IconResource);
            }
            else if (rc is ManifestResource)
            {
                Dump(rc as ManifestResource);
            }
        }

        public static void Dump(ManifestResource rc)
        {
            Console.WriteLine("Manifest type: {0}", rc.ManifestType);
            Console.WriteLine(rc.Manifest.OuterXml);
        }

        public static void Dump(VersionResource rc)
        {
            Console.WriteLine("File version: {0}", rc.FileVersion);
            Console.WriteLine("Product version: {0}", rc.ProductVersion);
            Console.WriteLine("Language: {0}", rc.Language);
            Dictionary<string, ResourceTable>.Enumerator resourceEnumerator = rc.Resources.GetEnumerator();
            while (resourceEnumerator.MoveNext())
            {
                Dump(resourceEnumerator.Current.Value);
            }
        }

        public static void Dump(GroupIconResource rc)
        {
            Console.WriteLine(" GroupIconResource: {0}, {1}", rc.Name, rc.Type);
            foreach (IconResource icon in rc.Icons)
            {
                Dump(icon);
            }
        }

        public static void Dump(IconResource rc)
        {
            Console.WriteLine(" Icon: {0} ({1} byte(s))",
                rc.ToString(), rc.ImageSize);
            Console.WriteLine("  {0} ({1}x{2})",
                rc.Image.Header.BitmapCompression, rc.Image.Header.biHeight, rc.Image.Header.biWidth);
        }

        public static void Dump(ResourceTable rc)
        {
            if (rc is StringFileInfo)
                Dump(rc as StringFileInfo);
            else if (rc is VarFileInfo)
                Dump(rc as VarFileInfo);
            else if (rc is StringTable)
                Dump(rc as StringTable);
            else if (rc is VarTable)
                Dump(rc as VarTable);
        }

        public static void Dump(StringFileInfo resource)
        {
            Console.WriteLine("StringFileInfo: {0}", resource.Key);
            Dictionary<string, StringTable>.Enumerator enumerator = resource.Strings.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Dump(enumerator.Current.Value);
            }
        }

        public static void Dump(VarFileInfo resource)
        {
            Console.WriteLine("VarFileInfo: {0}", resource.Key);
            Dictionary<string, VarTable>.Enumerator enumerator = resource.Vars.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Dump(enumerator.Current.Value);
            }
        }

        public static void Dump(StringTable stringTableResource)
        {
            Console.WriteLine("StringTableResource: {0} [{1}:{2}]", 
                stringTableResource.Key, stringTableResource.LanguageID, stringTableResource.CodePage);
            Dictionary<string, StringResource>.Enumerator stringEnumerator = stringTableResource.Strings.GetEnumerator();
            while (stringEnumerator.MoveNext())
            {
                Console.WriteLine(" {0} = {1}",
                    stringEnumerator.Current.Key,
                    stringEnumerator.Current.Value.StringValue);
            }
        }

        public static void Dump(VarTable varTableResource)
        {
            Console.WriteLine("VarTableResource: {0}", varTableResource.Key);
            Dictionary<UInt16, UInt16>.Enumerator langEnumerator = varTableResource.Languages.GetEnumerator();
            while (langEnumerator.MoveNext())
            {
                Console.WriteLine(" 0x{0:X} => {1}", langEnumerator.Current.Key, langEnumerator.Current.Value);
            }
        }
    }
}
