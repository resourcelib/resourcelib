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
Special case handling:

Currently when loading an `IconDirectoryResource` from file `RT_GROUP_ICON` resource will be searched by default ID = 1. 
In many executables though the `RT_GROUP_ICON` resource will be named differently so you might expect exception to be 
thrown when loading some executables, saying that the resource with given ID cannot be found. In order to handle this
you should explicitly define `RT_GROUP_ICON` resource ID and Language as follows:

``` csharp
const ushort DEFAULT_LANG_ID = 1033;
const uint RT_GROUP_ICON_ID = 10; //whatever was spot 

IconDirectoryResource rc = new IconDirectoryResource();
rc.Name = new ResourceId(RT_GROUP_ICON_ID);
rc.Language = DEFAULT_LANG_ID;
rc.LoadFrom(@"d:\test.exe");
```

Both `DEFAULT_LANG_ID` and `RT_GROUP_ICON_ID` may be spot by either any resource viewer available or
by custom methods. 

The default implementation of `IconDirectoryResource` loads a US-English resource of type `RT_GROUP_ICON` with ID 1. This may not be the case in your executable. You can also load a resource with a different ID and language.

``` csharp
IconDirectoryResource rc = new IconDirectoryResource();
rc.Name = new ResourceId(10);
rc.Language = ResourceUtil.USENGLISHLANGID;
rc.LoadFrom(filename);
```

Writing Icon Resources
----------------------

In order to embed an existing icon from an `.ico` file into an executable (`.exe` or `.dll`) we load the `.ico` file and convert it to an `IconDirectoryResource`. The structure in an `.ico` file is similar to the structure of the icon in an executable. The only difference is that the executable headers store the icon Id, while an `.ico` header contains the offset of icon data.

``` csharp
IconFile iconFile = new IconFile("Icon1.ico");
IconDirectoryResource iconDirectoryResource = new IconDirectoryResource(iconFile);
iconDirectoryResource.SaveTo("test.dll");
```

Special case handling:

For the same reason described in `Reading Icon Resources` section you may encounter problem updating existing icon
in executable. At this moment following workaround to be used to achieve the goal correctly:

```csharp
const ushort DEFAULT_LANG_ID = 1033;
const uint RT_GROUP_ICON_ID = 10;
string fileName = @"d:\test.exe";
//load executable to be modified
IconDirectoryResource rc = new IconDirectoryResource();
rc.Name = new ResourceId(RT_GROUP_ICON_ID);
rc.Language = DEFAULT_LANG_ID;
rc.LoadFrom(fileName);
//clear existing icons
rc.Icons.Clear();

//load new icon to embed
IconFile iconFile = new IconFile(@"d:\icon.ico");
//embed new icons instead
uint id = 1;
foreach (IconFileIcon icon in iconFile.Icons)
{
    rc.Icons.Add(new IconResource(icon, new ResourceId(id++), DEFAULT_LANG_ID));
}
rc.SaveTo(fileName);
```
