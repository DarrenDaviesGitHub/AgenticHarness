---
name: verify
description: Verify the solution is ready to commit - build with warnings as errors, format check, all tests, and OpenAPI contract up to date. Use before every commit, after finishing a feature or fix, or when asked "is it green?", "run the checks", "verify".
allowed-tools: Bash(dotnet build *) Bash(dotnet format *) Bash(dotnet test *) Bash(git status *) Bash(git diff *) PowerShell(dotnet build *) PowerShell(dotnet format *) PowerShell(dotnet test *) PowerShell(git status *) PowerShell(git diff *)
---

# Verify

Run these steps in order from the repository root. Run every step even if an earlier one fails, so the report is complete.

1. **Build** - `dotnet build -warnaserror`
   This also regenerates `docs/openapi.json`.
2. **Format** - `dotnet format --verify-no-changes`
   If it fails, run `dotnet format`, then re-run the check and mention that files were reformatted.
3. **Tests** - `dotnet test`
4. **OpenAPI contract** - `git status --porcelain -- docs/openapi.json`
   - Empty output: the committed contract matches the code. Pass.
   - Any output: the build changed the contract. That is expected when endpoints or contracts changed; report it as **needs staging** and show `git diff --stat -- docs/openapi.json`. It is a failure only if no API code changed (then something unintended altered the contract).

## Report

Reply with a table, then details only for failures:

| Step | Result |
| --- | --- |
| Build (-warnaserror) | pass / fail |
| Format | pass / fixed / fail |
| Tests | pass (N passed) / fail (N failed) |
| OpenAPI contract | up to date / needs staging / unexpected change |

For each failure, list the first errors with `file:line` and a one-line diagnosis. Do not fix failures unless the user asked you to; if you are verifying your own work, fix them and run `/verify` again.
