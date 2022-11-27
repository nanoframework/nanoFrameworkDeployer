[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFrameworkDeployer&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFrameworkDeployer) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFrameworkDeployer&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFrameworkDeployer) [![Build Status](https://dev.azure.com/nanoframework/nanoFrameworkDeployer/_apis/build/status/nanoFrameworkDeployer?repoName=nanoframework%2FnanoFrameworkDeployer&branchName=main)](https://dev.azure.com/nanoframework/nanoFrameworkDeployer/_build/latest?definitionId=80&repoName=nanoframework%2FnanoFrameworkDeployer&branchName=main) [![NuGet](https://img.shields.io/nuget/dt/nanoFrameworkDeployer.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFrameworkDeployer/) [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/master/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

# Cross-platform .NET nanoFrameworkDeployer Console Application

This repo provides a .NET Console Application that can deploy your .NET **nanoFramework** projects to a **nanoFramework** supported device connected via USB. While this is already possible through the **nanoFramework** extension in Visual Studio for Windows, this repository aims to add support for deploying your projects on non-Windows based devices such as macOS and Linux.

## Requirements


### Windows

.NET 4.7.2

### Linux / macOS
- mono-complete on non Windows platforms, you can find out how to install mono [here](https://www.mono-project.com/docs/getting-started/install/). The reason that you need mono is because the tool requires .NET 4.7.2. This tool cannot yet be build using .NET 6.0 or .NET Core because some of the dependencies are those used by the Visual Studio extension. The other reason why you need mono is to build a nanoFramework project.

## Getting Started

The tool provide various options:

```text
  -d, --directory    Required. Folder containing the PE files.
  -v, --verbose      Show verbose messages.
  -c, --comport      The COM port to use if multiple. By default, the first
                     valid found is used.
  -e, --exception    COM Port exception file.
  -r, --reboot       Reboot the device after flash.
  -b, --bin          Creates a deployment binary file only. This is not attempting to deploy.
  --help             Display this help screen.
  --version          Display version information.
```


**NOTE: When running this tool in a NON Windows environment, you need to use `mono`.** 
**If you are using Windows 7 or above, you ***DO NOT*** need `mono` **

You can then use commands like:

### Linux / macOS
```shell
mono nanoFrameworkDeployer -d path_to_pe_files
```

`path_to_pe_files` is the path to the build folder where all the `.pe` files are located. Note that the tool will automatically upload all the `.pe` files available in that folder.


### Excluding a COM port

Some virtual COM ports are provided by Bluetooth devices and other software. By default the tool will scan all the possible mounted COM ports to try to find a valid .NET nanoFramework device. That can disconnect temporally your connected headset or any other device using a COM port and may also cause lockups in the deployer tool. In that case, you can create an exclusion file. Just list the COM ports you want to exclude from the search, one port per line. You can then use the `-e` option to pass the name of the file.

```text
/dev/tty-bluetooth
COM7
```

### Rebooting the device once flashed

You can automatically ask the tool to reboot your device once flashed, used the `-r` options for that.

### Selecting a specific COM port

If you have multiple valid .NET **nanoFramework** devices connected, you can select a specific COM port, just use the `-c` option to specify that followed by the COM port you want to use.

### Creating a binary deployment file

Using the --bin or -b option will create a binary deployment file. You can then use it with `nanoff` to flash your device. Please note that using this option will **not** flash the device. All other options except the directory one will be ignored.

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
