Version (RT_VERSION)
====================

When `ResourceInfo` encouters a resource of type 16 (`RT_VERSION`), it creates an object of type `VersionResource`.

A version resource is stored in a `VS_VERSIONINFO` root structure, which contains the following file-version information. 

* `VS_FIXEDFILEINFO`: language- and code page-independent file version information. 
* `StringFileInfo`: language and code page formatting information for the strings. 
* `VarFileInfo`: version information not dependent on a particular language and code page combination. 

Reading Version Resources
-------------------------

The following example loads file version information directly, without enumerating all resources. 

``` csharp
string filename = Path.Combine(Environment.SystemDirectory, "atl.dll");
VersionResource versionResource = new VersionResource();
versionResource.LoadFrom(filename);
Console.WriteLine("File version: {0}", versionResource.FileVersion);
StringFileInfo stringFileInfo = (StringFileInfo) versionResource["StringFileInfo"];
foreach (KeyValuePair<string, StringTableEntry> stringTableEntry in stringFileInfo.Default.Strings)
{
  Console.WriteLine("{0} = {1}", stringTableEntry.Value.Key, stringTableEntry.Value.StringValue);
}
```

Writing Version Information
---------------------------

The easiest way to update version information is to load an existing binary resource, update it and save it back. 

``` csharp
string filename = "test.dll";
VersionResource versionResource = new VersionResource();
versionResource.LoadFrom(filename);
Console.WriteLine("File version: {0}", versionResource.FileVersion);
versionResource.FileVersion = "1.2.3.4";
StringFileInfo stringFileInfo = (StringFileInfo) versionResource["StringFileInfo"];
stringFileInfo["CompanyName"] = "My Company\0";
stringFileInfo["Weather"] = "Sunshine, beach volleyball.\0"; 
versionResource.SaveTo(filename);
```

Note that internally string resources are stored with an extra null terminator, a requirement of the Windows operating system. The library is consistent with this and always stores the value with two null terminators while doing the work of appending it to the assigned value when required.

Generating a complete version resource header allows you to save version information into a file that doesn't have any. This is more involved because you must generate all the structures. ResourceLib makes it easy since you don't have to worry about structure sizes or data alignment.

``` csharp
VersionResource versionResource = new VersionResource();
versionResource.FileVersion = "1.2.3.4";
versionResource.ProductVersion = "4.5.6.7";

StringFileInfo stringFileInfo = new StringFileInfo();
versionResource[stringFileInfo.Key] = stringFileInfo;
StringTable stringFileInfoStrings = new StringTable();
stringFileInfoStrings.LanguageID = 1033;
stringFileInfoStrings.CodePage = 1200;
stringFileInfo.Strings.Add(stringFileInfoStrings.Key, stringFileInfoStrings);
stringFileInfoStrings["ProductName"] = "ResourceLib";
stringFileInfoStrings["FileDescription"] = "File updated by ResourceLib";
stringFileInfoStrings["CompanyName"] = "Vestris Inc.\0"; // \0 is optional, ResourceLib will append it
stringFileInfoStrings["LegalCopyright"] = "All Rights Reserved";
stringFileInfoStrings["Comments"] = "This file has a version resource updated by ResourceLib";
stringFileInfoStrings["ProductVersion"] = string.Format("{0}\0", versionResource.ProductVersion);

VarFileInfo varFileInfo = new VarFileInfo();
versionResource[varFileInfo.Key] = varFileInfo;
VarTable varFileInfoTranslation = new VarTable("Translation");
varFileInfo.Vars.Add(varFileInfoTranslation.Key, varFileInfoTranslation);
varFileInfoTranslation[ResourceUtil.USENGLISHLANGID] = 1300;

versionResource.SaveTo(filename);
```
