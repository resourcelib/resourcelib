Dialog (RT_DIALOG)
==================

When `ResourceInfo` encouters a resource of type 5 (`RT_DIALOG`), it creates an object of type `DialogResource` which contains either a `DialogTemplate` or a `DialogExTemplate`. These structures represent a single dialog template in a standard or extended format and include a collection of `DialogTemplateControl` or `DialogExTemplateControl` controls respectively. 

Reading Dialog Resources
------------------------

The following example loads all dialog resources from a file. 

``` csharp
string filename = Path.Combine(Environment.SystemDirectory, "acppage.dll");
using (ResourceInfo ri = new ResourceInfo())
{
    ri.Load(filename);
    foreach(DialogResource rc in ri[Kernel32.ResourceTypes.RT_DIALOG])
    {
        Console.WriteLine("DialogResource: {0}, {1}", rc.Name, rc.TypeName);
        Console.WriteLine(rc.ToString());
    }
}
```

The `acppage.dll` contains two extended dialog resources. 

```
DialogResource: 101, RT_DIALOG
DIALOGEX 0, 0, 224, 233
STYLE WS_DLGFRAME | WS_BORDER | WS_CAPTION | WS_DISABLED | WS_CHILD | DS_FIXEDSYS | DS_SETFONT | DS_SHELLFONT
EXSTYLE WS_OVERLAPPED | WS_EX_LTRREADING | WS_EX_LTRREADING | WS_EX_LTRREADING
CAPTION "Shim Layer Property Page"
FONT 8, "MS Shell Dlg"
{
 Static "If you have problems with this program and it worked correctly on an earlier version of Windows, select the compatibility mode that matches that earlier version." 1013, Static, 7, 7, 210, 31, WS_GROUP | WS_VISIBLE | WS_CHILD | SS_LEFT
 Button "Run this program in compatibility mode for:" 5000, Button, 18, 53, 149, 10, WS_TABSTOP | WS_VISIBLE | WS_CHILD | SS_LEFT| BS_AUTOCHECKBOX
 ComboBox "" 1005, ComboBox, 18, 66, 152, 85, WS_TABSTOP | WS_VSCROLL | WS_DISABLED | WS_VISIBLE | WS_CHILD | SS_LEFT
 ...
}
```

```
DialogResource: 5011, RT_DIALOG
DIALOGEX 0, 0, 224, 250
STYLE WS_DLGFRAME | WS_BORDER | WS_CAPTION | WS_DISABLED | WS_CHILD | DS_FIXEDSYS | DS_SETFONT | DS_SHELLFONT
EXSTYLE WS_OVERLAPPED | WS_EX_LTRREADING | WS_EX_LTRREADING | WS_EX_LTRREADING
CAPTION "Shim Layer Property Page"
FONT 8, "MS Shell Dlg"
{
 Static "If you have problems with this program and it worked correctly on an earlier version of Windows, select the compatibility mode that matches that earlier version." 1013, Static, 7, 7, 210, 31, WS_GROUP | WS_VISIBLE | WS_CHILD | SS_LEFT
 Button "Run this program in compatibility mode for:" 5000, Button, 18, 53, 149, 10, WS_TABSTOP | WS_VISIBLE | WS_CHILD | SS_LEFT| BS_AUTOCHECKBOX
 ComboBox "" 1005, ComboBox, 18, 66, 152, 85, WS_TABSTOP | WS_VSCROLL | WS_DISABLED | WS_VISIBLE | WS_CHILD | SS_LEFT
 Button "Run in 256 colors" 5001, Button, 18, 106, 196, 10, WS_TABSTOP | WS_VISIBLE | WS_CHILD | SS_LEFT| BS_AUTOCHECKBOX
 ...
}
```

Reading Specific Dialog Resources
---------------------------------

The following example loads a single dialog resource directly, without enumerating all resources. 

``` csharp
string filename = Path.Combine(Environment.SystemDirectory, "acppage.dll");
DialogResource rc = new DialogResource();
rc.Name = new ResourceId(5011);
rc.LoadFrom(filename);
Console.WriteLine("DialogResource: {0}, {1}", rc.Name, rc.TypeName);
Console.WriteLine(rc.ToString());
```

Writing Dialog Resources
------------------------

In order to embed a dialog into an executable (`.exe` or `.dll`) create a new instance of `DialogTemplate` or a `DialogExTemplate`, add controls, assign it to a `DialogResource` and save it to the destination file. You may also copy a dialog from one file to another, etc. 

``` csharp
source sourceFilename = ...
string targetFilename = ...
DialogResource sourceDialog = new DialogResource();
sourceDialog.Name = new ResourceId("DIALOGID");
sourceDialog.LoadFrom(sourceFilename);
sourceDialog.SaveTo(targetFilename);
```

