# Release Gate

Version: `0.9.0-alpha.11`

## Required checks

- [ ] .NET 9 and .NET 10 SDK/runtime available locally, or GitHub Actions uses `actions/setup-dotnet` with `9.0.x` and `10.0.x`.
- [ ] `dotnet restore ISOCodex.Currency.sln`
- [ ] `dotnet build ISOCodex.Currency.sln -c Release --no-restore`
- [ ] `dotnet test ISOCodex.Currency.sln -c Release --no-build`
- [ ] `pwsh ./eng/pack-packages.ps1 -Configuration Release -OutputPath artifacts -Version 0.9.0-alpha.11`
- [ ] Expected `.nupkg` files produced for all packable package projects.
- [ ] Matching `.snupkg` files produced for all packable package projects.
- [ ] `pwsh ./eng/smoke-test-package.ps1 -Version 0.9.0-alpha.11`
- [ ] If the local machine lacks the .NET 9 runtime but has a newer compatible runtime, `pwsh ./eng/smoke-test-package.ps1 -Version 0.9.0-alpha.11 -UseMajorRollForward`
- [ ] Local package smoke test installs package references from the local package folder, not from project references.
- [ ] README package section is accurate.
- [ ] Changelog entry exists.
- [ ] NuGet trusted publisher exists for repository `AnthonyPWatts/ISOCodex.Currency`, workflow `publish-nuget.yml`, and environment `release`.
- [ ] GitHub environment `release` is configured with the intended approval rules.
- [ ] Release workflow runs from the intended `v*` tag.

## Manual review

- [ ] Package description is truthful.
- [ ] Known limitations remain visible, including that the pinned checked-in currency snapshot is derived metadata and not an official ISO 4217 redistribution.
- [ ] No unintentional runtime dependencies were added.
- [ ] Package contents inspected with NuGet Package Explorer or equivalent.
- [ ] Package README renders acceptably in the NuGet preview.
- [ ] Owner approves publication.

## Publication

Use `.github/workflows/publish-nuget.yml`.

Recommended publish tag:

```text
v0.9.0-alpha.11
```

Pushing a `v*` tag starts the trusted-publishing workflow. The workflow uses GitHub OIDC and `NuGet/login@v1` to obtain a short-lived NuGet API key, so no long-lived `NUGET_API_KEY` repository secret is required.

For recovery or rerun scenarios, the workflow can also be started manually with `release_tag` set to an existing `v*` tag. Normal branch pushes and pull requests must not publish this package.

## Post-publication checks

- [ ] Install `ISOCodex.Currency` from NuGet.org in a fresh local project.
- [ ] Confirm the package README renders correctly on NuGet.org.
- [ ] Confirm symbol package indexing succeeds or record the NuGet validation error.
- [ ] Create GitHub release notes for `v0.9.0-alpha.11`.
- [ ] Open follow-up issues for data snapshot and additional bridge/provider packages as needed.
