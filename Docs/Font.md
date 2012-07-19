Font (RT_FONT, RT_FONTDIR)
==========================

The library has only limited support for font resources. 

Font resources are different from the other types of resources in that they are not usually added to the resources of a specific application program. Font resources are added to `.exe` files that are renamed into `.fon` files. These files are libraries as opposed to applications. 

When `ResourceInfo` encouters a resource of type 7 (`RT_FONTDIR`), it creates an object of type `FontDirectoryResource` that contains a collection of instances of `FontDirectoryEntry`. When `ResourceInfo` encouters a resource of type 8 (`RT_FONT`), it creates an object of type `FontResource`. 
