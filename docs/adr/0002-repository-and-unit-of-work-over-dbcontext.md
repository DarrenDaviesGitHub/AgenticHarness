# 0002. Keep EF Core out of Application with small repositories and a unit of work

- **Status:** Accepted
- **Date:** 2026-10-03
- **Author:** Ardent

## Context

Handlers need to load and save aggregates. Exposing `DbContext` (or an `IApplicationDbContext` with `DbSet<T>` properties) to Application is common, but it makes Application reference EF Core, lets handlers build arbitrary queries, and turns handler unit tests into in-memory-database tests.

## Decision

We will define one narrow repository interface per aggregate in `Application/Common/Interfaces` (e.g. `IProductRepository`) containing only the operations handlers use, plus `IUnitOfWork.SaveChangesAsync`. `ApplicationDbContext` implements `IUnitOfWork` directly; repositories in Infrastructure stage changes and never save. Application has no EF Core reference, enforced by an architecture test.

## Consequences

- Positive: Application depends only on Domain; handler tests substitute two small interfaces.
- Positive: query shape (tracking, ordering, paging) lives in one place per aggregate.
- Negative: every new query needs a repository method; complex read models may need a dedicated query service later.
- Negative: no generic repository, so some boilerplate repeats per aggregate (deliberately: generic repositories leak `IQueryable` and hide intent).
- Revisit when: read-heavy features need projections that do not fit a repository; add a query interface per read model rather than exposing `DbContext`.

## Alternatives considered

- **`IApplicationDbContext` in Application** - fewer files, but couples Application to EF Core and `IQueryable` semantics.
- **Generic `IRepository<T>`** - looks reusable, but leaks persistence concerns and encourages anaemic handlers.
