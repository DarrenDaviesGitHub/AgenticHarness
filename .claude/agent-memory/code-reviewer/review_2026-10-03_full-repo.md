---
name: review-2026-10-03-full-repo
description: Recurring weak spots found in the first full-repo review (2026-10-03); check these first in later diffs
metadata:
  type: project
---

Codebase is conformant on the mechanical checklist (no `var`, sealed, primary ctors, tokens passed, domain constants reused). Weak spots are behavioural:

- Unique-index race (SKU) is never translated to 409: handlers do check-then-save and nothing maps `DbUpdateException`. Look for the same in any new uniqueness rule.
- Paging math `(Page - 1) * PageSize` is unchecked int and `Page` has no upper bound.
- `LoggingBehaviour` logs "Handled" in `finally`, so failures also say "Handled".
- `GlobalExceptionHandler` treats `OperationCanceledException` as a 500 + error log.
- `ApiClient.ShouldBeProblemAsync` uses `ContentType?.MediaType.ShouldBe(...)`, which passes silently when the header is missing.
- Create/Update validators duplicate the same rules and SKU regex (domain does not enforce the charset).
- Hooks `protect-files.ps1` and `guard-bash.ps1` fail open on any internal error.

- Re-verified same day on a second full pass: all items above still present, no new ones. Missing-test gaps: no handler/integration test for race-to-409, none for page overflow.

**How to apply:** when a diff adds a uniqueness rule, paging, a pipeline behaviour or a problem-details test helper, re-check these.
