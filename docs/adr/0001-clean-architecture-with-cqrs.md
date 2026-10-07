# 0001. Use Clean Architecture with CQRS via MediatR

- **Status:** Accepted
- **Date:** 2026-10-03
- **Author:** Ardent

## Context

Every new service starts from this template, so its structure must stay readable in an afternoon while scaling to dozens of use cases. Business rules must be testable without a database or web host, and each use case should be easy to find, review and change in isolation.

## Decision

We will split the solution into four projects (Domain, Application, Infrastructure, Api) with dependencies pointing inwards, enforced by `tests/*ArchitectureTests`. Each use case is a command or query record with one handler and one validator in its own folder (`Application/<Feature>/<UseCase>/`), dispatched through MediatR. Cross-cutting concerns (logging, validation) are MediatR pipeline behaviours. `Products` is the reference slice.

## Consequences

- Positive: one obvious place for each kind of code; handlers are unit-testable with substitutes; new features copy a known shape (`/add-feature`).
- Positive: behaviours apply validation and logging uniformly without attributes or base classes.
- Negative: more files per feature than a minimal-API-only design, and an indirection (`ISender`) between controller and handler.
- Revisit when: the service is tiny (one or two endpoints) or MediatR licensing becomes unacceptable (see 0004).

## Alternatives considered

- **Vertical slices in a single project** - fewer projects, but the dependency rule could only be enforced by convention.
- **Plain service classes per feature** - less indirection, but no shared pipeline for validation and logging.
