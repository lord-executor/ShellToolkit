[![GitHub](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/lord-executor/Larcanum.ShellToolkit/blob/master/LICENSE) [![Nuget](https://img.shields.io/nuget/v/Larcanum.ShellToolkit.svg)](https://www.nuget.org/packages/Larcanum.ShellToolkit/)

# Overview

The goal of this library is to provide a set of tools to start and interact with processes on the host system in a way that is similarly easy to use as _Bash_ or oder shells. First and foremost this means providing a convenient API on top of the rather crusty and awkward `System.Diagnostics.Process` and `System.Diagnostics.ProcessStartInfo`. On top of that, the library provides methods for building _pipelines_ of commands similar to how pipes work in Bash.

One common use case and the reason for creating this library is the creation of custom .NET command line tools that rely on other programs like `git`, `7z`, `dotnet`, `jq`, `sed`, `awk`, etc. to avoid reinventing the wheel.

# Examples

The examples here are assuming that the following command line tools are available in your system `PATH`
- [dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)
- [jq](https://jqlang.github.io/jq/)

## Capturing and Processing simple Command Output

```cs
var runner = CommandRunner.Create();

var version = Version.Parse((await runner.CaptureAsync(Command.Create("dotnet", ["--version"]))).Output!);
// version.Major = 8
// version.Minor = 0
// version.Build = 200
```

## Running Command Output through Filters and into a File

```cs
var runner = CommandRunner.Create();
var tempFile = new FileInfo(Path.GetTempFileName());

var pipeline = Command.Create("dotnet", ["list", @"C:\path\to\ShellToolkit.csproj", "package", "--format", "json"])
    .Pipe(Command.Create("jq", [".projects.[].frameworks.[].topLevelPackages.[].id"]))
    .Pipe(tempFile);
 
if (await runner.ExecAsync(pipeline) != 0)
{
    throw new Exception("Pipeline failed");
}

var result = await File.ReadAllTextAsync(tempFile.FullName);
// result = "Microsoft.Extensions.Logging.Abstractions"
```
