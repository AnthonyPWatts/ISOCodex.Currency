# Findings

Use this file for notes found while exercising `ISOCodex.Currency` from the extended rigs.

## Current notes

- The rigs are deliberately small examples rather than full framework integrations.
- `BulkMoneyImportTool` still translates some domain exceptions into simple review rows instead of modelling every validation path as structured result objects.
- `CheckoutPricingApi` still uses primitive DTOs at the HTTP boundary. JSON converter packages exist, but the rig keeps request and response contracts explicit for readability.
