# Ponder

Ponder (a.k.a. `dotnet-ponder`) is a [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) for loading and running tests
for a .NET solution (*.sln file).

## Status

Ponder is currently **alpha** and is not particularly useful yet. See below for
Ponder's development roadmap.

## Installation/Upgrading

Prerequisites: .NET 6 SDK (or newer)

Install using:
```
dotnet tool install -g dotnet-ponder --version <your desired version>
```

**Note that you will need to specify a version, as `dotnet-ponder` is only publishing `-alpha` tagged packages.**

Upgrade using:

```
dotnet tool update -g dotnet-ponder --version <your desired version>
```

If you have trouble getting the latest version, try adding the `--no-cache` argument:

```
dotnet tool update -g dotnet-ponder --no-cache --version <your desired version>
```


## Running

Use the command `dotnet ponder` in a directory containing a single *.sln file, or
pass the path to the solution file a la `dotnet ponder some-solution.sln`.

## Roadmap

* Version 0.1 - Show solution contents

  Display what projects exist in a solution, and indicate which are test projects

* Version 0.2 - Run all tests on manual trigger

  Runs all tests in the solution, one project at a time, when the user presses a key

* Version 0.3 - Rerun on source code change

  Reruns all tests, one project at a time, when a source code file changes

* Version 0.4 - View build/test failure details

  Display detailed information about build and test failures

* Version 0.5 - Run tests efficiently

  Reruns only relevant builds and tests based on file changes

* Version 0.6 - Improve output relevance

  Show expected time for builds and tests; elide irrelevant stack frames from test failure stack traces

* Version 0.7 - Focus on specific failures

  Add a "failure cursor" that allows navigating from one failure to the next; retains cursor location on subsequent test runs

* Version 1.0

  Parallelize builds & tests; run once in CI environment
