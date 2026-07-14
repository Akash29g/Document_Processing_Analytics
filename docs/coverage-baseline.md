# Coverage Baseline (Round 0)
Measured 2026-07-14 at commit 0c159a2

| Layer             | Line % (measured) | R3 gate floor (−2%) |
|-------------------|-------------------|---------------------|
| Service           | 81.1              | 79                  |
| Domain            | 77.5              | 75                  |
| Data              | 25.6              | 23                  |
| Api               | 24.5              | 22                  |
| Frontend (Vitest) | 75.15             | 73                  |

Backend aggregate: 58.7% line / 45.3% branch (informational).
Gate = per-LAYER line floors above (NOT one global number — the tool enforces per-assembly).
Branch coverage informational for now.

## R3 plan (coverage-gate owner)
- Annotate non-unit-testable infra with [ExcludeFromCodeCoverage]:
  Program, *FeatureExtensions, DependencyInjection, DbSeeder,
  background services, middleware, S3FileStorage, SmtpEmailSender,
  NovaInvoiceExtractor, Swagger filter, Security/DataProtection extensions.
  (coverage.runsettings already excludes ExcludeFromCodeCoverageAttribute)
- Re-measure -> Api/Data jump up -> raise their floors.
- CI runs: 4 backend correctness projects + `ng test --watch=false`.
  Perf tests excluded from the blocking gate.
