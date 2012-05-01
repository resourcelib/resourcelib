ResourceLib C# File Resource Management Library
===============================================

There are several good articles about reading and writing resources from/to a compiled binary. Most focus on retrieving module version information and modifying version information, mostly in C++. Some detail the same operations for cursors, icons or dialog resources. There is, however, no single .NET library to retrieve and save any type of resources, or any library to edit version resources specifically. 

This implementation is a framework that enumerates resources and implements both read and write of the file version VS_VERSIONINFO, string resources such as company, copyright and product information, RT_GROUP_ICON and RT_ICON, RT_CURSOR, RT_BITMAP, RT_MENU, RT_DIALOG, RT_STRING, RT_ACCELERATOR, RT_FONT and RT_FONTDIR and RT_MANIFEST resources. It is unit-tested and documented.

Essentials
----------

* [Download Version 1.2](https://github.com/downloads/dblock/resourcelib/Vestris.ResourceLib.1.2.zip)

Copyright and License
---------------------

This project is licensed under the MIT license.
