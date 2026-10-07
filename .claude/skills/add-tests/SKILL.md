---
name: add-tests
description: Find and fill test gaps for a class, feature or endpoint - missing invariants, validator boundaries, handler failure paths and HTTP contract rows. Use when asked to "add tests", "improve coverage", "test X" or after a feature lands without enough tests.
argument-hint: <class, feature or endpoint>
context: fork
agent: qa-engineer
---

# Add tests for: $ARGUMENTS

1. Locate the target with Grep/Glob (source file, its folder, and the endpoints it serves). If `$ARGUMENTS` is empty, use the files changed in `git status --short`.
2. Read the existing tests for it and `.claude/rules/testing.md`.
3. List the gaps: domain invariants, validator rules and boundaries, handler happy/not-found/conflict/no-save-on-failure paths, and every HTTP contract row for the endpoints (see `CLAUDE.md`).
4. Write the missing tests in the mirrored folder of the right test project, reusing `ProductFactory`, `TestData`, `ApiFactory` and `ApiClient` patterns.
5. Run `dotnet test --project <each test project you touched>` until green. A test that exposes a real bug stays and is reported; never weaken it.
6. Report using your output contract: tests added (`file:line - name`) and remaining gaps by severity.
