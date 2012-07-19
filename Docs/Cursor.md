Cursor (RT_CURSOR, RT_GROUP_CURSOR)
===================================

When `ResourceInfo` encouters a resource of type 12 (`RT_GROUP_CURSOR`), it creates an object of type `CursorDirectoryResource`. The latter creates a `CursorResource`, which loads an `DeviceIndependentBitmap`. 

* A `CursorDirectoryResource` represents a collection of cursor resources. 
* A `CursorResource` represents a single cursor with multiple images of type `DeviceIndependentBitmap`.
* A `DeviceIndependentBitmap` is not a resource, but raw data embedded in the file at an offset defined by the cursor resource and represents a single cursor bitmap. This data includes the image's XOR bitmap followed by its AND bitmap. These two bitmaps are used together to support transparency. 

Reading Cursor Resources
------------------------

The following example loads a known cursor resource directly from `guitils.dll`, without enumerating all resources. 

``` csharp
string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "gutils.dll");
CursorDirectoryResource rc = new CursorDirectoryResource();
rc.Name = new ResourceId("HORZLINE");
rc.LoadFrom(filename);
Console.WriteLine("CursorDirectoryResource: {0}, {1}", rc.Name, rc.TypeName);
foreach (CursorResource cursor in rc.Icons)
{
  Console.WriteLine(" Cursor {0}: {1} ({2} byte(s)), hotspot @ {3}:{4}",
      cursor.Header.nID, cursor.ToString(), cursor.ImageSize, cursor.HotSpotX, cursor.HotSpotY);
}
```

`GUtils.dll` contains two cursor groups, including "HORZLINE". The following output is generated from the example above. 

```
CursorDirectoryResource: HORZLINE, RT_GROUP_CURSOR
Cursor 2: 32x0 1-bit B/W (308 byte(s)), hotspot @ 16:16
```

Writing Cursor Resources
------------------------

In order to embed an existing cursor from a `.cur` file into an executable (`.exe` or `.dll`) we load the `.cur` file and convert it to a `CursorDirectoryResource`. The structure in a `.cur` file is similar to the structure of the cursor in an executable with two major differences: the executable stores cursor hotspot information and the header contains the cursor Id, while a `.cur` header simply contains the offset of cursor data. 

It's important to identify existing cursor resources in the executable and define the cursor Id. For example, if there're 2 existing `RT_GROUP_CURSOR` resources in the target binary, the added resource's Id must be 3. 

``` csharp
CursorFile cursorFile = new CursorFile("Cursor1.ico");
CursorDirectoryResource cursorDirectoryResource = new CursorDirectoryResource(cursorFile);
cursorDirectoryResource.Name = new ResourceId("TEST");
cursorDirectoryResource.Language = ResourceUtil.USENGLISHLANGID;
cursorDirectoryResource.Icons[0].Id = 3;
cursorDirectoryResource.Icons[0].HotSpotX = 12;
cursorDirectoryResource.Icons[0].HotSpotY = 12;
cursorDirectoryResource.Icons[0].Language = ResourceUtil.USENGLISHLANGID;
cursorDirectoryResource.SaveTo("test.dll");
```

