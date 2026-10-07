# Ardent.Project

ASP.NET Core Web API on **.NET 10 / C# 14** built with Clean Architecture and CQRS. It ships with one complete sample feature (`Products` CRUD) that every new feature copies, and a Claude Code harness so AI-assisted work follows the same rules as you do.

| Concern | Choice |
| --- | --- |
| Architecture | Domain / Application / Infrastructure / Api, dependency rule enforced by tests |
| Use cases | MediatR 14 commands and queries with logging and validation behaviours |
| Validation | FluentValidation 12 (through the MediatR pipeline only) |
| Persistence | EF Core 10 + SQLite, repositories + unit of work |
| HTTP | Controllers, RFC 9457 ProblemDetails for every error, OpenAPI 3.1 + Scalar UI |
| Tests | xUnit v3 (Microsoft.Testing.Platform), Shouldly, NSubstitute, ArchUnitNET |

## Prerequisites

- .NET 10 SDK (`global.json` accepts any 10.0.1xx or later feature band)
- PowerShell 7 (`pwsh`) on `PATH`, used by the Claude Code hooks
- Git. Run `git init` if the project is not in a repository yet; hooks and skills read git state.

## Run

```bash
dotnet tool restore
dotnet run --project src/Ardent.Project.Api --launch-profile https
```

- Scalar UI: <https://localhost:7180/scalar> (Development only)
- OpenAPI document: <https://localhost:7180/openapi/v1.json> (Development only), also committed at `docs/openapi.json`
- Health: <https://localhost:7180/health>
- Try every endpoint with `src/Ardent.Project.Api/Ardent.Project.Api.http` (VS Code REST Client, Rider or Visual Studio)

In Development the SQLite database (`app.db`) is created and migrated on startup.

## Test and verify

```bash
dotnet build -warnaserror
dotnet format --verify-no-changes
dotnet test
```

CI (`.github/workflows/ci.yml`) runs the same steps and fails if `docs/openapi.json` is out of date.

## Configuration

| Key | Purpose | Where to set it |
| --- | --- | --- |
| `Database:ConnectionString` | SQLite connection string, validated at startup | `appsettings.json` / environment |
| `MediatR:LicenseKey` | MediatR licence key | user-secrets locally, environment variable elsewhere. **Never commit it.** |

MediatR 13+ is commercially licensed (free Community edition for organisations under USD 5M revenue; register at Lucky Penny Software). Without a key it works and logs a warning. See `docs/adr/0004-mediatr-commercial-licence.md`.

```bash
# Local development
dotnet user-secrets set "MediatR:LicenseKey" "<your key>" --project src/Ardent.Project.Api
```

- **GitHub Actions**: add a repository secret `MEDIATR_LICENSE_KEY`; the workflow exposes it as `MediatR__LicenseKey`.
- **Production**: set the environment variable `MediatR__LicenseKey` (or `MEDIATR_LICENSE_KEY`) from your secret store.

## Project layout

```text
src/
  Ardent.Project.Domain/          Entities and invariants (no dependencies)
  Ardent.Project.Application/     Use cases: commands/queries, handlers, validators, behaviours
  Ardent.Project.Infrastructure/  EF Core, repositories, migrations, options
  Ardent.Project.Api/             Controllers, contracts, ProblemDetails, OpenAPI, Program.cs
tests/
  *.Domain.UnitTests  *.Application.UnitTests  *.Api.IntegrationTests  *.ArchitectureTests
docs/
  adr/            Architecture decision records
  openapi.json    Generated API contract (do not edit)
```

## Claude Code quick start

Open the repository and run `claude`. `CLAUDE.md` and the path-scoped rules in `.claude/rules/` load automatically, and the hooks start guarding the session.

**Skills** (type them as slash commands):

| Skill | Use it to |
| --- | --- |
| `/add-feature <description>` | Build a full vertical slice (domain to tests) copying the Products pattern |
| `/add-tests <target>` | Find and fill test gaps with the `qa-engineer` agent |
| `/add-migration <Name>` | Create an EF Core migration and review its SQL |
| `/verify` | Build with warnings as errors, check format, run tests, check the OpenAPI contract (also runs before commits) |
| `/review` | Run code, architecture and security reviewers in parallel on your changes |
| `/commit` | Write a Conventional Commits message and commit after you confirm |
| `/explain-feature <feature>` | Trace a request end to end with a sequence diagram |
| `/project-structure [question]` | Explain the layout and where new code goes |
| `/overview` | Onboarding briefing |
| `/test-me [topic]` | Five-question quiz on this codebase |
| `/code-cleanup <path>` | Behaviour-preserving tidy-up with tests before and after |
| `/new-adr <title>` | Create the next numbered ADR |

**Agents** (Claude delegates to them automatically, or ask for one by name): `code-reviewer`, `architecture-reviewer`, `security-reviewer` (read-only, with project memory in `.claude/agent-memory/`), `qa-engineer` (edits `tests/` only), `debugger`.

**Hooks** (see `.claude/hooks/README.md`): session context at start-up, blocked edits to secrets/migrations/generated files, blocked destructive shell commands, automatic formatting of edited C# files, and a build check before Claude finishes.

**Permissions** (`.claude/settings.json`): builds, tests, formatting and read-only git run without prompts; commits, pushes, new packages and database updates ask first; secrets, force-pushes, hard resets and database drops are denied. Put personal overrides in `.claude/settings.local.json` and `CLAUDE.local.md` (both gitignored).

The optional `.mcp.json` connects the Microsoft Learn MCP server so Claude can check current .NET documentation; approve it when Claude Code asks.

## Not included (add per project)

Authentication and authorization, API versioning, rate limiting, output/distributed caching, .NET Aspire orchestration, OpenTelemetry. Record each addition with `/new-adr`.
