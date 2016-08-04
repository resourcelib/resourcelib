Accelerator (RT_ACCELERATOR)
============================

An accelerator is a keystroke defined by the application to give the user a quick way to perform a task. 

When `ResourceInfo` encouters a resource of type 9 (`RT_ACCELERATOR`), it creates an object of type `AcceleratorResource` which contains a collection of instances of `Accelerator`. 

Reading Accelerator Resources
-----------------------------

The following example loads all accelerator resources from a file. 

``` csharp
string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
using (ResourceInfo ri = new ResourceInfo())
{
    ri.Load(filename);
    foreach(AcceleratorResource rc in ri[Kernel32.ResourceTypes.RT_ACCELERATOR])
    {
        Console.WriteLine("AcceleratorResource: {0}, {1}", rc.Name, rc.TypeName);
        Console.WriteLine(rc);
    }
}
```

`Explorer.exe` contains the following accelerator table. 

``` 
Resource: 251 of type RT_ACCELERATOR, 88 byte(s) [en-US]
251 ACCELERATORS
BEGIN
 VK_F4, 305, ALT
 VK_TAB, 41008, VIRTKEY , NOINVERT
 VK_TAB, 41008, VIRTKEY , NOINVERT , SHIFT
 VK_TAB, 41008, VIRTKEY , NOINVERT , CONTROL
 VK_TAB, 41008, VIRTKEY , NOINVERT , SHIFT , CONTROL
 VK_F5, 41061, VIRTKEY , NOINVERT
 VK_F6, 41008, VIRTKEY , NOINVERT
 VK_RETURN, 413, VIRTKEY , NOINVERT , ALT
 Z, 416, VIRTKEY , NOINVERT , CONTROL
 VK_F3, 41093, VIRTKEY , NOINVERT
 M, 419, VIRTKEY , NOINVERT , ALT
END
```
**How to interpret these values, with the following example:**

`VK_TAB, 41008, VIRTKEY , NOINVERT , CONTROL`

The Accelerator will excecute command 41008, when the User presses Ctrl + Tab

| Entry        | Explatation           | Type  |
| ------------- |:-------------:| -----:|
| `VK_TAB`      | key top press for Accelerator | Virtual Key or ASCII character |
| `41008`      | command / identifier to execute      |   Integer-Value |
| `VIRTKEY , NOINVERT , CONTROL` |   options    |    string, entries separated with ',' |
For more Information, please look at the [MSDN reference](https://msdn.microsoft.com/en-us/library/windows/desktop/aa380610(v=vs.85).aspx "ACCELERATOR resources")

Reading Specific Accelerator Resources
--------------------------------------

The following example loads a single accelerator resource directly, without enumerating all resources. 

``` csharp
string filename = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
AcceleratorResource rc = new AcceleratorResource();
rc.Name = new ResourceId(251);
rc.LoadFrom(filename);
Console.WriteLine("AcceleratorResource: {0}, {1}", rc.Name, rc.TypeName);
Console.WriteLine(rc);
```

Writing an Accelerator Table
----------------------------

In order to add an accelerator into an executable (`.exe` or `.dll`) create a new instance of `AcceleratorResource` or load an existing one and add instances of `Accelerator` to its `Accelerators` collection. 

