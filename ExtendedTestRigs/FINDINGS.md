# Findings

Use this file for notes found while exercising `ISOCodex.Currency` from the extended rigs.

## Current notes

- There is no structured validation API yet, so the rigs currently catch exceptions and translate them into simple review results.
- There is no formatter API yet, so examples display money with explicit currency codes rather than symbols.
- There is no JSON converter package yet, so API DTOs use primitive amount/currency fields at the boundary.
