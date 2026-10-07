---
name: commit
description: Write a Conventional Commits message from the staged diff, show it, and commit after confirmation. Use when the user says "commit", "commit this" or "write a commit message".
disable-model-invocation: true
allowed-tools: Bash(git status *) Bash(git diff *) Bash(git log *) Bash(git commit *) PowerShell(git status *) PowerShell(git diff *) PowerShell(git log *) PowerShell(git commit *)
---

# Commit

Staged changes:

!`git diff --cached --stat`

!`git diff --cached`

Recent messages (match their style):

!`git log --oneline -10 --all`

## Steps

1. If nothing is staged, show `git status --short` and ask what to stage. Do not stage files yourself unless the user says which ones.
2. Check the staged set: if source under `src/*.Api` changed but `docs/openapi.json` is not staged and `git status` shows it modified, warn that the contract should be committed too. If `/verify` has not run since the last change, suggest running it first.
3. Write the message:
   - Header `<type>(<scope>): <summary>`, at most 72 characters, imperative mood, no full stop. Types: `feat`, `fix`, `refactor`, `test`, `docs`, `build`, `ci`, `chore`, `perf`. Scope = feature or layer (`products`, `api`, `infra`, `harness`).
   - Blank line, then a body explaining **why** and anything non-obvious, wrapped at 72 characters. Skip the body for trivial changes.
   - `BREAKING CHANGE:` footer if the HTTP contract or schema changed incompatibly.
4. Show the message in a code block and ask the user to confirm or edit.
5. After confirmation, commit with `git commit -F -` (message on stdin) so formatting is preserved, then show `git log --oneline -1`.

Never use `--no-verify`, `--amend` or `git push` unless the user explicitly asks.
