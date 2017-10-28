ResourceLib C# File Resource Management Library
===============================================

[![Build status](https://ci.appveyor.com/api/projects/status/7deynnff1qboipkb/branch/master?svg=true)](https://ci.appveyor.com/project/thoemmi/resourcelib/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Vestris.ResourceLib.svg)](https://www.nuget.org/packages/Vestris.ResourceLib)

There are several good articles about reading and writing resources from/to a compiled binary. Most focus on retrieving module version information and modifying version information, mostly in C++. Some detail the same operations for cursors, icons or dialog resources. There is, however, no single .NET library to retrieve and save any type of resources, or any library to edit version resources specifically.

This project is a framework that enumerates resources and implements both read and write of the file version `VS_VERSIONINFO`, string resources such as company, copyright and product information, `RT_GROUP_ICON` and `RT_ICON`, `RT_CURSOR`, `RT_BITMAP`, `RT_MENU`, `RT_DIALOG`, `RT_STRING`, `RT_ACCELERATOR`, `RT_FONT` and `RT_FONTDIR` and `RT_MANIFEST` resources. It's unit-tested and well documented.

Installation
------------

ResourceLib is distributed as a NuGet package:

```
PM> Install-Package Vestris.ResourceLib
```

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

* [Bitmap (RT_BITMAP)](https://github.com/resourcelib/resourcelib/wiki/Bitmap-(RT_BITMAP))
* [Version (RT_VERSION)](https://github.com/resourcelib/resourcelib/wiki/Version-(RT_VERSION))
* [Icon (RT_ICON, RT_GROUP_ICON)](https://github.com/resourcelib/resourcelib/wiki/Icon-(RT_ICON,-RT_GROUP_ICON))
* [Menu (RT_MENU)](https://github.com/resourcelib/resourcelib/wiki/Menu-(RT_MENU))
* [Dialog (RT_DIALOG)](https://github.com/resourcelib/resourcelib/wiki/Dialog-(RT_DIALOG))
* [String (RT_STRING)](https://github.com/resourcelib/resourcelib/wiki/String-(RT_STRING))
* [Font (RT_FONT, RT_FONTDIR)](https://github.com/resourcelib/resourcelib/wiki/Font-(RT_FONT,-RT_FONTDIR))
* [Accelerator (RT_ACCELERATOR)](https://github.com/resourcelib/resourcelib/wiki/Accelerator-(RT_ACCELERATOR))
* [Cursor (RT_CURSOR, RT_GROUP_CURSOR)](https://github.com/resourcelib/resourcelib/wiki/Cursor-(RT_CURSOR,-RT_GROUP_CURSOR))
* [Manifest (RT_MANIFEST)](https://github.com/resourcelib/resourcelib/wiki/Manifest-(RT_MANIFEST))

Contributing
------------

Fork the project on Github, commit changes to your local repository, push changes to your fork, and make a pull request. Bonus points for topic branches. Also see [Setting up a Development Environment](https://github.com/resourcelib/resourcelib/blob/master/CONTRIBUTING.md).

Copyright and License
---------------------

Copyright (c) Daniel Doubrovkine, Vestris Inc., 2008-2016.

This project is licensed under the MIT license.
