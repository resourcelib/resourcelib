# Releasing resourcelib

There're no hard rules about when to release resourcelib. Release bug fixes
frequently, features not so frequently and breaking API changes rarely.

## Release

* Verify that all issues assigned to the [milestone](https://github.com/resourcelib/resourcelib/milestones)
  are closed.

* Run tests, check that all tests succeed locally.

  ```bash
  vstest.console .\Source\ResourceLibUnitTests\bin\Debug\netcoreapp2.0\ResourceLibUnitTests.dll
  ```

  (`dotnet test` is not supported yet due to restrictions of GitVersion, see
  [GitTools/GitVersion#1269](https://github.com/GitTools/GitVersion/pull/1269))

* Check that the last build succeeded in [AppVeyor](https://ci.appveyor.com/project/thoemmi/resourcelib/branch/master).

* Change "Next" in [CHANGELOG.md](CHANGELOG.md) to the current date.

  ```markdown
  ### 2.0 (5/15/2018)
  ```

* Commit your changes in the **master** branch, tag it with the current version (resourcelib uses
  [GitVersion](https://github.com/GitTools/GitVersion) to generate semantic version
  numbers) and push it to GitHub:

  ```bash
  git add CHANGELOG.md
  git commit -m "Releasing v2.0."
  git tag -a v2.0 -m "Releasing v2.0."
  git push origin master --follow-tags
  ```

The continuos integration build at AppVeyor will kick in, compile resourcelib
with the version number 2.0, generate the NuGet package and upload it to the
repository at [nuget.org](https://www.nuget.org/packages/Vestris.ResourceLib).

## Prepare for the Next Version

* Add the next release to [CHANGELOG.md](CHANGELOG.md).

  ```markdown
  ### 2.1 (Next)

  * Your contribution here.
  ```

* Update the next version in [GitVersion.yml](GitVersion.yml)

  ```yaml
  next-version: 2.1
  ```

* Commit your changes.

  ```bash
  git add CHANGELOG.md
  git commit -m "Preparing for next development iteration, 2.1. [skip ci]"
  git push origin master
  ```

  (The commit message includes `[skip ci]` to prevent a CI build)
