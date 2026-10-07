---
name: test-me
description: Quiz me on this codebase - five questions, one at a time, mixed difficulty across architecture, request flow, conventions and testing, each graded with file references, then a score. Use when the user says "test me", "quiz me" or wants to check their understanding.
argument-hint: [topic, e.g. "validation" or "testing"]
disable-model-invocation: true
allowed-tools: Read Grep Glob
---

# Codebase quiz

Topic focus: $ARGUMENTS (if empty, mix all topics).

Read-only: do not edit files.

## Prepare (silently)

Read `CLAUDE.md`, the `.claude/rules/*.md` files and the Products slice (`src/**/Products/**`, `GlobalExceptionHandler.cs`, `Program.cs`, the test projects). Write five questions **grounded in this repo's actual code**, not general .NET trivia:

- 2 easy (e.g. "Which layer may reference EF Core?"), 2 medium (e.g. "What status and body does a duplicate SKU return, and which file decides it?"), 1 hard (e.g. "Why does `Product.Update` validate into a candidate before assigning?").
- Cover at least three of: architecture/dependency rule, request flow through MediatR, conventions, error handling, persistence, testing approach.
- For each, know the correct answer and the `file:line` that proves it.

## Run the quiz

For each question, one at a time:

1. Ask the question (state its number and difficulty). Then **stop and wait** for the answer. Never ask the next question in the same message.
2. Grade the answer: Correct, Partly correct, or Incorrect.
3. Explain the right answer in two to four sentences with `path/File.cs:line` references the user can open.

## Finish

Give the score (Correct = 1, Partly = 0.5) out of 5, one sentence on strengths, and a short "revisit" list of files or rules to read, based on what they missed.
