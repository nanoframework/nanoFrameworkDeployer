# Cross-platform nanoFrameworkDeployer Console Application 

This repo provides a .NET Console Application that can deploy your nanoFramework projects to a nanoFramework supported device connected via USB. While this is already possible through the nanoFramework extension on Visual Studio 2019 on Windows, this repository aims to add support for deploying your projects on non-Windows based devices such as macOS and Linux.

## Requirements
- mono-complete
- nuget
- msbuild


## Getting Started
Restore packages:
```
nuget restore
```

Build solution:
```
msbuild
```

Run nanoFrameworkFlasher:
```
mono bin/Debug/nanoFrameworkFlasher.exe
```