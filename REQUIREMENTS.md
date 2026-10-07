# .NET 10 Template – Claude Code Build Prompt

Oct 3, 2026 · @Ardent

## How to use this prompt

Everything from **Role and goal** down is the prompt. Export the doc as Markdown, save it in an empty folder as `TEMPLATE_PROMPT.md`, start `claude` there, switch to plan mode (Shift+Tab), and send: *"Read TEMPLATE\_PROMPT.md and follow it."*

- Claude Code will ask you a few setup questions first (Phase 0), then present a plan for approval before writing anything.
- Expect it to work in phases and run the build and tests at the end of each phase. Review each phase before saying "continue".
- The finished result is a folder you install once with `dotnet new install .` and then reuse with `dotnet new <your-short-name> -n Acme.Orders`.

**Two things worth knowing before you start (checked October 2026):**

1. **MediatR is no longer plain open source.** Version 13+ ships under a dual RPL-1.5 / commercial licence from Lucky Penny Software. It is free under the Community edition for companies under $5M gross annual revenue, but you need to register for a licence key; without one it still runs and only logs warnings. The prompt keeps MediatR as you asked and wires the key from configuration so it is never committed. ([source](https://www.jimmybogard.com/automapper-and-mediatr-commercial-editions-launch-today/), [source](https://startdebugging.net/2026/05/mediatr-vs-plain-service-classes-in-2026/))
2. **FluentAssertions went commercial too**, so the prompt uses Shouldly for test assertions instead.

## Role and goal

You are a senior .NET architect and Claude Code power user. Build a reusable **`dotnet new` project template** for an ASP.NET Core Web API on **.NET 10 (LTS) / C# 14** that I will use as the starting point for every new project.

The template has two equal halves, and both must be production quality:

1. **A clean, minimal .NET solution** that demonstrates Clean Architecture, CQRS with MediatR, FluentValidation, records for DTOs, OpenAPI, correct HTTP semantics and centralised exception handling, through **one** sample feature (`Products` CRUD). It must be easy to read in an afternoon. No speculative abstractions.
2. **A correctly harnessed Claude Code setup** committed in the repo: `CLAUDE.md`, path-scoped rules, settings and permissions, subagents, skills and hooks, so that any future project generated from this template is immediately productive and safe to work on with Claude Code.

### Working rules for this task

- **Plan first.** Stay in plan mode for Phase 0. Do not write files until I approve the plan.
- **Work in phases** (listed below). At the end of each phase run `dotnet build` and `dotnet test`, fix all errors and warnings, show me a short summary, and wait for "continue".
- **Verify, don't recall.** Package versions and Claude Code file formats change often. Look up the latest stable NuGet versions (`dotnet package search <name>` or nuget.org) and check the current Claude Code docs at `https://code.claude.com/docs` (pages: memory, skills, sub-agents, hooks, settings) before writing those files. Never invent frontmatter fields or package APIs.
- **Simplicity wins ties.** If two approaches are equally correct, choose the one with fewer files and concepts. Every file must earn its place.
- **Explain non-obvious choices** in a one-line code comment or an ADR, never in long comment blocks.
- Use `TodoWrite` / the task list to track phases so I can see progress.

## Phases and Phase 0 (discovery)

The work runs in seven phases:

1. **Phase 0** – Ask setup questions, then present the plan (file tree + key decisions) for approval.
2. **Phase 1** – Solution skeleton, build configuration, code style enforcement.
3. **Phase 2** – Domain, Application, Infrastructure and Api layers with the `Products` feature.
4. **Phase 3** – Test projects (unit, integration, architecture).
5. **Phase 4** – Claude Code harness: `CLAUDE.md`, rules, settings, agents, skills, hooks.
6. **Phase 5** – Package as a `dotnet new` template.
7. **Phase 6** – End-to-end verification from a freshly generated project.

### Ask me these before planning (use AskUserQuestion, one batch)

- My operating system(s) and shell (decides whether hooks are Bash, PowerShell or both).
- Root namespace / company prefix for the template's placeholder (default `Ardent.Project`).
- Template short name (default `cleanapi`) and author name.
- Database for the sample: **SQLite** (default, zero setup) or **PostgreSQL** (via Docker / Testcontainers).
- Whether I have a MediatR licence key (Community or paid) to configure.
- CI provider: GitHub Actions (default), Azure DevOps, or none.

Then present the plan: the full file tree, the NuGet packages with versions you verified, and any place where you recommend deviating from this prompt (with one sentence of reasoning each).

## Phase 1 – Solution structure, build configuration and conventions

Use this layout. `Ardent.Project` is the template placeholder that `dotnet new -n` replaces.

```text
Ardent.Project/
├── .claude/                      # Phase 4
├── .config/dotnet-tools.json     # local tools: dotnet-ef
├── .github/workflows/ci.yml      # build, format check, test
├── .template.config/template.json
├── docs/adr/                     # 3–4 short ADRs
├── src/
│   ├── Ardent.Project.Domain/
│   ├── Ardent.Project.Application/
│   ├── Ardent.Project.Infrastructure/
│   └── Ardent.Project.Api/
├── tests/
│   ├── Ardent.Project.Domain.UnitTests/
│   ├── Ardent.Project.Application.UnitTests/
│   ├── Ardent.Project.Api.IntegrationTests/
│   └── Ardent.Project.ArchitectureTests/
├── .editorconfig
├── .gitignore
├── CLAUDE.md
├── Ardent.Project.slnx
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
└── README.md
```

**Dependency rule (enforced by architecture tests):** Domain depends on nothing. Application depends on Domain. Infrastructure depends on Application. Api depends on Application and references Infrastructure only for composition-root registration.

### Build configuration

- **`.slnx`** solution format (the .NET 10 default), not `.sln`.
- **`global.json`** pins the .NET 10 SDK (`rollForward: latestFeature`) and opts `dotnet test` into Microsoft.Testing.Platform.
- **`Directory.Build.props`**: `TargetFramework net10.0`, `Nullable enable`, `ImplicitUsings enable`, `TreatWarningsAsErrors true`, `AnalysisLevel latest-recommended`, `EnforceCodeStyleInBuild true`, `GenerateDocumentationFile true` (for OpenAPI descriptions; suppress CS1591 in test projects only).
- **`Directory.Packages.props`**: Central Package Management for every package version. No versions in `.csproj` files.
- **`.editorconfig`** enforces the conventions below as build errors where the rule exists, so the compiler, `dotnet format` and Claude all agree.

### C# conventions

- **Explicit types everywhere.** No `var` (`csharp_style_var_* = false:error`). Target-typed `new()` is allowed when the type is on the left.
- **File-scoped namespaces**, one type per file, namespace matches folder.
- **Primary constructors** for dependency injection in handlers, services and controllers. Never reassign primary-constructor parameters.
- **Records** for DTOs, requests, responses, commands and queries. Use `required` / `init` where it clarifies intent.
- **`sealed` by default** for classes and records unless designed for inheritance.
- Use modern features where they make code clearer, not as decoration: collection expressions, C# 14 extension blocks (e.g. `AddApplication()` / `AddInfrastructure()` DI registration), the `field` keyword for guarded domain properties, null-conditional assignment, pattern matching.
- `async` all the way with `CancellationToken` passed to every async call; `Async` suffix on async methods.
- Inject `TimeProvider` instead of `DateTime.UtcNow`; generate IDs with `Guid.CreateVersion7()`.
- **No AutoMapper.** Map explicitly with small static `ToResponse()` / `FromRequest()` methods next to the DTO.
- Guard clauses over nested `if`s; early returns; methods short enough to read without scrolling.

## Phase 2a – The sample feature: Products CRUD

One aggregate, one controller, five endpoints. It exists to show the pattern every future feature will copy, so make it exemplary rather than large.

### Domain

- `Product` entity: `Id` (Guid v7), `Name`, `Description?`, `Sku` (unique), `Price` (decimal, > 0), `CreatedAt`, `UpdatedAt?`.
- Private setters; created only through `Product.Create(...)`; changed only through `product.Update(...)`. Invariants enforced inside the entity, throwing a `DomainException`.
- No framework references at all in this project.

### Application (folder per feature, one use case per folder)

```text
Application/
├── Common/
│   ├── Behaviours/        # ValidationBehaviour, LoggingBehaviour
│   ├── Exceptions/        # NotFoundException, ConflictException
│   ├── Interfaces/        # IProductRepository, IUnitOfWork
│   └── Models/            # PagedResponse<T>
├── Products/
│   ├── CreateProduct/     # Command, Handler, Validator
│   ├── UpdateProduct/
│   ├── DeleteProduct/
│   ├── GetProductById/    # Query, Handler
│   ├── ListProducts/      # Query (page, pageSize), Handler, Validator
│   └── ProductResponse.cs # record + static mapping from Product
└── DependencyInjection.cs
```

- Commands and queries are `sealed record`s implementing `IRequest<T>`.
- Use a small `IProductRepository` + `IUnitOfWork` so Application has **no EF Core reference** (record the trade-off in an ADR).
- Handlers throw `NotFoundException` / `ConflictException`; they never return null or HTTP concepts.

### Api

- `ProductsController : ControllerBase` with `[ApiController]`, `[Route("api/products")]`, `ISender` injected via primary constructor. Controllers stay thin: map request → command, send, map result → HTTP.
- Separate request records (`CreateProductRequest`, `UpdateProductRequest`) from commands so the HTTP contract can evolve independently.
- Annotate every action with generic `[ProducesResponseType<T>(...)]` attributes and XML doc comments so the OpenAPI document is complete.

### HTTP contract

| Operation | Route | Success | Error responses (ProblemDetails) |
| --- | --- | --- | --- |
| List (paged) | `GET /api/products?page=1&pageSize=20` | 200 + `PagedResponse<ProductResponse>` | 400 invalid paging |
| Get by id | `GET /api/products/{id:guid}` | 200 + `ProductResponse` | 404 |
| Create | `POST /api/products` | 201 + `Location` header (`CreatedAtAction`) + body | 400 validation, 409 duplicate SKU |
| Update | `PUT /api/products/{id:guid}` | 204 No Content | 400, 404, 409 |
| Delete | `DELETE /api/products/{id:guid}` | 204 No Content | 404 |

Any unhandled exception returns 500 ProblemDetails with a `traceId` and no stack trace outside Development.

## Phase 2b – Cross-cutting concerns

### MediatR pipeline

- Register handlers from the Application assembly. Read the licence key from configuration (`MediatR:LicenseKey`), supplied via user-secrets locally and an environment variable in CI/production. Never commit it; document it in the README.
- `ValidationBehaviour<TRequest, TResponse>`: runs all FluentValidation validators for the request and throws `ValidationException` with every failure.
- `LoggingBehaviour`: logs request name and elapsed time with structured logging (`ILogger`, message templates, no string interpolation).

### FluentValidation

- One validator per command/query that takes input, registered with `AddValidatorsFromAssembly`.
- Validate through the MediatR behaviour only. Do **not** use the deprecated `FluentValidation.AspNetCore` auto-validation.

### Exception handling

Use the built-in exception handler middleware with `IExceptionHandler` (the modern replacement for hand-written exception middleware) plus `AddProblemDetails()`, so every error is RFC 9457 ProblemDetails.

| Exception | Status | Body |
| --- | --- | --- |
| `ValidationException` (FluentValidation) | 400 | `ValidationProblemDetails` with field → messages |
| `DomainException` | 400 | ProblemDetails with the rule that was broken |
| `NotFoundException` | 404 | ProblemDetails |
| `ConflictException` | 409 | ProblemDetails |
| Anything else | 500 | Generic ProblemDetails; log the exception; detail only in Development |

- Use `CustomizeProblemDetails` to add `traceId` and `instance` to every response.
- Keep `[ApiController]` automatic 400s for malformed JSON / binding errors; confirm their shape matches.

### OpenAPI

- Built-in `Microsoft.AspNetCore.OpenApi` (`AddOpenApi` / `MapOpenApi`), OpenAPI 3.1, with a document transformer for title, version and description. No Swashbuckle.
- Scalar (`Scalar.AspNetCore`) as the interactive UI, Development only.
- Generate `docs/openapi.json` at build time and commit it, so contract changes show up in diffs and Claude can read the API surface without running the app.

### Persistence (Infrastructure)

- EF Core 10 `ApplicationDbContext`, one `IEntityTypeConfiguration<T>` per entity (unique index on `Sku`, decimal precision on `Price`).
- Repository and unit-of-work implementations. Migrations live in Infrastructure; create the initial migration with the local `dotnet-ef` tool.
- Apply migrations on startup in Development only.

### Api host

- `Program.cs` stays short (aim for under 40 lines) by delegating to `AddApplication()`, `AddInfrastructure(configuration)` and `AddPresentation()` extensions.
- Options classes bound with `ValidateDataAnnotations().ValidateOnStart()`.
- `/health` endpoint including a database check. HTTPS redirection on.

## Phase 3 – Tests

Stack: **xUnit v3** on Microsoft.Testing.Platform, **Shouldly** for assertions, **NSubstitute** for test doubles. Verify each is current and actively maintained before adding it.

| Project | What it proves | Approach |
| --- | --- | --- |
| `Domain.UnitTests` | Entity invariants and behaviour | Pure unit tests, no mocks |
| `Application.UnitTests` | Handlers and validators | Substitute repositories; one test class per handler and validator |
| `Api.IntegrationTests` | Every row of the HTTP contract table, ProblemDetails shapes, OpenAPI document is served | `WebApplicationFactory<Program>` with an isolated database per test class (SQLite in-memory, or Testcontainers if PostgreSQL was chosen) |
| `ArchitectureTests` | The dependency rule and conventions | NetArchTest or ArchUnitNET (pick the better-maintained one) |

**Architecture rules to encode:** Domain references no other project and no `Microsoft.*` packages; Application does not reference Infrastructure, Api or EF Core; handlers and validators are `sealed`; commands, queries and DTOs are records; every command has a validator; controllers exist only in Api.

**Test conventions:** names follow `Method_Scenario_ExpectedResult`; Arrange / Act / Assert with blank lines between; one behaviour per test; no logic (loops, ifs) in tests; use builders or static factory helpers for test data rather than shared mutable fixtures.

## Phase 4a – Claude Code harness: memory, rules and settings

Principle: **CLAUDE.md and rules guide; hooks and permissions enforce.** Anything that must happen every time goes in a hook or a permission rule, not in prose.

```text
.claude/
├── settings.json          # permissions, env, hooks (committed)
├── rules/                 # path-scoped conventions
│   ├── domain.md          # paths: src/**/*.Domain/**
│   ├── application.md     # paths: src/**/*.Application/**
│   ├── infrastructure.md  # paths: src/**/*.Infrastructure/**
│   ├── api.md             # paths: src/**/*.Api/**
│   └── testing.md         # paths: tests/**
├── agents/                # Phase 4b
├── skills/                # Phase 4c
└── hooks/                 # Phase 4d scripts
```

### CLAUDE.md (repo root, under 150 lines)

Only facts Claude needs in every session, written as concrete, checkable statements:

- One-paragraph project summary and the tech stack with versions.
- **Commands:** build, test (all / one project / one test), `dotnet format`, run the API, add and apply a migration, regenerate `docs/openapi.json`.
- **Architecture map:** the four layers, what lives where, the dependency rule, and where a new feature's files go.
- **Non-negotiable conventions** in one short list (explicit types, records for DTOs, sealed, file-scoped namespaces, CancellationToken, ProblemDetails). Detailed per-layer guidance lives in `.claude/rules/`, not here.
- **Workflow:** use `/add-feature` for new endpoints; run `/verify` before committing; never hand-edit migrations or `docs/openapi.json`.
- **Definition of done** for any change (builds with zero warnings, tests pass, format clean, OpenAPI updated, tests added for new behaviour).
- **Gotchas** specific to this repo (e.g. MediatR licence key location, SQLite limitations).

Don't put directory listings, dependency lists or tutorials in CLAUDE.md; Claude can read those from the code. Use `<!-- -->` comments for maintainer notes (they are stripped from context).

### Path-scoped rules (`.claude/rules/*.md`)

Each file starts with `paths:` frontmatter so it loads only when Claude touches matching files. Keep each under 40 lines of imperative, checkable rules (e.g. Domain: "no public setters", "no framework references"; Api: "controllers only map and send, no business logic", "every action has ProducesResponseType attributes"; Testing: naming and AAA conventions).

### settings.json

- **allow:** `dotnet build`, `dotnet test`, `dotnet format`, `dotnet restore`, `dotnet ef migrations add`, read-only git (`status`, `diff`, `log`, `branch`, `show`).
- **ask:** `git commit`, `git push`, `dotnet add package`, `dotnet ef database update`.
- **deny:** reading `.env*`, `**/secrets.json`, `*.pfx`/`*.key`; `rm -rf`, `git push --force`, `git reset --hard`, `dotnet ef database drop`, piping downloads into a shell.
- **env:** `DOTNET_CLI_TELEMETRY_OPTOUT=1`, `DOTNET_NOLOGO=1`.
- Add `.claude/settings.local.json` and `CLAUDE.local.md` to `.gitignore` for personal overrides.

### Optional: live docs

Add a project `.mcp.json` with the Microsoft Learn MCP server (verify the current endpoint) so Claude can check .NET and ASP.NET Core docs instead of relying on memory.

## Phase 4b – Subagents (`.claude/agents/`)

Each agent is one focused job with the minimum tools it needs. Descriptions are short and say exactly when to delegate (include "use proactively" where appropriate).

| Agent | Job | Tools | Model | Can edit? |
| --- | --- | --- | --- | --- |
| `code-reviewer` | Readability, naming, duplication, conventions from CLAUDE.md and rules, async/cancellation, nullability | Read, Grep, Glob, Bash | sonnet | No |
| `architecture-reviewer` | Dependency rule, layer placement, CQRS boundaries, coupling, unjustified abstractions or over-engineering | Read, Grep, Glob, Bash | opus | No |
| `security-reviewer` | OWASP API Top 10: validation gaps, over-posting, injection, secrets, error/PII leakage, where auth must plug in, `dotnet list package --vulnerable --include-transitive` | Read, Grep, Glob, Bash | sonnet | No |
| `qa-engineer` | Finds untested behaviour and contract rows, edge cases and flaky patterns; writes the missing tests | Read, Grep, Glob, Bash, Edit, Write | sonnet | Only under `tests/` |
| `debugger` | Root-causes build, test or runtime failures and applies the minimal fix with a regression test | Read, Grep, Glob, Bash, Edit | inherit | Yes |

**Requirements for every agent file:**

- Valid frontmatter (`name`, `description`, `tools`, `model`; verify field names against the current docs).
- Reviewers set `memory: project` and are told to check their memory for recurring issues before reviewing and record new patterns after.
- `qa-engineer` demonstrates an agent-scoped `PreToolUse` hook in its frontmatter that blocks `Edit`/`Write` outside `tests/`.
- The body states: what to read first (git diff, CLAUDE.md, relevant rules), a checklist specific to this codebase, and an **output contract**: findings grouped as Critical / Warning / Suggestion, each with `file:line`, why it matters and a concrete fix. If nothing is found, say so in one line; never pad.
- Reviewers judge against this repo's conventions, not generic preferences, and never suggest adding complexity the template deliberately avoids.

## Phase 4c – Skills (`.claude/skills/<name>/SKILL.md`)

Skills are the repeatable workflows; they load only when used, so they can be detailed without bloating every session. Skills that create code **point to the Products slice as the canonical example** instead of embedding code snippets, so they never drift from the real code.

| Skill | What it does | Who invokes |
| --- | --- | --- |
| `/add-feature <description>` | Builds a full vertical slice: domain change, command/query + handler + validator, request/response records, repository method, EF configuration and migration if needed, controller action with `ProducesResponseType`, unit and integration tests, regenerated `openapi.json`. Asks clarifying questions first if the description is ambiguous; ends by running `/verify` and asking `code-reviewer` and `architecture-reviewer` to check the diff | You or Claude |
| `/add-tests <target>` | Uses `qa-engineer` to find gaps for a class, feature or endpoint and writes tests following the test conventions | You or Claude |
| `/add-migration <Name>` | Runs `dotnet ef migrations add` with the right project and startup flags, shows the generated SQL script for review, never edits the migration by hand | You or Claude |
| `/code-cleanup <path>` | Behaviour-preserving cleanup: dead code, naming, explicit types, sealing, simplification. Runs tests before and after; reports what changed and why | You only |
| `/explain-feature <feature>` | Traces one request end to end (route → controller → MediatR behaviours → handler → repository → EF → response) with file links and a Mermaid sequence diagram. Read-only | You or Claude |
| `/project-structure` | Explains the folder layout, layers, dependency rule and "where does X go?", using live `git ls-files` output | You or Claude |
| `/overview` | Onboarding briefing: purpose, stack, how to run, key commands, recent commits (`git log`), open TODOs | You or Claude |
| `/test-me [topic]` | Quizzes me on this codebase: five questions, one at a time, mixed difficulty (architecture, request flow, conventions, testing). Waits for my answer, grades it, explains with file references, then gives a score and what to revisit | You only |
| `/review` | Runs `code-reviewer`, `architecture-reviewer` and `security-reviewer` in parallel on the current diff, then merges and de-duplicates findings into one prioritised list | You only |
| `/verify` | `dotnet build -warnaserror`, `dotnet format --verify-no-changes`, `dotnet test`, and checks `openapi.json` is up to date. Reports pass/fail per step | You or Claude |
| `/commit` | Writes a Conventional Commits message from the staged diff and commits after showing it to me | You only |
| `/new-adr <title>` | Creates the next numbered ADR in `docs/adr/` using the house template | You or Claude |

**Requirements for every skill:**

- `description` leads with the key use case and trigger phrases; keep the body under \~100 lines and move long checklists into supporting files in the skill folder.
- "You only" skills set `disable-model-invocation: true`. Leave `verify` model-invocable: recent Claude Code versions automatically run a project skill named `verify` before each commit.
- Use `$ARGUMENTS` for input, `` !`command` `` dynamic context for live data (git diff, git log, file lists), and `allowed-tools` to pre-approve only the exact commands a skill runs.
- Read-only skills (`explain-feature`, `project-structure`, `overview`, `test-me`) restrict themselves to Read, Grep, Glob and read-only git.

## Phase 4d – Hooks (configured in `.claude/settings.json`, scripts in `.claude/hooks/`)

Hooks are the deterministic guard rails. Keep them fast, quiet on success, and specific in their error messages so Claude can self-correct.

| Event | Matcher | Script | Behaviour |
| --- | --- | --- | --- |
| `SessionStart` | `startup\|resume\|compact` | `session-context` | Prints branch, count of uncommitted files, last 5 commits and the .NET SDK version into context, plus a one-line reminder to run `/verify` before committing |
| `PreToolUse` | `Edit\|Write` | `protect-files` | Blocks (exit 2, with reason) edits to `.env*`, existing files under `Migrations/`, `docs/openapi.json`, `bin/`, `obj/`, `.git/`, certificates and keys |
| `PreToolUse` | `Bash` | `guard-bash` | Backstop to the deny rules: blocks force-push, `reset --hard`, `rm -rf` outside `bin/`/`obj/`, `dotnet ef database drop`, and piping downloads into a shell |
| `PostToolUse` | `Edit\|Write` (use the handler `if` field to limit to `*.cs`) | `format-cs` | Runs `dotnet format whitespace --include <file>` on just the edited file. Must finish in about 2 seconds; never blocks |
| `Stop` | — | `build-check` | If C# or project files changed since the last green check, runs a quiet `dotnet build -warnaserror`. On failure exits 2 with the first \~30 error lines so Claude fixes them before finishing. Respects `stop_hook_active` to avoid loops; tests are left to `/verify` |

**Requirements:**

- Reference scripts as `"$CLAUDE_PROJECT_DIR"/.claude/hooks/<name>` and set an explicit `timeout` on each.
- Language follows my Phase 0 answer: Bash + `jq` for macOS/Linux/WSL, PowerShell (`shell: powershell`) for Windows, or both. Measure the alternative of .NET 10 file-based C# apps (`dotnet run hook.cs`) and recommend it only if per-call latency is acceptable.
- Fail open: an unexpected script error exits 0 with a stderr note; only deliberate policy blocks exit 2.
- Test every hook by piping sample JSON into it, and document each one in `.claude/hooks/README.md` with how to disable it locally.
- Put the desktop notification hook in the README as a suggested **user-level** setting, not in the committed project settings.

## Phase 5 – Package as a `dotnet new` template

- `.template.config/template.json` with `identity`, `name`, `shortName` (from Phase 0), `author`, `classifications` (Web, API, Clean Architecture), `tags` (C#, project), `sourceName: "Ardent.Project"` and `preferNameDirectory: true`, so `-n Acme.Orders` renames every project, namespace, folder and the `.slnx`.
- Keep parameters to a minimum. Add none unless Phase 0 asked for more than one database option.
- **Include** the dotfolders and dotfiles that make the harness work: `.claude/`, `.github/`, `.config/`, `.editorconfig`, `.gitignore`. Confirm they appear in generated output; don't assume.
- **Exclude** `bin/`, `obj/`, `.vs/`, `.idea/`, `*.user`, `.claude/settings.local.json`, `.claude/agent-memory*/`, `CLAUDE.local.md`, `TEMPLATE_PROMPT.md`, TEMPLATE.md and any local database files.
- Add a restore post-action.
- Add a small `templatepack.csproj` (or equivalent) so the template can also be packed as a NuGet package with `dotnet pack` and installed on other machines.
- Write two READMEs: the repo `README.md` for the generated project (run, test, configure the MediatR key, Claude Code quick start listing agents and skills), and a short `TEMPLATE.md` for maintaining the template itself (install, uninstall, update, pack).

## Phase 6 – Verification and definition of done

Prove it works from the consumer's side, not just in the template folder.

1. `dotnet new install .`, then in a temporary directory `dotnet new <shortName> -n Acme.Sample`.
2. In the generated project: `dotnet build -warnaserror`, `dotnet format --verify-no-changes` and `dotnet test` all pass, and a search for `Ardent.Project` returns nothing.
3. Run the API and exercise every row of the HTTP contract using the committed `Acme.Sample.Api.http` file (include one in the template). Check status codes, the `Location` header on create, ProblemDetails bodies, `/openapi/v1.json`, the Scalar UI and `/health`.
4. Check the harness in the generated project: `claude plugin validate .claude/skills` and `.claude/agents` report no errors; a session shows CLAUDE.md in `/context` and every hook in `/hooks`; an attempted edit to `.env` is blocked; a deliberate compile error is caught by the Stop hook; `/verify` passes.
5. Run `/review` on the generated project and fix every Critical finding. Run `/doctor prompt-audit` on the harness and apply sensible suggestions.
6. `dotnet new uninstall` cleanly removes the template.

### Definition of done

- [ ] Generated project builds with zero warnings, is format-clean, and all tests pass
- [ ] Every HTTP contract row has an integration test
- [ ] Architecture tests enforce the dependency rule
- [ ] No secrets, licence keys or local files in the template output
- [ ] CLAUDE.md under 150 lines; each rule file under 40 lines; each SKILL.md under \~100 lines
- [ ] All agents, skills and hooks verified against current Claude Code docs and tested as in step 4
- [ ] README, TEMPLATE.md and 3–4 ADRs written (Clean Architecture + CQRS, repository over DbContext in Application, exceptions + ProblemDetails, MediatR licensing)

### Final report

Finish with a short report: the final file tree, the package versions used, every deviation from this prompt and why, known limitations, and a list of deliberately **not** included extensions I may add per project (authentication/authorisation, API versioning, rate limiting, caching, .NET Aspire, OpenTelemetry).
