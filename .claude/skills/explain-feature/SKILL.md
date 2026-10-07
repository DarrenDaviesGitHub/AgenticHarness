---
name: explain-feature
description: Trace one feature or request end to end - route, controller, MediatR behaviours, handler, repository, EF Core, response and error paths - with file links and a Mermaid sequence diagram. Read-only. Use when asked "how does X work?", "walk me through Y", or "what happens when I call Z?".
argument-hint: <feature, endpoint or use case>
allowed-tools: Read Grep Glob Bash(git log *) Bash(git ls-files *) PowerShell(git log *) PowerShell(git ls-files *)
---

# Explain: $ARGUMENTS

This is read-only: do not edit files or run builds.

1. If `$ARGUMENTS` is empty, ask which feature, endpoint or use case to explain. Otherwise find the entry point with Grep (route template, controller action, or command/query name).
2. Trace the request in order and read each file:
   - Route and controller action (`src/*.Api/Controllers/`), request record and `ToCommand`.
   - MediatR pipeline: `LoggingBehaviour` then `ValidationBehaviour` (and which validator runs).
   - Handler, the domain methods it calls, and the invariants they enforce.
   - Repository method and the EF Core query or save it performs (`Persistence/`), including indexes that matter.
   - Response mapping and the HTTP status returned.
   - Error paths: which exception each failure throws and how `GlobalExceptionHandler` maps it.
3. Check the tests that cover it (unit and integration) and note gaps.

## Output

1. A one-paragraph summary.
2. A numbered walkthrough, each step with a `path/File.cs:line` link and one or two sentences.
3. A Mermaid `sequenceDiagram` with participants Client, Controller, Behaviours, Handler, Domain, Repository, Database, including the main error branch as an `alt` block.
4. "Tests covering this" and "Not covered" lists.
