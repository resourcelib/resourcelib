ResourceLib C# File Resource Management Library
===============================================

There are several good articles about reading and writing resources from/to a compiled binary. Most focus on retrieving module version information and modifying version information, mostly in C++. Some detail the same operations for cursors, icons or dialog resources. There is, however, no single .NET library to retrieve and save any type of resources, or any library to edit version resources specifically. 

This project is a framework that enumerates resources and implements both read and write of the file version VS_VERSIONINFO, string resources such as company, copyright and product information, RT_GROUP_ICON and RT_ICON, RT_CURSOR, RT_BITMAP, RT_MENU, RT_DIALOG, RT_STRING, RT_ACCELERATOR, RT_FONT and RT_FONTDIR and RT_MANIFEST resources. It's unit-tested and well documented.

Essentials
----------

* [Download Version 1.2](https://github.com/downloads/dblock/resourcelib/Vestris.ResourceLib.1.2.zip)
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

Please refer to the CHM help file in the distribution package for more information.

Copyright and License
---------------------

Copyright (c) Daniel Doubrovkine, Vestris Inc., 2008-2012 

This project is licensed under the MIT license.
