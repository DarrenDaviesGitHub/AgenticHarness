---
name: project-baseline-review
description: Accepted patterns and open findings from the first full-repo architecture review of the Products template (2026-10-03)
metadata:
  type: project
---

First whole-repo review of the template, 2026-10-03 (no commits yet).

Accepted, do not re-flag:
- Validators repeat domain invariants (lengths, price > 0) so clients get all field errors as one 400; domain stays the final guard.
- Create/Update validators duplicate each other per use case (ADR 0001 favours isolated use-case folders).
- GetProductByIdQuery reuses the tracked `IProductRepository.GetByIdAsync`; a separate no-tracking method is not worth the extra surface.
- `ApplicationDbContext` implements `IUnitOfWork` directly (ADR 0002). Standard 3-ctor exception classes exist for analyzer rules (CA1032).
- Application referencing Microsoft.Extensions.Logging/DependencyInjection abstractions is fine; only EF Core and ASP.NET Core are banned.

Raised on 2026-10-03 (check whether fixed before raising again):
- Unique SKU index race surfaces as 500 (DbUpdateException not translated to ConflictException).
- SKU character rule lives only in validators (regex duplicated), not in `Product`.
- `Id NotEmpty` rules on Update/Delete give 400 for Guid.Empty while GET gives 404; DELETE 400 undocumented. Root cause: convention test forces a validator per command.
- Arch tests: no Application -> Microsoft.AspNetCore check; no handlers -> ISender/IMediator check.
- Infrastructure has FrameworkReference Microsoft.AspNetCore.App (opens ASP.NET Core to Infrastructure).
- `IApplicationMarker` is a marker with no need beyond assembly anchoring.

**Why:** Products is the slice every feature copies, so issues here multiply.
**How to apply:** On later reviews, diff against this list; flag regressions and copies of these issues in new slices.
