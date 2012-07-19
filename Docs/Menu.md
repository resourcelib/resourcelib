Menu (RT_MENU)
==============

When `ResourceInfo` encouters a resource of type 4 (`RT_MENU`), it creates an object of type `MenuResource` which contains either a `MenuTemplate` or a `MenuExTemplate`. These structures represent a single menu template in a standard or extended 32-bit format and include a collection of `MenuTemplateItem` or `MenuExTemplateItem` controls respectively. 

Reading Menu Resources
----------------------

The following example loads all menu resources from a file. 

``` csharp
string filename = Path.Combine(Environment.GetVariable("WINDIR"), "explorer.exe");
using (ResourceInfo ri = new ResourceInfo())
{
    ri.Load(filename);
    foreach(MenuResource rc in ri[Kernel32.ResourceTypes.RT_MENU])
    {
        Console.WriteLine("MenuResource: {0}, {1}", rc.Name, rc.TypeName);
        Console.WriteLine(rc.ToString());
    }
}
```

`Explorer.exe` on Windows Vista contains several standard and extended menu resources, including the following. 

```
MenuResource: 204, RT_MENU
MENUEX 0
BEGIN
 MENUITEM "", 129
 MENUITEM SEPARATOR
 POPUP "&Programs"
 BEGIN
  MENUITEM "(Empty)    ", 513
 END
 POPUP "F&avorites"
 BEGIN
  MENUITEM "(Empty)    ", 4294967295
 END
 POPUP "&Documents"
 BEGIN
  MENUITEM "(Empty)    ", 514
 END
 POPUP "&Settings"
 BEGIN
  MENUITEM "&Control Panel", 505
  MENUITEM SEPARATOR
  MENUITEM "&Windows Security...", 5001
  MENUITEM "&Network Connections", 557
  MENUITEM "&Printers", 510
  MENUITEM "&Taskbar and Start Menu", 413
 END
 POPUP "Sear&ch"
 BEGIN
  MENUITEM SEPARATOR
 END
 MENUITEM "&Help and Support", 503
 MENUITEM "&Run...", 401
 MENUITEM SEPARATOR
 MENUITEM "S&ynchronize", 553
 MENUITEM "&Log Off %s...", 402
 MENUITEM "D&isconnect...", 5000
 MENUITEM "Undock Comput&er", 410
 MENUITEM "Sh&ut Down...", 506
END
```

```
MenuResource: 210, RT_MENU
MENUEX 0
BEGIN
 MENUITEM "", 129
 MENUITEM "Ca&scade", 403
 MENUITEM "Show Windows S&tacked", 405
 MENUITEM "Show Windows S&ide by Side", 404
 MENUITEM "&Minimize Group", 311
 MENUITEM SEPARATOR
 MENUITEM "&Close Group", 48913
END
```

Reading Specific Menu Resources
-------------------------------

The following example loads a single menu resource directly, without enumerating all resources. 

``` csharp
string filename = Path.Combine(Environment.GetVariable("WINDIR"), "explorer.exe");
MenuResource rc = new MenuResource();
rc.Name = new ResourceId(204);
rc.LoadFrom(filename);
Console.WriteLine("MenuResource: {0}, {1}", rc.Name, rc.TypeName);
Console.WriteLine(rc.ToString());
```

Writing Menu Resources
----------------------

In order to embed a menu into an executable (`.exe` or `.dll`) create a new instance of `MenuTemplate` or a `MenuExTemplate`, add menu items, assign it to a `MenuResource` and save it to the destination file. You may also copy a menu from one file to another, etc.

``` csharp
source sourceFilename = ...
string targetFilename = ...
MenuResource sourceMenu = new MenuResource();
sourceMenu.Name = new ResourceId("MENUID");
sourceMenu.LoadFrom(sourceFilename);
sourceMenu.SaveTo(targetFilename);
```
