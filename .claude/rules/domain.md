---
paths:
  - "src/**/*.Domain/**"
---

# Domain layer rules

- Reference nothing: no project references, no NuGet packages, no `Microsoft.*` namespaces. BCL only (`System.*`).
- Entities are `sealed` classes with `private set` (or `private init`) on every property. No public setters.
- Create entities only through a static factory (`Product.Create(...)`); change them only through intention-revealing methods (`product.Update(...)`).
- Keep a private parameterless constructor for EF Core; never make it public.
- Enforce invariants inside the entity and throw `DomainException` with a message that names the broken rule.
- Use the C# 14 `field` keyword for guarded properties instead of separate backing fields.
- Expose limits as `public const` (e.g. `NameMaxLength`) so validators and EF configuration reuse them instead of repeating numbers.
- Generate IDs with `Guid.CreateVersion7(...)`. Take timestamps as parameters; never call `DateTime.Now`, `DateTimeOffset.UtcNow` or `TimeProvider` here.
- A failed operation must leave the entity unchanged (validate everything before assigning anything).
- No persistence, HTTP, logging or serialization attributes on domain types.
- Every new rule needs a unit test in `tests/*.Domain.UnitTests` (see `ProductTests`).
