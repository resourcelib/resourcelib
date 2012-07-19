Bitmap (RT_BITMAP)
==================

When `ResourceInfo` encouters a resource of type 2 (`RT_BITMAP`), it creates an object of type `BitmapResource` which contains a `DeviceIndependentBitmap`. 

Reading Bitmap Resources
------------------------

The following example loads all bitmap resources from a file. 

``` csharp
string filename = Path.Combine(Environment.SystemDirectory, "msftedit.dll");
using (ResourceInfo ri = new ResourceInfo())
{
    ri.Load(filename);
    foreach(BitmapResource rc in ri[Kernel32.ResourceTypes.RT_BITMAP])
    {
        Console.WriteLine("BitmapResource: {0}, {1}", rc.Name, rc.TypeName);
        Console.WriteLine("DIB: {0}x{1} {2}", rc.Bitmap.Image.Width, rc.Bitmap.Image.Height, 
            rc.Bitmap.Header.PixelFormatString);
    }
}
```

`Msftedit.dll` contains several 4-bit 16 color resources. 

```
BitmapResource: 125, RT_BITMAP
DIB: 28x14 4-bit 16 Colors
BitmapResource: 126, RT_BITMAP
DIB: 28x14 4-bit 16 Colors
...
```

Reading Specific Bitmap Resources
---------------------------------

The following example loads a single bitmap resource directly, without enumerating all resources. 

``` csharp
string filename = Path.Combine(Environment.SystemDirectory, "msftedit.dll");
BitmapResource rc = new BitmapResource();
rc.Name = new ResourceId(125);
rc.LoadFrom(filename);
Console.WriteLine("BitmapResource: {0}, {1}", rc.Name, rc.TypeName);
Console.WriteLine("DIB: {0}x{1} {2}", rc.Bitmap.Image.Width, rc.Bitmap.Image.Height, rc.Bitmap.Header.PixelFormatString);
```

`Msftedit.dll` contains a 4-bit 16 color resource under Id 125. 

```
BitmapResource: 125, RT_BITMAP
DIB: 28x14 4-bit 16 Colors
```

Writing Bitmap Resources
------------------------

In order to embed an existing bitmap from a `.bmp` file into an executable (`.exe` or `.dll`) we load the `.bmp` file and assign it to a bitmap of a `BitmapResource`. The structure in a `.bmp` file consits of a `BITMAPFILEINFO` header followed by the device independent bitmap. 

``` csharp
BitmapFile bitmapFile = new BitmapFile("Bitmap1.bmp");
BitmapResource bitmapResource = new BitmapResource();
bitmapResource.Name = new ResourceId("TEST");
bitmapResource.SaveTo("test.dll");
```
