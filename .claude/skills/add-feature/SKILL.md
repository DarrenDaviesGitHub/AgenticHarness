---
name: add-feature
description: Build a complete vertical slice for a new endpoint or feature - domain change, command/query + handler + validator, request/response records, repository method, EF configuration and migration, controller action, unit and integration tests, regenerated OpenAPI. Use when asked to "add an endpoint", "add a feature", "create CRUD for X" or "add a use case".
argument-hint: <feature description>
allowed-tools: Bash(dotnet build *) Bash(dotnet test *) Bash(dotnet format *) Bash(dotnet ef migrations add *) Bash(git status *) Bash(git diff *) PowerShell(dotnet build *) PowerShell(dotnet test *) PowerShell(dotnet format *) PowerShell(dotnet ef migrations add *) PowerShell(git status *) PowerShell(git diff *)
---

# Add feature: $ARGUMENTS

The `Products` slice is the canonical example. **Read the matching Products file before writing each new file and copy its shape**; never invent a new pattern. Detailed per-layer rules load automatically from `.claude/rules/`.

## 0. Clarify first

If the description leaves any of these open, ask (one AskUserQuestion batch) before writing code: entity name and fields with types and limits, uniqueness rules, which operations (list/get/create/update/delete or a custom action), route, status codes, and whether it changes the database. Then state the plan as a short file list.

## 1. Build the slice (in this order)

Follow [checklist.md](checklist.md) for the file-by-file list and the Products file to copy for each step:

1. Domain entity or method + domain unit tests.
2. Application: one folder per use case (record, handler, validator), response record, repository interface method + application unit tests.
3. Infrastructure: repository implementation, `IEntityTypeConfiguration`, DI registration, then a migration with `/add-migration <Name>` if the schema changed.
4. Api: request records in `Contracts/`, controller action(s) with XML docs and `ProducesResponseType<T>`, requests appended to the `.http` file.
5. Integration tests: one per HTTP contract row (success and every error status).
6. Add the new rows to the HTTP contract table in `CLAUDE.md`.

Keep it minimal: no new abstractions, base classes or packages unless the user asked for them.

## 2. Verify and review

1. Run `/verify` and fix everything until it passes. `docs/openapi.json` will change: that is expected; it must be committed with the code.
2. Ask the `code-reviewer` and `architecture-reviewer` subagents (in parallel) to review the diff. Fix every Critical and Warning finding, or explain why not.
3. Run `/verify` again if you changed anything.

## 3. Report

List the files added or changed by layer, the new endpoints with status codes, the tests added, and any follow-ups (e.g. "add authorization policy").
