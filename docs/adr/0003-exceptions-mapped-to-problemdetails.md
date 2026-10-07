# 0003. Signal failures with exceptions mapped centrally to ProblemDetails

- **Status:** Accepted
- **Date:** 2026-10-03
- **Author:** Ardent

## Context

Handlers must report "not found", "conflict" and invalid input without knowing about HTTP, and every error response must be consistent RFC 9457 ProblemDetails with a `traceId` for support.

## Decision

We will throw a small set of exceptions and map them in one `IExceptionHandler` (`Api/ExceptionHandling/GlobalExceptionHandler.cs`), registered with `AddProblemDetails()` and the built-in exception handler middleware:

| Exception | Status |
| --- | --- |
| FluentValidation `ValidationException` (from `ValidationBehaviour`) | 400 `ValidationProblemDetails`, camelCase field keys |
| `DomainException` | 400 |
| `NotFoundException` | 404 |
| `ConflictException` (also thrown by `ApplicationDbContext` for unique-index violations, so a race on a duplicate key is still 409) | 409 |
| anything else | 500, logged, detail only in Development |

A client abort (`OperationCanceledException` with `RequestAborted` set) is swallowed without an error log. `CustomizeProblemDetails` adds `traceId` and `instance` to every problem, including `[ApiController]` model-binding 400s and `UseStatusCodePages` 404s.

## Consequences

- Positive: handlers stay HTTP-free and short; one file defines the whole error contract; integration tests assert one shape.
- Negative: exceptions for expected outcomes cost more than return values (irrelevant at this scale) and are less explicit in method signatures.
- Revisit when: profiling shows exception cost matters, or callers other than HTTP need typed outcomes; then consider a `Result<T>` type.

## Alternatives considered

- **`Result<T>` / OneOf return types** - explicit, but adds a concept to every handler and controller for little gain in a CRUD template.
- **Hand-written exception middleware** - superseded by `IExceptionHandler`, which integrates with `IProblemDetailsService`.
