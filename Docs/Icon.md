Icon (RT_ICON, RT_GROUP_ICON)
=============================

When `ResourceInfo` encouters a resource of type 14 (`RT_GROUP_ICON`), it creates an object of type `IconDirectoryResource`. The latter creates an `IconResource`, which loads a `DeviceIndependentBitmap`. 

* An `IconDirectoryResource` represents a collection of icon resources. 
* An `IconResource` represents a single icon with many images of type `DeviceIndependentBitmap`. 
* A `DeviceIndependentBitmap` is not a resource per-se, but raw data embedded in the file at an offset defined by the icon resource and represents a single icon bitmap, for example in a BMP format.

Reading Icon Resources
----------------------

The following example loads icon resources directly, without enumerating all resources. 

``` csharp
string filename = Path.Combine(Environment.SystemDirectory, "regedt32.exe");
IconDirectoryResource rc = new IconDirectoryResource();
rc.LoadFrom(filename);
Console.WriteLine("IconDirectoryResource: {0}, {1}", rc.Name, rc.TypeName);
foreach (IconResource icon in rc.Icons)
{
  Console.WriteLine(" Icon {0}: {1} ({2} byte(s))",
      icon.Header.nID, icon.ToString(), icon.ImageSize);
}
```

`Regedt32.exe` contains a single 16-color, 32x32 icon. The following output is generated from the example above. 

```
IconDirectoryResource: 1, RT_GROUP_ICON
Icon 1: 32x32 4-bit 16 Colors (744 byte(s))
```

Writing Icon Resources
----------------------

In order to embed an existing icon from an `.ico` file into an executable (`.exe` or `.dll`) we load the `.ico` file and convert it to an `IconDirectoryResource`. The structure in an `.ico` file is similar to the structure of the icon in an executable. The only difference is that the executable headers store the icon Id, while an `.ico` header contains the offset of icon data. 

``` csharp
IconFile iconFile = new IconFile("Icon1.ico");
IconDirectoryResource iconDirectoryResource = new IconDirectoryResource(iconFile);
iconDirectoryResource.SaveTo("test.dll");
```
