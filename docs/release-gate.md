# Release Gate

Version: `1.0.2`

## Current status

`v1.0.2` has been pushed and the trusted-publishing workflow completed successfully. The workflow restored, built, tested, packed, verified the expected artifacts, uploaded package artifacts, authenticated with NuGet trusted publishing, and pushed the package and symbol package set.

Use the checklist below as the evidence record for `1.0.2` and as the template for future releases. NuGet Gallery and symbol indexing can lag a successful package push, so post-publication visibility should still be checked separately.

## Required checks

- [x] .NET 9 and .NET 10 SDK/runtime available locally, or GitHub Actions uses `actions/setup-dotnet` with `9.0.x` and `10.0.x`.
- [x] `dotnet restore ISOCodex.Currency.sln`
- [x] `dotnet build ISOCodex.Currency.sln -c Release --no-restore`
- [x] `dotnet test ISOCodex.Currency.sln -c Release --no-build`
- [x] `pwsh ./eng/pack-packages.ps1 -Configuration Release -OutputPath artifacts -Version 1.0.2`
- [x] Expected `.nupkg` files produced for all packable package projects.
- [x] Matching `.snupkg` files produced for all packable package projects.
- [x] `pwsh ./eng/smoke-test-package.ps1 -Version 1.0.2`
- [x] Local package smoke test installs package references from the local package folder, not from project references.
- [x] README package section is accurate.
- [x] Changelog entry exists.
- [x] NuGet trusted publisher exists for repository `AnthonyPWatts/ISOCodex.Currency`, workflow `publish-nuget.yml`, and environment `release`.
- [x] GitHub environment `release` is configured with the intended approval rules.
- [x] Release workflow ran from the intended `v*` tag.

## Manual review

- [x] Package description is truthful.
- [x] Known limitations remain visible, including that the pinned checked-in currency snapshot is derived metadata and not an official ISO 4217 redistribution.
- [x] No unintentional runtime dependencies were added.
- [x] Package contents inspected with NuGet Package Explorer or equivalent.
- [x] Package README renders acceptably in the NuGet preview.
- [x] Owner approves publication.

Package content inspection was performed with the local `.nupkg` files under `artifacts/smoke/`. The core package contains `README.md` and `lib/netstandard2.1` assets; the analyzer package contains the analyzer assembly under `analyzers/dotnet/cs`.

## Publication

Use `.github/workflows/publish-nuget.yml`.

Published tag:

```text
v1.0.2
```

Pushing a `v*` tag starts the trusted-publishing workflow. The workflow uses GitHub OIDC and `NuGet/login@v1` to obtain a short-lived NuGet API key, so no long-lived `NUGET_API_KEY` repository secret is required.

For recovery or rerun scenarios, the workflow can also be started manually with `release_tag` set to an existing `v*` tag. Normal branch pushes and pull requests must not publish this package.

## Post-publication checks

- [x] Install `ISOCodex.Currency` from NuGet.org in a fresh local project.
- [x] Confirm the package README renders correctly on NuGet.org.
- [x] Confirm symbol packages are available from NuGet.org or record the NuGet validation error.
- [x] Create GitHub release notes for `v1.0.2`.
- [x] Review whether follow-up issues are needed for additional bridge, provider, validation-adapter, or analyzer packages.

Publication evidence:

- GitHub Actions run `28020914341` completed successfully for tag `v1.0.2`: `https://github.com/AnthonyPWatts/ISOCodex.Currency/actions/runs/28020914341`.
- The publish log recorded HTTP `Created` responses from NuGet.org for all `1.0.2` `.nupkg` and `.snupkg` artifacts.
- GitHub release notes were created at `https://github.com/AnthonyPWatts/ISOCodex.Currency/releases/tag/v1.0.2`.
- On 2026-06-23, NuGet.org flat-container package indexes exposed `1.0.2` for all package IDs in the package family.
- On 2026-06-23, a fresh `net9.0` console project installed `ISOCodex.Currency` `1.0.2` from NuGet.org and built successfully in Release.
- On 2026-06-23, `https://www.nuget.org/packages/ISOCodex.Currency/1.0.2` returned HTTP 200 and contained the package README text.
- On 2026-06-23, all `1.0.2` symbol packages downloaded successfully from NuGet Gallery's `api/v2/symbolpackage` endpoint.
- Exchange behaviours beyond direct-rate conversion were reviewed and deferred as future issue work rather than `1.0.2` release blockers:
  - `https://github.com/AnthonyPWatts/ISOCodex.Currency/issues/6` - inverse exchange-rate conversion.
  - `https://github.com/AnthonyPWatts/ISOCodex.Currency/issues/7` - triangulated exchange-rate conversion.
  - `https://github.com/AnthonyPWatts/ISOCodex.Currency/issues/8` - optional cached exchange-rate lookup support.
  - `https://github.com/AnthonyPWatts/ISOCodex.Currency/issues/9` - live exchange-rate provider package strategy.
