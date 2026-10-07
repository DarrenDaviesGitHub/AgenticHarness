---
name: project-structure
description: Explain the folder layout, the four layers, the dependency rule and where new code belongs, using the live file list. Read-only. Use when asked "where does X go?", "how is this organised?", "what is in each project?" or when unsure where to put a file.
argument-hint: [question, e.g. "where do I put an email sender?"]
allowed-tools: Read Grep Glob Bash(git ls-files *) PowerShell(git ls-files *)
---

# Project structure

Tracked files right now:

!`git ls-files -- src tests docs .claude .github`

Read-only: do not edit files.

1. Group the files above by project and folder. Read `CLAUDE.md` (architecture table) and the `.csproj` files for the actual project references.
2. Explain each layer in two or three sentences: what lives there, what it may reference, and the example files from the list.
3. Explain the dependency rule and that `tests/*ArchitectureTests` enforces it.
4. If the user asked a question (`$ARGUMENTS`), answer it concretely: the exact folder and file name to create, the Products file to copy, and why it belongs in that layer. Typical answers:
   - New business rule -> the entity in Domain.
   - New use case -> `Application/<Feature>/<UseCase>/` (record, handler, validator).
   - Talking to an external system (email, HTTP API, queue) -> interface in `Application/Common/Interfaces`, implementation in Infrastructure, registration in `AddInfrastructure()`.
   - New endpoint -> controller in Api plus request records in `Contracts/`.
   - Cross-cutting request concern -> a MediatR behaviour in `Application/Common/Behaviours`.

Keep the answer short; link files as `path` so they are clickable. Mention untracked new files only if `git status --short` shows them and they matter to the question.
