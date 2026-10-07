---
name: code-cleanup
description: Behaviour-preserving cleanup of a file or folder - dead code, naming, explicit types, sealing, simplification - with tests run before and after. Use when the user asks to "clean up", "tidy" or "simplify" specific code.
argument-hint: <path>
disable-model-invocation: true
allowed-tools: Bash(dotnet build *) Bash(dotnet test *) Bash(dotnet format *) Bash(git diff *) Bash(git status *) PowerShell(dotnet build *) PowerShell(dotnet test *) PowerShell(dotnet format *) PowerShell(git diff *) PowerShell(git status *)
---

# Clean up: $ARGUMENTS

Behaviour must not change. Public API, HTTP contract, database schema and test expectations stay exactly the same.

1. If `$ARGUMENTS` is empty, ask which path to clean. Read every file under it plus `CLAUDE.md` and the relevant `.claude/rules/*.md`.
2. **Baseline**: run `dotnet test` and record the result. If it is not green, stop and report; do not clean up on a red baseline.
3. Make only these kinds of changes:
   - Remove dead code: unused private members, parameters, usings, unreachable branches, commented-out code.
   - Rename for clarity where names mislead (private and internal symbols only).
   - Apply conventions: explicit types, `sealed`, file-scoped namespaces, primary constructors, collection expressions, guard clauses instead of nesting.
   - Simplify: inline needless variables or indirection, replace hand-written loops with clearer LINQ (or the reverse) when it reads better.
   - Remove abstractions with a single implementation and no test seam, only inside the given path.
4. Do not reformat files you did not otherwise change, and do not touch migrations or `docs/openapi.json`.
5. **After**: `dotnet build -warnaserror`, `dotnet format --verify-no-changes`, `dotnet test`. Results must match the baseline.

## Report

A table of `file:line | change | why`, then the before/after test counts. If something looked worth changing but would alter behaviour, list it separately as a suggestion instead of doing it.
