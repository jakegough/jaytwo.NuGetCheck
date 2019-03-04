# jaytwo.Common.ParseExtensions

A [.NET Core Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) (and docker image) that can check to see if a NuGet package/version has already been published.

I use this in my CI pipelines to fail a build in which I forget to bump the version.


## Installation

dotnet tool install -g jaytwo.NuGetCheck

## Normal Usage

To view help:

```bash
nugetcheck --help
```

To see if the package `xunit` has any published versions greater than or equal to `2.0.0` and less than `3.0.0`, run the following command:

```bash
nugetcheck xunit -gte 2.0.0 --same-major
```

To see if the package `xunit` has any published versions greater than or equal to `2.0.0` and less than `2.1.0`, run the following command:

```bash
nugetcheck xunit -gte 2.0.0 --same-minor
```

> For any .NET Core Global Tools to work, your path needs to include `~/.dotnet/tools`

### Exit Codes

If there are no results, the process exits with code `0` (success).

If there are results, the process exits with code `-1` (fail).

If there is an exception, the process exits with code `1` (fail).

## Docker Usage

Sometimes the easiest way to use someting is in a docker image.  To see if the package has any published versions greater than or equal to `2.0.0` and less than `3.0.0`, run the following command:

```bash
docker run -it --rm jakegough/jaytwo.nugetcheck xunit -gte 2.0.0 --same-major
```
