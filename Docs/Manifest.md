Manifest (RT_MANIFEST)
======================

When ResourceInfo encouters a resource of type 24 (`RT_MANIFEST`), it creates an object of type `ManifestResource`. The latter loads the embedded Side-by-Side (SxS) Manifest as an XML document.

See [Isolated Applications and Side-by-Side Assemblies Manifests](http://msdn.microsoft.com/en-us/library/aa375365.aspx) for more information.

Reading Manifest Resources
--------------------------

The following example loads the SxS manifest directly, without enumerating all resources. 

``` csharp
string filename = Path.Combine(Environment.SystemDirectory, "write.exe");
ManifestResource rc = new ManifestResource();
rc.LoadFrom(filename);
Console.WriteLine("Manifest type: {0}", rc.ManifestType);
Console.WriteLine(rc.Manifest.OuterXml);
```
Writing Manifest Resources
--------------------------

A manifest can be written directly to an existing binary file, adding a new manifest or replacing an existing one. 

``` csharp
ManifestResource manifestResource = new ManifestResource(Kernel32.ManifestType.CreateProcess);
// A default manifest resource is valid, but has no useful attributes, load the desired manifest here.
// manifestResource.Manifest.LoadXml(...);
manifestResource.SaveTo(targetFilename);
```
Rewriting an identical manifest does not necessarily preserve the size of the `RT_MANIFEST` resource because new lines in attribute values are normalized. See the [W3C spec section 3.3.3](http://www.w3.org/TR/REC-xml/#AVNormalize) for details. 
 
