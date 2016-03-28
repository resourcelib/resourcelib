ResourceLib C# File Resource Management Library
===============================================

[![Build status](https://ci.appveyor.com/api/projects/status/35kx80j687p9835d/branch/master?svg=true)](https://ci.appveyor.com/project/dblock/resourcelib/branch/master)

There are several good articles about reading and writing resources from/to a compiled binary. Most focus on retrieving module version information and modifying version information, mostly in C++. Some detail the same operations for cursors, icons or dialog resources. There is, however, no single .NET library to retrieve and save any type of resources, or any library to edit version resources specifically.

This project is a framework that enumerates resources and implements both read and write of the file version `VS_VERSIONINFO`, string resources such as company, copyright and product information, `RT_GROUP_ICON` and `RT_ICON`, `RT_CURSOR`, `RT_BITMAP`, `RT_MENU`, `RT_DIALOG`, `RT_STRING`, `RT_ACCELERATOR`, `RT_FONT` and `RT_FONTDIR` and `RT_MANIFEST` resources. It's unit-tested and well documented.

Essentials
----------

* [Download Version 1.4](http://code.dblock.org/downloads/resourcelib/Vestris.ResourceLib.1.4.zip)
* [Old Versions on CodePlex](http://resourcelib.codeplex.com/)

Getting Started
---------------

In your project add a reference to `Vestris.ResourceLib.dll` and a namespace reference.

``` c#
using Vestris.ResourceLib;
```

The following example demonstrates enumerating resources by resource type. From the sample atl.dll in the Windows system directory, you will typically get the following resources: MUI, REGISTRY, TYPELIB, and RT_VERSION resource.

``` c#
string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
using (ResourceInfo vi = new ResourceInfo())
{
    vi.Load(filename);
    foreach (ResourceId id in vi.ResourceTypes)
    {
        Console.WriteLine(id);
        foreach (Resource resource in vi.Resources[id])
        {
            Console.WriteLine("{0} ({1}) - {2} byte(s)",
                resource.Name, resource.Language, resource.Size);
        }
    }
}
```

Reference
---------

* [Bitmap (RT_BITMAP)](https://github.com/dblock/resourcelib/blob/master/Docs/Bitmap.md)
* [Version (RT_VERSION)](https://github.com/dblock/resourcelib/blob/master/Docs/Version.md)
* [Icon (RT_ICON, RT_GROUP_ICON)](https://github.com/dblock/resourcelib/blob/master/Docs/Icon.md)
* [Menu (RT_MENU)](https://github.com/dblock/resourcelib/blob/master/Docs/Menu.md)
* [Dialog (RT_DIALOG)](https://github.com/dblock/resourcelib/blob/master/Docs/Dialog.md)
* [String (RT_STRING)](https://github.com/dblock/resourcelib/blob/master/Docs/String.md)
* [Font (RT_FONT, RT_FONTDIR)](https://github.com/dblock/resourcelib/blob/master/Docs/Font.md)
* [Accelerator (RT_ACCELERATOR)](https://github.com/dblock/resourcelib/blob/master/Docs/Accelerator.md)
* [Cursor (RT_CURSOR, RT_GROUP_CURSOR)](https://github.com/dblock/resourcelib/blob/master/Docs/Cursor.md)
* [Manifest (RT_MANIFEST)](https://github.com/dblock/resourcelib/blob/master/Docs/Manifest.md)

Contributing
------------

Fork the project on Github, commit changes to your local repository, push changes to your fork, and make a pull request. Bonus points for topic branches. Also see [Setting up a Development Environment](https://github.com/dblock/resourcelib/blob/master/Docs/Contributing.md).

Copyright and License
---------------------

Copyright (c) Daniel Doubrovkine, Vestris Inc., 2008-2016.

This project is licensed under the MIT license.
