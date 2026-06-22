# ISOCodex.Currency Codex Roadmap Pack

This folder contains remaining roadmap notes and implementation prompts for `ISOCodex.Currency`.

The NuGet-first release preparation has been implemented and the package is now in the `0.1.0-alpha.*` prerelease line. Keep future work scoped to APIs that exist in the checkout unless the task explicitly adds them.

Recommended remaining order:

1. `02-core-hardening-roadmap.md`
2. `03-boundary-and-isocodex-integrations.md`
3. `04-killer-features-backlog.md`
4. `05-codex-task-prompts.md`

The immediate release checklist now lives in `../../release-gate.md`.

The roadmap deliberately keeps `ISOCodex.Currency` core independent from `ISOCodex.Countries` and `ISOCodex.Addressing`, while defining optional bridge packages for country-aware and address-aware currency workflows.
