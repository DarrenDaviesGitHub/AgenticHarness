# Maintaining the `cleanapi` template

This repository is both a working solution and a `dotnet new` template. `.template.config/template.json` turns it into a template whose `sourceName` is `Ardent.Project`: every file name, folder, namespace and string containing it is replaced by the `-n` value. This file, `REQUIREMENTS.md`, build output, local databases and personal Claude settings are excluded from generated projects.

## Install from this folder

```bash
dotnet new install .            # from the repository root
dotnet new cleanapi -n Acme.Orders
```

`dotnet new cleanapi -n Acme.Orders` creates `Acme.Orders/` with `Acme.Orders.slnx`, `src/Acme.Orders.Api`, and so on, then restores packages.

## Update

Edit the files, keep the solution green (`dotnet build -warnaserror`, `dotnet format --verify-no-changes`, `dotnet test`, `pwsh .claude/hooks/test-hooks.ps1`), then reinstall:

```bash
dotnet new install . --force
```

Before releasing, generate a project into a temporary folder and run the same checks there; also search it for leftovers: `git grep -n "Ardent.Project"` must return nothing.

## Uninstall

```bash
dotnet new uninstall .                         # when installed from the folder
dotnet new uninstall Ardent.CleanApi.Template  # when installed from the NuGet package
```

## Pack and share as a NuGet package

```bash
dotnet pack .template.config/templatepack.csproj -o artifacts
dotnet new install artifacts/Ardent.CleanApi.Template.1.0.0.nupkg
```

Bump `PackageVersion` in `.template.config/templatepack.csproj` for each release. Publish the `.nupkg` to your internal feed (`dotnet nuget push`) so other machines can run `dotnet new install Ardent.CleanApi.Template`.

## Things to keep in sync

- Package versions: `Directory.Packages.props` only. Check for updates with `dotnet list package --outdated`.
- Excludes: if you add local-only files, add them to both `template.json` (`sources[0].exclude`) and `templatepack.csproj` (`Content Exclude`).
- Harness formats: re-check `.claude/` files against the Claude Code docs (memory, skills, sub-agents, hooks, settings) when upgrading Claude Code, and run `claude plugin validate .claude/skills` and `claude plugin validate .claude/agents`.
