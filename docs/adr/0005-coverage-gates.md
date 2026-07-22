# ADR-0005: Per-Layer Code Coverage Gates in CI

| | |
|---|---|
| **Status** | Accepted |
| **Date** | Round 3 (CI/CD) |
| **Deciders** | Shubh (Dev B, coverage gate) & Akash (Dev A), cross-reviewed |
| **Round** | R3 — coverage gate + lint/format |
| **Related** | `docs/coverage-baseline.md`, `coverage.runsettings`, `scripts/coverage-gate.sh` |

---

## Context

We wanted CI to **prevent coverage regressions** without turning the gate into theatre. A single global coverage threshold is actively misleading for this solution because the layers have **very different testability**:

- **Service** and **Domain** are heavily unit-tested (business logic, validators, mappers) — high, meaningful coverage.
- **Api** and **Data** contain large amounts of **un-unit-testable infrastructure**: `Program`, `*FeatureExtensions` / DI wiring, `DbSeeder`, background services (`ExtractionWorker`, `AlertEvaluationBackgroundService`), middleware, `S3FileStorage`, `SmtpEmailSender`, `NovaInvoiceExtractor`, Swagger filters, Security/DataProtection extensions, EF migrations.

A global floor high enough to be meaningful for Service would **fail** Api/Data; a floor low enough to pass Api/Data would be **meaningless** for Service. We also needed the frontend (Angular/Vitest) covered by the same discipline.

**Measured baseline (2026-07-14, commit `0c159a2`):**

| Layer | Line % (measured) | Gate floor (−2% headroom) |
|---|---|---|
| Service | 81.1 | 79 |
| Domain | 77.5 | 75 |
| Data | 25.6 | 23 |
| Api | 24.5 | 22 |
| Frontend (Vitest) | 75.15 line / 70.11 branch | 73 |

Backend aggregate at baseline: 58.7% line / 45.3% branch (informational only). The low Api/Data numbers are **not** missing logic tests — controllers are near 100%, services ~81% — they are dominated by un-unit-testable infra.

## Alternatives considered

- **Single global coverage threshold** — rejected: misleading given the per-layer testability gap (see Context).
- **No gate, rely on review** — rejected: coverage silently erodes over time; regressions go unnoticed.
- **Count everything, set a low global floor** — rejected: passes trivially and provides no real protection for the layers that matter.
- **Exclude Api/Data from coverage entirely** — rejected: hides genuinely testable controller/service logic that *does* live there.

## Decision

1. **Per-layer line floors, not one global number.** Each assembly has its own floor set at the measured baseline minus ~2% headroom (table above). CI **fails** if any layer drops below its floor.
2. **Backend tooling.** `dotnet test --collect:"XPlat Code Coverage" --settings coverage.runsettings` (coverlet) + **ReportGenerator** to merge cobertura files into an HTML + text summary. `coverage.runsettings` excludes `[*.Tests]`, `*.Migrations.*`, and anything marked `[ExcludeFromCodeCoverage]` / `[GeneratedCode]`. Assembly filter `+DocAnalytics.*;-DocAnalytics.*.Tests`; class filter `-*.Migrations.*`.
3. **Raise Api/Data honestly.** Un-unit-testable infra is annotated `[ExcludeFromCodeCoverage]` (already excluded by the runsettings) so the Api/Data percentages reflect **real testable code**; their floors are then raised and **ratcheted** upward over time — never lowered to accommodate a regression.
4. **Frontend tooling.** Angular’s **Vitest** runner via `ng test --watch=false --coverage` (v8 provider). Plain `ng test` hangs in watch mode and is **not** used in CI; running `npx vitest` directly is also avoided (globals/env are only wired via `ng test`).
5. **Enforced in CI** on branch `feat/round-3-coverage-gate` via `scripts/coverage-gate.sh`, alongside a **lint/format** check (`dotnet format` + Prettier/ESLint). Branch protection requires the `backend`, `frontend`, and coverage checks to pass before merge.
6. **Scope of the blocking run.** The **4 backend correctness projects** (`Domain`, `Data`, `Service`, `Api` tests) + the **frontend Vitest** run are blocking. `DocAnalytics.Performance.Tests` (flaky wall-clock timing) is **excluded** from the blocking gate and reports separately.

## Implementation notes

- **Files:** `coverage.runsettings` (excludes), `coverage.ps1` (local run + HTML report + summary; `-Open` to open), `scripts/coverage-gate.sh` (CI enforcement), `docs/coverage-baseline.md` (baseline + rationale), `.github/workflows/ci.yml` (coverage step in the backend/frontend jobs).
- **Local backend run:** `./coverage.ps1` (clean → test with collector → ReportGenerator → print `Summary.txt`).
- **Local frontend run:** `cd docanalytics-web && ng test --watch=false --coverage`.
- **Ratchet workflow:** after annotating infra with `[ExcludeFromCodeCoverage]`, re-measure → Api/Data percentages jump → raise those floors in `scripts/coverage-gate.sh`.

## Consequences

**Positive**
- Coverage regressions are caught **per-layer**, where they matter, without punishing infrastructure-heavy assemblies.
- Excluding true infra via attributes keeps the numbers **honest** and the gate **meaningful**.
- Floors ratchet upward as coverage improves, so quality trends monotonically.
- The same discipline spans backend (coverlet) and frontend (Vitest).

**Negative / trade-offs**
- Per-layer floors are **more configuration** to maintain than a single number.
- `[ExcludeFromCodeCoverage]` can be **abused** to hide genuinely testable logic — reviewers must ensure it is applied **only** to true infrastructure, not to dodge writing a test.
- Baseline numbers **drift**; the table in this ADR is a point-in-time reference — the **authoritative** floors live in `scripts/coverage-gate.sh` / `coverage.runsettings`.
- Excluding the Performance project from the gate means perf regressions aren’t caught by coverage (they’re covered separately by the NFR performance tests + the JMeter load test).

## Testing & verification

- CI fails the PR if any layer’s line coverage is below its floor, or if `dotnet format` / Prettier reports unformatted files.
- The frontend job runs `ng test --watch=false` (add `--coverage` for the gate); a green run reports e.g. Statements ~70% / Branches ~70% / Functions ~58% / Lines ~73% for `docanalytics-web`.

## Related PRs / commits

- `R3 CI/CD: coverage gate + lint/format (dotnet format + Prettier)`
- `Test: fix coverage runsettings + expand service-layer unit tests (35% → 81% line, 30% → 66% branch)`
- `test: restructure to per-layer test projects with strict-mock unit testing`

## Follow-ups

- Continue ratcheting Api/Data floors upward as controllers/services gain tests and infra is annotated.
- Consider adding branch-coverage floors (currently informational) once line floors are comfortably met.
- Optionally publish the ReportGenerator HTML as a CI artifact for easy inspection on failed runs.
