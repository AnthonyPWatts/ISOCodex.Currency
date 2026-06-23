# Release Gate

Version: `1.0.1`

## Current status

`v1.0.1` has been prepared as a documentation-only patch release for package README and release-status correctness after `1.0.0`.

Use the checklist below as the evidence record for `1.0.1` and as the template for future releases. NuGet Gallery and symbol indexing can lag a successful package push, so post-publication visibility should still be checked separately.

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
- [ ] Package contents inspected with NuGet Package Explorer or equivalent.
- [ ] Package README renders acceptably in the NuGet preview.
- [x] Owner approves publication.

## Publication

Use `.github/workflows/publish-nuget.yml`.

Published tag:

```text
v1.0.1
```

Pushing a `v*` tag starts the trusted-publishing workflow. The workflow uses GitHub OIDC and `NuGet/login@v1` to obtain a short-lived NuGet API key, so no long-lived `NUGET_API_KEY` repository secret is required.

For recovery or rerun scenarios, the workflow can also be started manually with `release_tag` set to an existing `v*` tag. Normal branch pushes and pull requests must not publish this package.

## Post-publication checks

- [ ] Install `ISOCodex.Currency` from NuGet.org in a fresh local project.
- [ ] Confirm the package README renders correctly on NuGet.org.
- [ ] Confirm symbol package indexing succeeds or record the NuGet validation error.
- [ ] Create GitHub release notes for `v1.0.1`.
- [ ] Open follow-up issues for additional bridge, provider, validation-adapter, or analyzer packages as needed.
