String (RT_STRING)
==================

Unlike the other resource formats, where the resource identifier is the same as the value listed in the `.rc` file, string resources are packaged in blocks. Each string resource block has 16 strings. 

When `ResourceInfo` encouters a resource of type 9 (`RT_STRING`), it creates an object of type `StringResource` which contains a table of strings. 

Reading String Resources
------------------------

The following example loads all string resources from a file. 

``` csharp
string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
using (ResourceInfo ri = new ResourceInfo())
{
    ri.Load(filename);
    foreach(StringResource rc in ri[Kernel32.ResourceTypes.RT_STRING])
    {
        Console.WriteLine("StringResource: {0}, {1}", rc.Name, rc.TypeName);
        Console.WriteLine(rc);
    }
}
```

`Explorer.exe` contains several string tables, including this one. 

```
StringResource: 19, RT_STRING
STRINGTABLE
BEGIN
 300 Store letters, reports, notes, and other kinds of documents.
 301 Displays recently opened documents and folders.
 302 Store and play music and other audio files.
 303 Store pictures and other graphics files.
END
```

Reading Specific String Resources
---------------------------------

In order to locate a string you must first calculate the string table block that contains it with `GetBlockId` and use the result to load the resource. 

``` csharp
public static UInt16 GetBlockId(int stringId)
{
    return (UInt16)((stringId / 16) + 1);
}
```

The following example retrieves string Id=8243 from `explorer.exe`. 

``` csharp
string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
StringResource sr = new StringResource();
sr.Name = new ResourceId(StringResource.GetBlockId(8243));
sr.LoadFrom(filename);
Console.WriteLine(sr[8243]);
```

Writing a String Table
----------------------

Special care of preserving Ids must be taken when writing string tables. To add a string of Id 8243 create an instance of `StringResource`, assign it a block Id resulting from `GetBlockId` and add the string to its strings collection. 

``` csharp
StringResource sr = new StringResource();
sr.Name = new ResourceId(StringResource.GetBlockId(1256));
sr[1256] = "string of id 1256";
sr.SaveTo(targetFilename);
```
