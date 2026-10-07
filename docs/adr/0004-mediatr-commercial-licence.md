# 0004. Keep MediatR under its commercial licence, key supplied by configuration

- **Status:** Accepted
- **Date:** 2026-10-03
- **Author:** Ardent

## Context

From version 13, MediatR is dual-licensed (RPL-1.5 / commercial) by Lucky Penny Software. The Community edition is free for organisations under USD 5M gross annual revenue but requires registering for a licence key. Without a key MediatR still works and logs a licence warning. The key must never be committed.

## Decision

We will use MediatR 14 and pass the key from configuration (`MediatR:LicenseKey`) to `AddMediatR(config => config.LicenseKey = ...)`:

- Local development: `dotnet user-secrets set "MediatR:LicenseKey" "<key>" --project src/Ardent.Project.Api`.
- CI: the `MEDIATR_LICENSE_KEY` repository secret, exposed as `MediatR__LicenseKey` in `.github/workflows/ci.yml`.
- Production: the `MediatR__LicenseKey` environment variable or a secret store.

MediatR also reads `MEDIATR_LICENSE_KEY` directly as a fallback.

## Consequences

- Positive: keeps the familiar pipeline-behaviour model (see 0001) with no secrets in the repository.
- Negative: each organisation must obtain and rotate a key; licence terms can change on upgrade, so check them before bumping the major version.
- Revisit when: licence cost or terms are unacceptable. Replacing MediatR touches only `DependencyInjection.AddApplication`, the behaviours, request/handler interfaces and controllers (`ISender`), because handlers have no other MediatR dependency.

## Alternatives considered

- **Pin MediatR 12 (last Apache-2.0 release)** - free, but no future fixes.
- **Hand-rolled dispatcher or plain services** - no licence, but the pipeline must be rebuilt and maintained in-house.
- **Source-generated mediator libraries** - fast and permissive, but less familiar to most teams.
