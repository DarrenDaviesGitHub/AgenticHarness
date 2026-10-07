---
name: review-2026-10-03-full-repo
description: Baseline security review of the template; known open items and accepted risks
metadata:
  type: project
---

2026-10-03 baseline: package scan clean; no secrets; no raw SQL; paging bounded; OpenAPI/Scalar Development-only; 500s hide detail outside Development.

Open items raised: concurrent duplicate SKU -> DbUpdateException -> 500 instead of 409 (unique index exists, no catch); no Kestrel body size limit; no HSTS; health endpoint unauthenticated and unrestricted.
Accepted: auth absent by design (plug in as a fallback policy / [Authorize] on controllers in Program.cs; /health stays anonymous).
**How to apply:** do not re-flag auth absence; check whether the items above were fixed.
