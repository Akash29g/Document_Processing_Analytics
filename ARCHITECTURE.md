# DocAnalytics — Architecture Overview

> **Living architecture document** for **DocAnalytics**, a multi-tenant document-processing analytics platform.
> This document is the single entry point for understanding the system: what it does, how it is built, how data flows, how it is secured, and how it is deployed. It is the companion to the Architecture Decision Records (ADRs) under [`docs/adr/`](docs/adr/) — this document captures the *what* and *how*; the ADRs capture the *why* behind each significant decision.
>
> **Maintainers:** Akash (Dev A, `@akash29g`) · Shubh (Dev B, `@g9shubh`). CODEOWNERS lists both on everything (cross-review enforced).

---

## Table of contents

1. Product & business context
2. Requirements traceability (FR / NFR)
3. Technology stack
4. Solution architecture (layers)
5. Data model
6. API design conventions
7. Frontend architecture
8. Multi-tenancy & isolation
9. Authentication & authorization
10. Realtime (SignalR)
11. Invoice extraction pipeline (S3 + Bedrock)
12. AWS infrastructure
13. CI/CD pipeline
14. Security posture (hardening summary)
15. Local development
16. Delivery model & conventions
17. Hardening round history (timeline)
18. Architecture Decision Records (index)
19. Glossary

---

## 1. Product & business context

DocAnalytics is an **analytics and monitoring web application** layered over a cloud document-processing pipeline. The underlying platform serves multiple **customers (tenants)**, each with one or more **sites** (physical locations such as factories or plants). Documents are uploaded in **batches**; every file then flows through a multi-step pipeline:

```
Upload → Validate → Transform → Load / Publish
```

Files can succeed, fail, or get stuck at any step. Operations teams use DocAnalytics to:

- Watch pipeline health at a glance (dashboards, counters, charts).
- Drill into batches and individual files, down to per-step history.
- Analyse failures and apply remediation guidance.
- Read a tamper-evident audit trail (activity log).
- Configure email/in-app alerts when the failure rate crosses a threshold.

On top of the generic pipeline, DocAnalytics adds an **AI invoice-extraction** capability: uploaded invoices are parsed by AWS Bedrock (Amazon Nova) into a structured header + line-items model for downstream analytics.

**Hard rule (business + security):** every tenant + site combination is completely isolated — one customer must **never** see another customer’s data.

### Core capabilities

| Area | Capability |
|---|---|
| **Dashboard (FR-1)** | Status counters (Queued/In Progress/Completed/Failed), throughput chart, status-distribution chart, recent-failures table (sortable, paginated), 30s auto-refresh, per-step processing-time percentiles (P50/P90/P99). |
| **Batch Explorer (FR-2)** | Paginated/filterable/searchable batch list, batch detail with per-status file counts, files table, file step-history drill-down (timeline + error + remediation). |
| **Error Analysis (FR-3)** | Top-10 errors, error-trend chart, remediation messages, filtered error list, CSV export. |
| **Activity Log (FR-4)** | Append-only chronological audit trail, filterable by event type/entity/date, paginated. |
| **Tenant & Site (FR-5)** | Multi-tenant/site selection, all data scoped to the selection, server + client authorization. |
| **Invoice extraction** | Presigned S3 upload → GuardDuty malware-scan gate → magic-byte check → Bedrock Nova extraction → validation → structured persistence (header + line items + categories). |
| **Alerts (S-4)** | Failure-rate threshold rules → email + in-app notifications (bell), background evaluator. |
| **Realtime (S-1)** | SignalR live pipeline updates (site-scoped groups). |
| **Provisioning / Admin** | Role-based tenant onboarding (Developer console) + user/site management (Admin). |
| **Stretch** | Dark mode (S-2), role-based access (S-3), throughput comparison view (S-6). |

---

## 2. Requirements traceability (FR / NFR)

| Requirement | Where satisfied |
|---|---|
| FR-1 Dashboard | `DashboardController` + `DashboardAnalyticsController`; Angular `features/dashboard`. |
| FR-2 Batch Explorer | `BatchesController`, `FilesController`; `features/batches`, `features/files`. |
| FR-3 Error Analysis | `ErrorsController`, `ErrorAnalyticsController`; `features/errors`. |
| FR-4 Activity Log | `ActivityLogController`; `features/activity-log`. |
| FR-5 Tenant/Site | `AuthController`, `SitesController`, `TenantSiteMiddleware`; `SiteContextService`, guards. |
| NFR-1 Performance | Pre-aggregated counters, offset pagination (max 100), targeted indexes; proven by `DocAnalytics.Performance.Tests` + a JMeter load test (10 users, APDEX 1.0, 0% errors). |
| NFR-2 Usability | Responsive layout (1024–1920px), loading/skeleton states, inline error + retry (`SKIP_ERROR_TOAST`), dark mode. |
| NFR-3 Security | JWT auth, EF global query filters, parameterized LINQ only, input validation; see ADRs 0001–0004. |
| NFR-4 Reliability | `/api/v1/health` DB-connectivity check; frontend never shows a blank screen on error. |
| NFR-5 Maintainability | Layered separation (Api/Service/Data/Domain), EF migrations, consistent `{ data, meta, error }` envelope. |

---

## 3. Technology stack

| Layer | Choice | Notes |
|---|---|---|
| Backend | ASP.NET Core Web API, **.NET 10** | Controller-based, vertical-slice services. |
| Solution layers | `DocAnalytics.Api` → `.Service` → `.Data` → `.Domain` | Dependencies point downward only. |
| ORM | Entity Framework Core 10 | Parameterized LINQ → SQL; migration-based schema. |
| Database | **PostgreSQL** (AWS RDS, ap-south-1) | snake_case via `EFCore.NamingConventions`; `db.t4g.micro`, gp3, single-AZ. |
| Frontend | **Angular 22** | Standalone components + Signals; `docanalytics-web/`. |
| Frontend tests | **Vitest** (via `ng test`) | NOT Jasmine/Karma. |
| Auth | JWT Bearer + DB-backed rotating refresh tokens | Refresh token in HttpOnly cookie. |
| Realtime | SignalR (`PipelineHub`) | Site-scoped groups. |
| Storage / AI | Amazon S3 + Amazon Bedrock **Nova 2 Lite** (`us.amazon.nova-2-lite-v1:0`) | Presigned S3 PUT/GET; Converse API for extraction. |
| Malware scan | GuardDuty Malware Protection for S3 | Object-tagging based, fail-closed gate. |
| Edge / hosting | Docker → **AWS ECS Fargate** behind an **ALB** | nginx serves the SPA + reverse-proxies. |
| IaC | **Terraform** (`infra/`) | ~26 resources, all live. |
| CI/CD | **GitHub Actions** (`ci.yml`, `deploy.yml`) | OIDC to AWS; gated EF migrations. |
| Domain / TLS | `docanalytics.dev` (Route 53 + ACM) | HTTPS at the ALB, HTTP→HTTPS redirect. |

---

## 4. Solution architecture (layers)

Clean, layered separation of concerns (NFR-5):

```
DocAnalytics.Api        Controllers, middleware, DI wiring, Program.cs (composition root)
   │  depends on
   ▼
DocAnalytics.Service    Business logic — feature-sliced services + AddXxxFeature() DI extensions
   │  depends on
   ▼
DocAnalytics.Data       AppDbContext, EF migrations, DbSeeder, DataProtection wiring
   │  depends on
   ▼
DocAnalytics.Domain     Entities, ITenantScoped / ICurrentUser abstractions (no dependencies)
```

- **Api** — thin controllers returning the `ApiResponse<T>` envelope; middleware pipeline (exception handling, security headers, tenant/site resolution); rate limiting; Swagger (dev only).
- **Service** — one feature slice per capability (Auth, Batches, Dashboard, Errors, Analytics, Alerts, Uploads, Extraction, Provisioning, AdminUsers, ActivityLog, Files, Invoices, Health). Each registers itself via an `AddXxxFeature()` extension. Services take `AppDbContext` and rely on the global query filter for isolation.
- **Data** — `AppDbContext` (global query filters, snake_case, DataProtection key context), migrations, `DbSeeder` (split into catalog seeding vs demo seeding — see §9/hardening).
- **Domain** — POCO entities and the `ITenantScoped` / `ICurrentUser` contracts. Zero framework dependencies.

---

## 5. Data model

12 tables across 4 logical layers (see `Design_Tasks_1-3_updated.pdf` / DT-1 for full column detail).

**Identity:** `tenants`, `sites`, `users`, `user_site_access` (N:M bridge).
**Pipeline:** `transactions` (the batch “TId” rows, with **pre-aggregated** counters), `files` (~1M rows), `file_step_history` (one row per step — queryable, not a JSON blob).
**Support:** `error_catalog` (codes + remediation), `activity_log` (append-only audit).
**Extraction:** `document_types`, `invoice_line_items`, `item_categories`, plus `invoice_headers` (1:1 with file, added later).

### Key data-model decisions

- **Pre-aggregated counters** on `transactions` (uploaded/processing/failed/completed) so the dashboard loads instantly instead of counting ~1M rows live (NFR-1). On any file status change: recompute counters → recompute `state` (`failed_count>0 → Failed`; else in-flight → `Processing`; else `Completed`) → update `last_updated_at` → write an `activity_log` entry.
- **Denormalized `tenant_id` + `site_id`** on high-volume tables so isolation filters never require a JOIN.
- **One row per step** in `file_step_history` → steps are queryable, sortable, groupable, indexable.
- **UUID (v4) PKs** everywhere; **VARCHAR statuses** so new statuses ship without migrations.
- **Targeted indexes** for each hot path (load-reporting view, recent failures, batch drill-down, error-by-step, invoice line items, activity feed, auth check).

---

## 6. API design conventions

(See DT-2 for the full endpoint catalogue.)

- **Base URL / versioning:** everything under `/api/v1` (path versioning → `/api/v2` can ship without breaking clients).
- **Response envelope (NFR-5):** single resource → `{ data, error }`; list → `{ data, meta, error }` where `meta = { total_count, page, page_size, total_pages }`. On success `error` is null; on failure `data` is null. snake_case JSON (`JsonNamingPolicy.SnakeCaseLower`).
- **Pagination:** offset-based (`page` + `pageSize`, hard cap 100) — chosen over cursor because the UI needs jump-to-page and total-page counts.
- **Filtering/sorting:** query-string only (never a body on GET). `sortBy` is validated against a whitelist and translated to an EF `OrderBy` — no string concatenation (injection-safe).
- **Status codes:** 200 OK, 400 validation, 401 unauth, 403 site-forbidden, **404 cross-tenant resource** (no existence leak — see ADR-0003), 500 unhandled.
- **Auth header:** every endpoint except `POST /auth/login` and `GET /health` requires `Authorization: Bearer <jwt>` (+ `X-Site-Id` for data routes).
- **Errors:** standard error body `{ code, message, details }`.

---

## 7. Frontend architecture

(See DT-3.)

- **Framework:** Angular 22, standalone components, Signals reactivity, `OnPush`.
- **Routing:** nested/parameterized routes; site + resource IDs live in the URL (bookmarkable, deep-linkable, Back-button safe). Lazy-loaded feature areas.
  - `/login`, `/site/:siteId/dashboard`, `/site/:siteId/batches`, `/site/:siteId/batches/:batchId`, `/site/:siteId/batches/:batchId/files/:fileId`, `/site/:siteId/errors`, `/site/:siteId/activity-log`, plus `/provision`, `/admin`, `/alerts`, `/change-password`.
  - Guards: `authGuard` (token present + rehydrate via `/auth/me`), `siteAccessGuard` (mirrors server FR-5.3).
- **Components:** atomic design — atoms (`StatusBadge`, `StatCard`, `AppButton`), molecules/organisms (`DataTable`, `SiteSelector`, `RefreshTimer`, `ChartCard`, `FilterBar`).
- **State:** Signals inside injectable services (one per feature). Each exposes `data` / `loading` / `error` signals per slice via `signal()` + `.asReadonly()`.
- **Tenant/site sync:** `SiteContextService` holds `selectedSiteId`, kept in sync with the `:siteId` URL param (source of truth).
- **HTTP plumbing:** `authSiteInterceptor` attaches `Authorization: Bearer` + `site_id`/`X-Site-Id`; `errorInterceptor` maps the `{ error }` envelope to a toast + inline message; `SKIP_ERROR_TOAST` `HttpContext` token opts individual calls out of the global toast so widgets show inline errors + Retry (never a blank screen, NFR-2).
- **Auto-refresh:** `RefreshTimerService` (`timer(0, 30s)`), paused when the tab is hidden, `takeUntilDestroyed`.
- **Theme:** dark mode via CSS variables persisted to LocalStorage (`theme.service.ts`, signal-based).

---

## 8. Multi-tenancy & isolation

- Every pipeline/support/extraction table carries `tenant_id` + `site_id`.
- The JWT carries `{ userId, tenantId, role }`; the requested site arrives as the `X-Site-Id` header/query param.
- `TenantSiteMiddleware` validates the JWT, resolves the current user/tenant/site, enforces site access (403 `SITE_FORBIDDEN`), populates `CurrentUser`, and hard-blocks the `Developer` role from data routes.
- An **EF Core global query filter** on every `ITenantScoped` entity auto-appends `WHERE tenant_id = X AND site_id = Y` — isolation cannot be forgotten.
- Cross-tenant reads return **404** (filtered row is invisible), never 403 — no existence leak.

Full design, non-scoped-entity handling, and the automated proofs are in **ADR-0003**.

---

## 9. Authentication & authorization

- **Roles:** `Developer` (platform super-admin, `TenantId = null`, no data access), `Admin`, `Viewer`. Enforced by a DB check constraint `role IN ('Developer','Admin','Viewer')`.
- **Policies:** `DeveloperOnly`, `AdminOnly`, `DataAccess` (Admin|Viewer). Every data controller is `[Authorize(Policy = "DataAccess")]`.
- **Access tokens:** short-lived JWT (20 min in production), custom claims, `MapInboundClaims = false`, `RoleClaimType = "role"`.
- **Refresh tokens:** DB-backed, rotating, hashed; delivered as an **HttpOnly / Secure / SameSite=Strict** cookie scoped to `/api/v1/auth`.
- **Login protection:** rate-limiting + account lockout (5 failures → 15-min lock); **BCrypt** hashing.
- **Provisioning:** Developer console onboards tenants (unique `org_domain`) + creates admins; Admin page creates Viewers (auto `first.last@org` email, temp password, forced reset via `must_change_password`).

Full design in **ADR-0004**.

---

## 10. Realtime (SignalR)

- `PipelineHub` at `/hubs/pipeline`, site-scoped groups. Pushes live file/batch state changes to the dashboard (S-1) instead of relying solely on polling.
- The nginx `/hubs/` location carries WebSocket upgrade headers; the ALB path-routes `/hubs/*` to the API service.
- `SignalRPipelineNotifier` implements `IPipelineNotifier`; the SPA uses an `accessTokenFactory` for the hub connection.

---

## 11. Invoice extraction pipeline (S3 + Bedrock)

```
Browser ─(presigned PUT)─▶ S3  ──▶ GuardDuty malware scan (async object tag)
   │                                    │
   ▼  POST /files/{id}/complete         ▼
ExtractionQueue (Channel<Guid>) ─▶ ExtractionWorker (BackgroundService)
   │  1. Malware gate: poll GetMalwareScanStatusAsync tag (~60s); THREATS_FOUND → delete + ERR_MALWARE_DETECTED
   │  2. Magic-byte gate: content must start %PDF- else ERR_INVALID_FILETYPE + delete
   │  3. Download from S3 → NovaInvoiceExtractor (Bedrock Converse, fence-strip, JSON parse)
   │  4. InvoiceValidator (line-sum vs subtotal + grand-total reconciliation → confidence)
   └─ 5. Persist InvoiceHeader (1:1) + InvoiceLineItem[] (+ auto-created ItemCategory), drive status/counters
```

- **Model:** Amazon **Nova 2 Lite** via the Converse API (chosen after Claude was blocked by an AISPL India Marketplace payment restriction). S3 in ap-south-1, Bedrock in us-east-1 (code supports split regions).
- **Worker gotchas:** no JWT context → `IgnoreQueryFilters()` then `CurrentUser.Set(...)`; idempotent re-insert; uploads tagged `SourceSystem="Manual_Upload"`.
- **Null-safety:** GuardDuty tags asynchronously; the tag read is null-safe (`res.Tagging?.FirstOrDefault(...)?.Value`) so the poll loop tolerates the pre-scan untagged window.

---

## 12. AWS infrastructure

All live in **account 323155024771 / ap-south-1 (Mumbai)**, provisioned via Terraform (`infra/`).

| Service | Detail |
|---|---|
| **ECR** | 3 repos: `docanalytics-api`, `-web`, `-migrations` (SHA-tagged images). |
| **ECS** | Cluster `docanalytics-cluster`; services `docanalytics-api-svc` (:8080) + `docanalytics-web-svc` (nginx :80). Fargate. |
| **ALB** | `docanalytics-alb`; target groups `tg-api:8080`, `tg-web:80`; HTTPS:443 + HTTP→HTTPS redirect; rules `/api/*` + `/hubs/*` → tg-api, default → tg-web. |
| **ACM / Route 53** | Cert for `docanalytics.dev` + `www`; hosted zone A/Alias → ALB. |
| **RDS** | PostgreSQL `docanalytics-db` (db.t4g.micro, gp3 20GB, single-AZ). |
| **S3** | `docanalytics-invoices` bucket, CORS for the prod domain + localhost, GuardDuty Malware Protection on. |
| **Bedrock** | Nova 2 Lite (us-east-1). |
| **IAM** | `docanalytics-gha-deploy` (OIDC, scoped to main), `-task-exec` (reads `docanalytics/*` secrets), `-task-role` (S3 + Bedrock). |
| **Secrets Manager** | `docanalytics/rds-conn`, `docanalytics/jwt-key` (+ planned `SuperAdmin__*`). |
| **CloudWatch** | Log groups `/ecs/docanalytics-{api,web,migrate}`. |
| **Security groups** | alb-sg (80/443 in), task-sg (8080/80 from ALB), RDS (5432 from task-sg). |

---

## 13. CI/CD pipeline

**CI (`.github/workflows/ci.yml`)** — runs on PR + push to main; `concurrency` cancels superseded runs:

- **backend** job: restore/build `DocAnalytics.slnx` (Release) → `dotnet test` on the 4 correctness projects (Perf excluded).
- **frontend** job (parallel): `npm ci` → `npm run build` → `ng test --watch=false`.
- **devsecops** job: gitleaks (working-tree, via Docker image), `dotnet list --vulnerable`, `npm audit`, Trivy image scans (via Docker image). Dockerfiles patch OS CVEs (`apk upgrade` / `apt upgrade`).
- **e2e** job: Playwright suite (with a test-only `Jwt__Key` fallback so Dependabot PRs can boot the API).
- **Coverage gate + lint/format** (`dotnet format` + Prettier) — see ADR-0005.

**CD (`.github/workflows/deploy.yml`)** — on push to main:

1. OIDC assume `docanalytics-gha-deploy`; ECR login.
2. Build SHA-tagged API/Web images → push to ECR.
3. **Gated EF migration** as an in-VPC ECS task (efbundle; exit-code 0 gate → no half-migrated deploys).
4. Render + deploy both ECS services; wait for stability.
5. **Smoke test** `GET /api/v1/health` → expect 200 + `"db":"connected"`.

**Dependabot** (`.github/dependabot.yml`): weekly NuGet (`/`), npm (`/docanalytics-web`), GitHub Actions.

---

## 14. Security posture (hardening summary)

Delivered across the Security/CI-CD rounds and the `harden/prod-security` pass (PR #113):

| # | Item | Summary |
|---|---|---|
| 1 | Seeder gating | Reference catalogs seed in all envs; demo tenants/users + startup migration are **Development-only** (no demo creds in prod). |
| 2 | Super-admin bootstrap | `SuperAdminSeeder` creates the platform admin from secrets, `MustChangePassword=true`. |
| 3 | Swagger | Gated to `IsDevelopment()` only. |
| 4 | CORS + HSTS | Prod origins locked via `appsettings.Production.json`; HSTS 1yr/includeSubDomains/preload; forwarded-headers for nginx TLS. (**ADR-0001**) |
| 5 | CSP | Restrictive `default-src 'none'` on the JSON API + SPA CSP on nginx. (**ADR-0002**) |
| 6 | Password policy | Min-length + complexity + **HaveIBeenPwned** k-anonymity breach check (3s fail-open). |
| 7 | Token hardening | Access TTL → 20 min; refresh token → HttpOnly/Secure/SameSite=Strict cookie. (**ADR-0004**) |
| 8 | Dependabot | Weekly NuGet/npm/Actions updates. |
| 9 | Ops hygiene | Prod log levels tightened (no EF SQL/params at Information); startup migration Dev-only; no secrets logged. |

Plus DevSecOps scanning in CI, DataProtection keys in Postgres, GuardDuty malware gate + magic-byte check on uploads, and gitleaks with rotated-key fingerprints.

---

## 15. Local development

- **Backend:** `dotnet watch run --project DocAnalytics.Api` → Swagger at `https://localhost:7001/swagger` (auto-seeds on first run). Secrets via user-secrets: `ConnectionStrings:Default`, `Jwt:Key` (≥ 32 chars).
- **Frontend:** `cd docanalytics-web && npm install && ng serve` → `http://localhost:4200` (proxy forwards `/api`).
- **Docker (full stack):** `./rebuild.ps1` (compose: `api` :5001, `web` :4200). No ALB locally → nginx reverse-proxies `/api` + `/hubs` to the `api` container using **lazy DNS** (`resolver 127.0.0.11` + `set $api_host`).
- **Tests:** `dotnet test` (backend), `ng test` (frontend, Vitest). EF migration: `dotnet ef migrations add <Name> --project DocAnalytics.Data --startup-project DocAnalytics.Api`.
- **Seed logins** (`Password123!`): `developer@platform.com` (Developer), `admin@acme.com` (Admin), `user.a@acme.com` (Viewer). Tenants: Acme, Globex.
- **Env notes:** Windows/PowerShell; line-continuation is backtick `` ` `` not `\`. `.slnx` solution format.

---

## 16. Delivery model & conventions

- **Two-dev parallel rounds:** each round both devs own a complete end-to-end slice, then cross-review. Domains alternate (Security ↔ CI/CD).
- **Git flow:** `main` is always runnable; work on `feature/*` / `fix/*` / `ci/*` / `harden/*` branches → PR → CODEOWNERS cross-review → merge. **Conventional Commits** (`feat:`, `fix:`, `docs:`, `chore:`, `ci:`, `security:`).
- **Co-owned files** (`app.routes.ts`, shell, shared services): hand-merge both halves — never blind “accept both”.
- **Branch protection:** green CI (`backend`, `frontend`, coverage) + 1 review required.
- **Backend patterns:** `AddXxxFeature()` DI extensions; thin controllers; services take `AppDbContext`.
- **Frontend patterns:** one signal-based service per feature; shared `DataTable`/`FilterBar`/`ChartCard`; `SKIP_ERROR_TOAST` for inline errors; guard reload effects with `untracked()`.

---

## 17. Hardening round history (timeline)

| Round | Akash (Dev A) | Shubh (Dev B) |
|---|---|---|
| **R0 Foundation** | gitleaks foundation, config-driven `SecurityOptions`, DataProtection keys → Postgres, coverage baseline, root `.editorconfig`. | (pair) |
| **R1** | **Security:** transport hardening — `SecurityHeadersMiddleware`, CORS/HSTS, pipeline order. | CI: core `ci.yml` (parallel backend + frontend). |
| **R2** | **CI/CD:** DevSecOps job (gitleaks + `dotnet list --vulnerable` + npm audit + Trivy) + Dependabot + Dockerfile OS patching. | Security: login rate-limit/lockout + secrets audit. |
| **R3** | **Security:** cross-tenant 404 test + `[Authorize]` audit + non-scoped-entity guard. | CI: coverage gate + lint/format. |
| **R4 (Step A pair)** | AWS deploy target + ECR + GitHub→AWS OIDC. | (pair) |
| **R4** | **CI/CD:** deploy pipeline — Terraform infra + ECS deploy job (ECR, gated EF migrations, smoke test), HTTPS custom domain, S3 via task role. | Security: DB-backed rotating refresh tokens (15/20-min access + silent refresh). |
| **R5** | **API rate limiting** (login / reads / export). | Activity Log polish, dark mode, responsive pass. |
| **Later** | Playwright E2E suite + CI job; XML doc pass (Swagger); GuardDuty S3 malware-scan gate (fail-closed); **`harden/prod-security`** (9-item prod hardening pass, PR #113). | (cross-review) |

---

## 18. Architecture Decision Records (index)

| ADR | Title | Status |
|---|---|---|
| [0001](docs/adr/0001-transport-hardening.md) | Transport hardening (HSTS, security headers, CORS lock-down) | Accepted |
| [0002](docs/adr/0002-content-security-policy.md) | Content Security Policy (API + SPA) | Accepted |
| [0003](docs/adr/0003-tenant-isolation.md) | Tenant isolation via EF Core global query filters | Accepted |
| [0004](docs/adr/0004-auth-refresh-tokens.md) | JWT auth + DB-backed rotating refresh tokens | Accepted |
| [0005](docs/adr/0005-coverage-gates.md) | Per-layer code coverage gates in CI | Accepted |

---

## 19. Glossary

- **Tenant** — a customer company; **Site** — a physical location within a tenant. Data is isolated per tenant+site.
- **Batch / Transaction** — a group of files submitted together (the “TId” row) with pre-aggregated counters.
- **ITenantScoped** — marker interface (`TenantId`, `SiteId`) that opts an entity into the global query filter.
- **CurrentUser** — request-scoped `ICurrentUser` (userId/tenantId/siteId/role) that drives the filter, set by `TenantSiteMiddleware`.
- **Envelope** — the `{ data, (meta), error }` response contract.
- **APDEX** — Application Performance Index; the JMeter load test hit 1.0 with 0% errors at 10 concurrent users (NFR-1).

---

*Update this document whenever a significant architectural decision lands, and add a matching ADR under `docs/adr/`.*
