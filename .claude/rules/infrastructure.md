---
paths:
  - "src/**/*.Infrastructure/**"
---

# Infrastructure layer rules

- Implement Application interfaces here; never add new abstractions that Application does not need.
- One `IEntityTypeConfiguration<T>` per entity in `Persistence/Configurations`. Use the domain constants for `HasMaxLength`.
- Declare unique indexes for every uniqueness rule the handlers check. `ApplicationDbContext` turns a unique-index violation into `ConflictException` (409), so a race between two requests stays a 409.
- Read-only queries use `AsNoTracking()`. Queries that feed an update return tracked entities.
- Repositories never call `SaveChanges`; `ApplicationDbContext` is the `IUnitOfWork` and handlers commit.
- Always pass `CancellationToken` to EF Core async methods.
- Paging queries need a deterministic `OrderBy(...).ThenBy(p => p.Id)` before `Skip`/`Take`.
- Never hand-edit files in `Persistence/Migrations/`. Create migrations with `/add-migration <Name>`; to undo the last one run `dotnet ef migrations remove`.
- SQLite cannot translate `ORDER BY` or comparisons on `decimal` and `DateTimeOffset`. Sort or filter on other columns, or add a value converter and note it in an ADR.
- Options classes are `sealed`, have a `SectionName` constant, and are registered with `BindConfiguration(...).ValidateDataAnnotations().ValidateOnStart()`.
- Register everything in `DependencyInjection.AddInfrastructure()`; nothing else in the solution may reference `ApplicationDbContext` except `Program.cs` via extension methods.
