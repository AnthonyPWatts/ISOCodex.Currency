# Release Gate

Version: `1.0.1`

## Current status

`v1.0.1` has been pushed and the trusted-publishing workflow completed successfully. The workflow restored, built, tested, packed, verified the expected artifacts, uploaded package artifacts, authenticated with NuGet trusted publishing, and pushed the package and symbol package set.

Use the checklist below as the evidence record for `1.0.1` and as the template for future releases. NuGet Gallery and symbol indexing can lag a successful package push, so post-publication visibility should still be checked separately.

Post-release housekeeping was completed on 2026-06-23. The package is publicly visible on NuGet.org, the core package installs from the NuGet.org v3 feed, and GitHub release notes exist for `v1.0.1`.

`master` now contains fixes committed after the `v1.0.1` tag. Publish a follow-up patch release before parking the project if those fixes should be available from NuGet.org.

## Required checks

- [x] .NET 9 and .NET 10 SDK/runtime available locally, or GitHub Actions uses `actions/setup-dotnet` with `9.0.x` and `10.0.x`.
- [x] `dotnet restore ISOCodex.Currency.sln`
- [x] `dotnet build ISOCodex.Currency.sln -c Release --no-restore`
- [x] `dotnet test ISOCodex.Currency.sln -c Release --no-build`
- [x] `pwsh ./eng/pack-packages.ps1 -Configuration Release -OutputPath artifacts -Version 1.0.1`
- [x] Expected `.nupkg` files produced for all packable package projects.
- [x] Matching `.snupkg` files produced for all packable package projects.
- [x] `pwsh ./eng/smoke-test-package.ps1 -Version 1.0.1`
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

Package content inspection was performed with the local `.nupkg` files under `artifacts/`. The core package contains `README.md` and `lib/netstandard2.1` assets; the analyzer package contains the analyzer assembly under `analyzers/dotnet/cs`.

## Publication

Use `.github/workflows/publish-nuget.yml`.

Published tag:

```text
v1.0.1
```

Pushing a `v*` tag starts the trusted-publishing workflow. The workflow uses GitHub OIDC and `NuGet/login@v1` to obtain a short-lived NuGet API key, so no long-lived `NUGET_API_KEY` repository secret is required.

For recovery or rerun scenarios, the workflow can also be started manually with `release_tag` set to an existing `v*` tag. Normal branch pushes and pull requests must not publish this package.

## Post-publication checks

- [x] Install `ISOCodex.Currency` from NuGet.org in a fresh local project.
- [x] Confirm the package README renders correctly on NuGet.org.
- [x] Confirm symbol package indexing succeeds or record the NuGet validation error.
- [x] Create GitHub release notes for `v1.0.1`.
- [x] Review whether follow-up issues are needed for additional bridge, provider, validation-adapter, or analyzer packages.

Evidence:

- `dotnet add artifacts/post-publication/CurrencyNuGetPostPublication/CurrencyNuGetPostPublication.csproj package ISOCodex.Currency --version 1.0.1 --source https://api.nuget.org/v3/index.json` installed `ISOCodex.Currency` `1.0.1` from NuGet.org.
- The NuGet.org package page for `ISOCodex.Currency` `1.0.1` renders the package README, install command, repository links, package download link, symbol download link, dependency list, and version history.
- GitHub release notes were created at `https://github.com/AnthonyPWatts/ISOCodex.Currency/releases/tag/v1.0.1`.
- No additional follow-up issues were opened from this release gate. Remaining provider, bridge, validation, and analyzer expansion work is intentionally future package work unless a new consumer need justifies it.

Symbol package note:

- The publish workflow produced matching `.snupkg` artifacts and NuGet.org shows a `Download symbols` link for `ISOCodex.Currency` `1.0.1`.
- On 2026-06-23, both `https://api.nuget.org/v3-flatcontainer/isocodex.currency/1.0.1/isocodex.currency.1.0.1.snupkg` and `https://www.nuget.org/api/v2/symbolpackage/ISOCodex.Currency/1.0.1` returned HTTP 404. Treat this as the recorded symbol validation issue for `1.0.1`; check the NuGet owner UI before the next release if symbol indexing status matters.
