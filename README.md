# nukit

[![Nuget](https://img.shields.io/nuget/v/nukit)](https://www.nuget.org/packages/nukit/)

Nuke the project's build clutter.

Repeat `dotnet build` and `dotnet test` enough times and soon you'll end with a lot of build clutter.

`Nukit` simply deletes `bin` &`obj` directories so you have clean builds: `project.assets.json` is reset, `bin\Debug`/`bin/Release` is cleared, and the test directories are wiped clean.

`Nukit` is a .NET tool. To install globally:

```
dotnet tool install --global nukit
```

To run:

```
nukit <path to folder> <flags> | -h
```

```
DESCRIPTION:
A command line tool to nuke build clutter. https://github.com/tonycknight/nukit

USAGE:
    Nukit.dll [path] [OPTIONS]

ARGUMENTS:
    [path]    The path to clear. Optional.

OPTIONS:
                       DEFAULT
    -h, --help                    Prints help information.
    -v, --version                 Prints version information.
        --no-banner               Show or hide the application banner.
    -f, --force                   Force clearance without prompting.
        --dry-run                 Runs a scan, without any effect.
        --bin          True       Nuke binary directories.
        --obj          True       Nuke object directories.
        --trx                     Nuke test results.
```

