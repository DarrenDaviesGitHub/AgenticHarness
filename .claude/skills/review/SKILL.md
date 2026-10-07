---
name: review
description: Multi-agent review of the current changes - runs code-reviewer, architecture-reviewer and security-reviewer in parallel, then merges and de-duplicates their findings into one prioritised list. Use when the user asks to "review my changes" or before opening a pull request.
disable-model-invocation: true
allowed-tools: Bash(git status *) Bash(git diff *) Bash(git log *) PowerShell(git status *) PowerShell(git diff *) PowerShell(git log *)
---

# Review the current changes

Changed files:

!`git status --short`

Diff size (unstaged, then staged):

!`git diff --stat`

!`git diff --cached --stat`

## Steps

1. If nothing changed, say so in one line and stop. If the user named a branch or commit range, review that instead and pass it to every agent.
2. Launch these three subagents **in parallel in a single message**, giving each the same scope (files above, or the range the user named) and asking for its standard output contract:
   - `code-reviewer`
   - `architecture-reviewer`
   - `security-reviewer`
3. Merge their findings:
   - De-duplicate: findings on the same `file:line` about the same problem become one entry; keep the clearest explanation and note which reviewers raised it.
   - Resolve conflicts: if reviewers disagree, prefer the option that keeps the code simpler and consistent with `CLAUDE.md`, and say why.
   - Drop findings that only restate a convention the code already follows, or that would add complexity the template avoids.

## Output

One list ordered **Critical**, then **Warning**, then **Suggestion**. Each entry: `file:line` - problem - why it matters - fix - (reviewers). End with a one-line verdict: "Ready to commit", "Fix criticals first", or "Needs rework". If all three reviewers found nothing, reply with one line saying so.

Do not apply fixes unless the user asks.
