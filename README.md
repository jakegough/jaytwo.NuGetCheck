# jaytwo.Common.ParseExtensions

A [.NET Core Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) (and docker image) that can check to see if a NuGet package/version has already been published.

I use this in my CI pipelines to fail a build in which I forget to bump the version.


## Installation

dotnet tool install -g jaytwo.NuGetCheck

## Normal Usage

To see if the package has any published versions greater than or equal to `2.0.0` and less than `3.0.0`, run the following command:

```bash
nugetcheck --packageId xunit --minVersion 2.0.0 --maxVersion 3.0.0
```

> For any .NET Core Global Tools to work, your path needs to include `~/.dotnet/tools`

## Docker Usage

Sometimes the easiest way to use someting is in a docker image.  To see if the package has any published versions greater than or equal to `2.0.0` and less than `3.0.0`, run the following command:

```bash
docker run -it --rm jakegough/nugetcheck --packageId xunit --minVersion 2.0.0 --maxVersion 3.0.0
```
