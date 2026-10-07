---
paths:
  - "tests/**"
---

# Testing rules

- Stack: xUnit v3 (Microsoft.Testing.Platform), Shouldly assertions, NSubstitute doubles. Do not add FluentAssertions or Moq.
- Name tests `Method_Scenario_ExpectedResult`, e.g. `Handle_DuplicateSku_ThrowsConflictException`.
- Arrange / Act / Assert separated by single blank lines; no `// Arrange` comments.
- One behaviour per test. Several `Should` calls are fine when they describe that one behaviour.
- No logic in tests: no loops, `if`s or try/catch. Use `[Theory]` + `[InlineData]` for variations.
- Test data comes from static factory helpers (`ProductFactory`, `TestData`, `ApiClient.NewProduct()`), never shared mutable fixtures.
- Pass `TestContext.Current.CancellationToken` to async calls.
- Test classes are `public sealed`; one class per handler and per validator, mirroring the source folder.
- Domain tests: no mocks. Application tests: substitute repositories, `IUnitOfWork` and `TimeProvider`.
- Integration tests use `IClassFixture<ApiFactory>` (isolated in-memory SQLite per class). Use unique SKUs (`ApiClient.UniqueSku()`) so tests in a class do not depend on each other.
- Assert error responses with `ShouldBeProblemAsync<T>(status)`, which also checks `traceId`, `instance` and the problem+json content type.
- Every row of the HTTP contract table in `CLAUDE.md` has an integration test. New endpoint = new rows = new tests.
- Architecture rules live in `tests/*.ArchitectureTests`. Add a rule there rather than a comment when a convention matters.
