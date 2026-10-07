---
name: overview
description: Onboarding briefing for this repository - purpose, stack, how to run and test it, key commands, recent commits and open TODOs. Read-only. Use when someone is new to the repo or asks "what is this project?", "get me up to speed", "how do I run this?".
allowed-tools: Read Grep Glob Bash(git log *) Bash(git status *) Bash(git branch *) PowerShell(git log *) PowerShell(git status *) PowerShell(git branch *)
---

# Overview

Recent history:

!`git log --oneline -15 --all`

Working tree:

!`git status --short`

Read-only: do not edit files or run builds.

1. Read `README.md`, `CLAUDE.md`, `docs/adr/` (titles and decisions) and `Directory.Packages.props` (versions).
2. Search for open work with Grep: `TODO|FIXME|HACK` in `src/` and `tests/`.
3. Write the briefing:
   - **What it is**: purpose in two sentences, and that `Products` is the reference slice.
   - **Stack**: framework and key packages with versions.
   - **Run it**: prerequisites (.NET 10 SDK, pwsh for hooks), `dotnet run` command, Scalar URL, `.http` file, MediatR licence key setup.
   - **Key commands**: build, test, format, migrations (from `CLAUDE.md`).
   - **Architecture in one picture**: the four layers and the request flow, in a few lines.
   - **Claude Code harness**: the agents and skills available, and that hooks guard secrets, migrations and builds.
   - **Recent activity**: summarise the commits above (or "no commits yet") and uncommitted changes.
   - **Open TODOs**: `file:line` list, or "none".
   - **Suggested first steps** for a newcomer.

Keep it under one screen; link files as paths.
