# documentation-filter
![Build And Test .NET Core Projects](https://github.com/tomashalac/documentation-filter/workflows/Build%20And%20Test%20.NET%20Core%20Projects/badge.svg)

It allows to remove blocks of documentation generated by the C# comiler according to accessibility conditions.


## Example
```c#
var doc = new DocumentationFilter("documentation-filter.xml", Environment.CurrentDirectory + "/documentation-filter.dll");
doc.MoveOnlyPublics();
```

## Documentation

### new DocumentationFilter(String fullFileName, String dllToLoad)
This object allows you to manipulate the specified documentation.
* **fullFileName** The relative path to the file, example "docs.xml"
* **dllToLoad** The absolute path to the DLL, example "C:\test\MyCode.dll"

### DocumentationFilter.MoveOnlyPublics
Move all summaries that are public to the other file "new_{fullFileName}.xml"
