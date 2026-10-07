---
paths:
  - "src/**/*.Application/**"
---

# Application layer rules

- One folder per use case under `<Feature>/<UseCase>/` holding exactly: the request record, its handler, and its validator (queries without input may skip the validator).
- Commands and queries are `sealed record`s implementing `IRequest` / `IRequest<T>`. Name them `<Verb><Noun>Command` or `<Verb><Noun>Query`.
- Handlers are `sealed` classes with primary-constructor injection, named `<Request>Handler`. One public method: `Handle`.
- Every command with input beyond the route `Id` has a `sealed` `AbstractValidator<TCommand>` named `<Command>Validator` (architecture-tested). Reuse domain constants (`Product.NameMaxLength`, `Product.SkuPattern`) instead of literals. An empty or unknown `Id` is a 404, not a validation error.
- Never call validators from handlers. `ValidationBehaviour` runs them for every request.
- Handlers throw `NotFoundException` or `ConflictException`. Never return null, `bool` success flags, status codes or anything from ASP.NET Core.
- No EF Core: depend on `IProductRepository`-style interfaces in `Common/Interfaces` plus `IUnitOfWork.SaveChangesAsync`.
- Inject `TimeProvider` for the current time; pass the value into domain methods.
- Responses are `sealed record`s with a static `From<Entity>(...)` mapper next to them. No AutoMapper.
- Pass `CancellationToken` to every async call. Async methods end in `Async` (except MediatR's `Handle`).
- Logging uses message templates with named placeholders, never string interpolation.
- New use case? Copy the shape of `Products/CreateProduct` and `Products/GetProductById`.
