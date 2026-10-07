---
name: add-migration
description: Create an EF Core migration with the correct project and startup flags and show the SQL for review. Use after changing an entity or IEntityTypeConfiguration, or when asked to "add a migration", "update the schema" or "migrate the database model".
argument-hint: <MigrationName in PascalCase>
arguments: [name]
allowed-tools: Bash(dotnet tool restore *) Bash(dotnet ef migrations add *) Bash(dotnet ef migrations list *) Bash(dotnet ef migrations script *) Bash(dotnet build *) PowerShell(dotnet tool restore *) PowerShell(dotnet ef migrations add *) PowerShell(dotnet ef migrations list *) PowerShell(dotnet ef migrations script *) PowerShell(dotnet build *)
---

# Add migration: $name

Rules: never hand-edit generated migration files (a hook blocks it). If the result is wrong, fix the entity or configuration, run `dotnet ef migrations remove --project src/Ardent.Project.Infrastructure --startup-project src/Ardent.Project.Api`, and generate again.

1. If `$name` is empty or not PascalCase, ask for a descriptive name such as `AddOrders` or `AddProductCategory`.
2. `dotnet tool restore`
3. Note the current last migration: `dotnet ef migrations list --project src/Ardent.Project.Infrastructure --startup-project src/Ardent.Project.Api --no-connect`
4. Create it:
   `dotnet ef migrations add $name --project src/Ardent.Project.Infrastructure --startup-project src/Ardent.Project.Api --output-dir Persistence/Migrations`
5. Show the SQL for just this migration (use the previous migration name from step 3, or `0` if there was none):
   `dotnet ef migrations script <Previous> $name --project src/Ardent.Project.Infrastructure --startup-project src/Ardent.Project.Api`
6. Review the SQL and point out anything risky: dropped columns or tables, data loss, non-nullable columns added without defaults, SQLite table rebuilds.
7. `dotnet build -warnaserror` to confirm the migration compiles.

Report the migration file names, the SQL (or a summary if long) and your risk notes. Development applies migrations on startup; other environments use `dotnet ef database update` or a migration bundle, which the user runs.
