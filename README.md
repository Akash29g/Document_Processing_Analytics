# Document Processing Analytics Platform

A **multi-tenant document-processing analytics & monitoring backend** built with **ASP.NET Core (.NET 10)** and **PostgreSQL 18**. It ingests batches of documents (PDF invoices, CSV manifests), tracks each file through a pipeline (**Upload → Validate → Transform → Load**), records errors + remediation, extracts invoice line items, and serves dashboards/analytics over a clean REST API.

> **📖 Full docs live in the [Wiki](../../wiki)** — architecture, database, isolation model, API examples, and troubleshooting. This README just gets you running.

> **Status:** Backend feature-complete (Phase 0 + all 5 rounds + site-level access enforcement / FR-5.3). Frontend (Angular, DT-3) next.

---

## Quick Start

### Prerequisites
- **.NET SDK 10.x** · **PostgreSQL 18** · **Git** · `dotnet-ef` global tool
  (`dotnet tool install --global dotnet-ef`, then open a fresh terminal)
- Remember the **`postgres`** superuser password you set during install.

### 1. Clone & open
```bash
git clone https://github.com/Akash29g/Document_Processing_Analytics.git
cd Document_Processing_Analytics
```
Open **`DocAnalytics.slnx`** (don't create a new project).

### 2. Set secrets (git-ignored)
```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=docanalytics;Username=postgres;Password=YOUR_LOCAL_PW" --project DocAnalytics.Api
dotnet user-secrets set "Jwt:Key" "any-32+-character-secret-key-for-local-dev" --project DocAnalytics.Api
```
> `Jwt:Key` must be ≥ 32 chars. Username/password must match a real PostgreSQL role.

### 3. Create the database (EF Core does it all)
```bash
dotnet ef database update --project DocAnalytics.Data --startup-project DocAnalytics.Api
```
If `docanalytics` doesn't exist, EF creates it, then builds all 12 tables. No hand-written SQL — the schema lives in migrations.

### 4. Run
```bash
dotnet watch run --project DocAnalytics.Api   # auto-opens Swagger + hot-reload
# or: dotnet run --project DocAnalytics.Api    # then open http://localhost:<port>/swagger
```
First run also **seeds** the database (2 tenants, users, batches, etc.).

### 5. Log in
Seed users (password `Password123!` for all): `user.a@acme.com`, `admin@acme.com`, `user.b@acme.com`, `user.c@globex.com`, `admin@globex.com`. Full roster + smoke test in the **[API Reference wiki page](../../wiki/API-Reference)**.

---

## Tech Stack
ASP.NET Core Web API (**.NET 10**) · EF Core 10 (migration-based) · **PostgreSQL 18** · JWT auth · BCrypt · Swagger/Swashbuckle · snake_case via EFCore.NamingConventions.

> Design docs say ".NET 8" but the project targets **.NET 10 (`net10.0`)** — that's the source of truth.

## Architecture (one-liner)
`Api → Service → Data → Domain`. Controllers never touch `AppDbContext` — they call a service; an EF global query filter enforces tenant/site scoping. Details: **[Architecture wiki](../../wiki/Architecture)**.

## Tenant & Site Isolation
Four layers (data columns → JWT → global query filter → `UserSiteAccess` 403 enforcement / FR-5.3) keep customers and sites separate. Details: **[Tenant and Site Isolation wiki](../../wiki/Tenant-and-Site-Isolation)**.

## Git Workflow
`main` is the runnable baseline; work on `feature/*` or `fix/*` branches → PR → merge. Conventional Commits (`feat:`, `fix:`, `docs:`, …). Details: **[Git Workflow wiki](../../wiki/Git-Workflow)**.

## Troubleshooting
Common errors (`28P01`, `IDX10720`, empty results after DB reset) are covered in **[FAQ and Troubleshooting wiki](../../wiki/FAQ-and-Troubleshooting)**.

---

## Frontend (docanalytics-web)

The frontend is an **Angular 22** single-page app (standalone components + Signals) located in `docanalytics-web/`. It consumes the backend REST API under `/api/v1` and renders the Dashboard, Batch Explorer, Error Analysis, and Activity Log.

### Tech Stack

| Layer | Choice |
|---|---|
| Framework | Angular 22 (standalone components, Signals) |
| Routing | Lazy-loaded, nested/parameterized routes (`/site/:siteId/...`) |
| State | Angular Signals inside injectable services (one service per feature) |
| HTTP | `HttpClient` + functional interceptors (auth + site, global error) |
| Styling | CSS variables (AVEVA purple/white theme), functional-first layout |

### Prerequisites

- **Node.js 22+** and npm
- **Angular CLI 22** — `npm install -g @angular/cli`

### Setup & Run

```Windows Powershell
cd docanalytics-web
npm install        # one-time — installs dependencies (node_modules is git-ignored)
ng serve -o        # serves at http://localhost:4200
```

---

## Code Coverage

### Backend (coverlet + ReportGenerator)

One-time setup:

```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Run:

```powershell
.\coverage.ps1 -Open   # runs all test projects -> HTML report in ./coverage-report
```

Covered layers: **Controllers** (Api.Tests, ~100%), **Services with business logic** (Service.Tests),
**tenant isolation** (Data.Tests), **Domain entities** (Domain.Tests).
Excluded from coverage: EF migrations, seeding, DI extensions, and external integrations
(S3, Bedrock, SignalR, SMTP) — these require integration tests, not unit tests.

### Frontend (Vitest)

```powershell
cd docanalytics-web
ng test --coverage --watch=false   # coverage table in terminal + report in ./coverage
```

Covered: guards, interceptors, core services, feature services and shared components (~76% lines).

---


## Team
- **Dev A** — Akash Goswami
- **Dev B** — Shubh Gupta

Full design rationale: `Design_Tasks_1-3_updated.pdf`.
