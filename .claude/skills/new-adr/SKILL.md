---
name: new-adr
description: Create the next numbered Architecture Decision Record in docs/adr using the house template. Use when a significant or hard-to-reverse decision is made - a new package, pattern, layer change, persistence or security choice - or when asked to "write an ADR" or "record this decision".
argument-hint: <decision title>
allowed-tools: Read Glob Write Bash(git config user.name) PowerShell(git config user.name)
---

# New ADR: $ARGUMENTS

1. If `$ARGUMENTS` is empty, ask for the decision title (short, imperative: "Use Redis for output caching").
2. Glob `docs/adr/[0-9][0-9][0-9][0-9]-*.md` and take the highest number + 1, zero-padded to four digits.
3. File name: `docs/adr/<NNNN>-<title-in-kebab-case>.md`.
4. Copy `docs/adr/template.md` exactly and fill it in:
   - Status `Accepted` unless the user says it is a proposal; today's date; author from `git config user.name`.
   - **Context**: the forces and constraints, from the conversation and the code (link files).
   - **Decision**: one paragraph in active voice ("We will ...").
   - **Consequences**: positive, negative, and what would make us revisit it.
   - **Alternatives considered**: each with one line on why it was not chosen.
5. Keep it under about 40 lines; an ADR records a decision, it is not a design document. If anything essential is unknown, ask instead of inventing it.
6. If the decision supersedes an older ADR, set the old one's status to `Superseded by <NNNN>` and link both ways.

Report the file path and a one-line summary.
