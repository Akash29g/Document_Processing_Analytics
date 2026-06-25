# Document Processing Analytics Platform

A **multi-tenant document-processing analytics & monitoring backend** built with **ASP.NET Core (.NET 10)** and **PostgreSQL 18**. It ingests batches of documents (PDF invoices, CSV manifests), tracks each file through a processing pipeline (**Upload → Validate → Transform → Load**), records errors and remediation guidance, extracts invoice line items, and exposes dashboards/analytics over a clean REST API.

> **Status:** Backend in active development. **Phase 0 (Foundation)** and the **Round 1 Auth slice** are complete; the **Batches list** slice is in review. See [Roadmap & Status](#roadmap--status).

---

## Table of Contents

1. [Overview](#overview)
2. [Tech Stack](#tech-stack)
3. [Architecture](#architecture)
4. [Database Schema](#database-schema)
5. [Tenant Isolation](#tenant-isolation)
6. [Prerequisites](#prerequisites)
7. [Database Setup](#database-setup)  ← **start here if you are cloning**
8. [Getting Started](#getting-started)
9. [Running & Testing](#running--testing)
10. [API Reference](#api-reference)
11. [Project Structure](#project-structure)
12. [Conventions & Contracts](#conventions--contracts)
13. [Git Workflow](#git-workflow)
14. [Roadmap & Status](#roadmap--status)
15. [Team](#team)

---

## Overview

The platform serves multiple customer companies (**tenants**), each with one or more **sites** (physical locations). Every **tenant + site** combination is completely isolated — one customer can never see another customer's data.

**Core capabilities (target):**

- **Dashboard** — status counters, throughput, status distribution, recent failures.
- **Batch Explorer** — paginated/filterable batches, drill-down into files and step history.
- **Error Analysis** — top errors, trends, filtered list, CSV export, remediation messages.
- **Activity Log** — append-only, paginated audit trail.
- **Auth & Tenant/Site selection** — JWT login, session rehydration, site listing.

---

## Tech Stack

| Layer | Choice |
|---|---|
| Backend | ASP.NET Core Web API (controller-based), **.NET 10** |
| ORM / Data access | Entity Framework Core 10 (migration-based, parameterized LINQ) |
| Database | **PostgreSQL 18** |
| Auth | JWT Bearer tokens (ASP.NET Core authentication middleware) |
| Password hashing | BCrypt.Net-Next |
| API docs | Swagger / OpenAPI (Swashbuckle) |
| Naming | C# PascalCase → DB snake_case via EFCore.NamingConventions |

> **Note:** the original design docs referenced “.NET 8”, but the project targets **.NET 10 (`net10.0`)**. Treat .NET 10 as the source of truth.

### Key package versions

- **EF Core 10.0.9** (`Microsoft.EntityFrameworkCore` + `.Relational` pinned to 10.0.9 in the Data project)
- **Npgsql.EntityFrameworkCore.PostgreSQL 10.0.2** (the PostgreSQL provider for EF Core)
- **EFCore.NamingConventions 10.0.1** (snake_case)
- **Microsoft.AspNetCore.Authentication.JwtBearer 10.0.9**
- **Swashbuckle.AspNetCore 10.2.1** (Microsoft.OpenApi 2.x)
- **BCrypt.Net-Next 4.2.0**
- **System.IdentityModel.Tokens.Jwt** (Service project, for JWT issuance)

---

## Architecture

Layered solution with **strict one-way dependencies**:

```
Api  →  Service  →  Data  →  Domain      (Domain depends on nothing)
```

| Project | Responsibility |
|---|---|
| **DocAnalytics.Domain** | Entities + contracts (`ITenantScoped`, `ICurrentUser`). No dependencies. |
| **DocAnalytics.Data** | `AppDbContext`, migrations, `DbSeeder`, `AddPersistence()`. References Domain. |
| **DocAnalytics.Service** | Business logic (`IXxxService` + `XxxService`), per-feature `AddXxxFeature()` DI. References Data + Domain. |
| **DocAnalytics.Api** | Controllers, middleware, JWT, `Program.cs` (composition root). References Service. |

**Golden rule:** controllers never touch `AppDbContext` directly — they call a service. The service runs the EF query; the global query filter auto-applies tenant/site scoping.

Each feature ships as its own files — `IXxxService` + `XxxService` + `XxxController` + `XxxDtos` + an `AddXxxFeature()` DI extension — so contributors don't edit shared files and merges stay clean.

---

## Database Schema

**12 tables across 4 layers (UUID primary keys throughout):**

- **Identity:** `tenants`, `sites`, `users`, `user_site_access`
- **Pipeline:** `transactions` (the “batch”), `files`, `file_step_history`
- **Support:** `error_catalog` (global), `activity_log`
- **Extraction:** `document_types` (global), `invoice_line_items`, `item_categories` (global)

**Notable entity / table mappings:**

- `FileRecord` → `files` (avoids clashing with `System.IO.File`)
- `Transaction` → `transactions` (**exposed in the API as “batch” / `/batches`**)
- `InvoiceLineItem` → `invoice_line_items`, `ItemCategory` → `item_categories`

The schema is **managed entirely via EF Core migrations — there is no hand-written SQL DDL.** The initial migration (`InitialCreate`) creates all 12 tables, indexes, and foreign keys. **The migrations are effectively your “database script”** — see [Database Setup](#database-setup).

---

## Tenant Isolation

Three layers of protection ensure **Customer A can never see Customer B**:

1. Every pipeline/support/extraction table carries `tenant_id` + `site_id`.
2. The JWT carries `userId`, `tenantId`, `role`; `site_id` comes from the request (`?site_id=` or `X-Site-Id` header).
3. An **EF Core global query filter** auto-injects `WHERE tenant_id = X AND site_id = Y` on every query against `ITenantScoped` entities (`Transaction`, `FileRecord`, `ActivityLog`, `InvoiceLineItem`).

Identity catalogs (`users`, `sites`, `user_site_access`) and global catalogs (`document_types`, `item_categories`, `error_catalog`) are intentionally **not** tenant-filtered.

> Even if a developer forgets a `WHERE` clause, the global filter protects the data.

---

## Prerequisites

Install locally (none of these are committed to the repo):

| Tool | Version | Notes |
|---|---|---|
| **.NET SDK** | **10.x** | `dotnet --version` should print `10.*` |
| **PostgreSQL** | **18** | Includes the `psql` CLI and (optionally) **pgAdmin** |
| **Git** | any recent | |
| **IDE** | Visual Studio 2026 / VS Code / Rider | |
| **dotnet-ef** global tool | latest | `dotnet tool install --global dotnet-ef` (open a **fresh** terminal afterwards so it's on your PATH) |

> 💡 During PostgreSQL installation you will set a password for the built-in **`postgres`** superuser. **Remember it** — you will need it for the connection string.

---

## Database Setup

You need a running **PostgreSQL 18** server and a database called **`docanalytics`**. You do **not** need to create any tables by hand — and you don't even need to create the database by hand: **EF Core does everything for you.**

EF Core can **create the database itself** and then apply all migrations. You only need PostgreSQL installed and the `postgres` superuser password.

1. Make sure the PostgreSQL service is running (Windows: “Services” → `postgresql-x64-18`, or it starts automatically).
2. Point your connection string at the `postgres` user (see [Getting Started → secrets](#2-set-local-secrets-postgresql--jwt)).
3. Run migrations:
   ```Windows Powershell
   dotnet ef database update --project DocAnalytics.Data --startup-project DocAnalytics.Api
   ```
   If the `docanalytics` database does not exist yet, EF runs `CREATE DATABASE docanalytics;` automatically, then creates all 12 tables, indexes, and foreign keys.

> 🔎 **Why no `schema.sql`?** Many repos ship a raw-SQL schema file because they don't use an ORM. This project is **migration-based**, so the schema lives in `DocAnalytics.Data/Migrations/`. To change the schema you create a new migration (`dotnet ef migrations add <Name> ...`) — never hand-edit the database.

### Connection string — what each field means

The connection string is **how the app talks to PostgreSQL.** Example:

```
Host=localhost;Port=5432;Database=docanalytics;Username=postgres;Password=YOUR_LOCAL_PW
```

| Field | Meaning | Typical local value |
|---|---|---|
| `Host` | Where PostgreSQL is running | `localhost` |
| `Port` | PostgreSQL listening port | `5432` (the default) |
| `Database` | The database name | `docanalytics` |
| `Username` | A PostgreSQL **login role** | `postgres` |
| `Password` | That role's password | whatever you set during install |

### Seed data

Seed data loads **automatically on first run** of the API (it runs `DbSeeder.SeedAsync`, which also applies any pending migrations). It creates **2 tenants** (Acme + Globex) with sites, users, transactions (batches), files, step history, invoice line items, categories, and an activity-log entry — enough to exercise every feature **and** to prove tenant isolation.

> The seeder is **idempotent**: it short-circuits with `if (await db.Tenants.AnyAsync()) return;`. So if you change the seed data later, you must **reset the database** first or the new seed code won't run:
>
> ```Windows Powershell
> dotnet ef database drop   --project DocAnalytics.Data --startup-project DocAnalytics.Api --force
> dotnet ef database update --project DocAnalytics.Data --startup-project DocAnalytics.Api
> dotnet run --project DocAnalytics.Api   # re-seeds on startup
> ```

---

## Getting Started

### 1. Clone & open

```Windows Powershell
git clone https://github.com/Akash29g/Document_Processing_Analytics.git
cd Document_Processing_Analytics
```

Open **`DocAnalytics.slnx`** via **File → Open → Project/Solution** (do **NOT** create a new project).

### 2. Set local secrets (PostgreSQL + JWT)

Secrets are **git-ignored**. `appsettings.json` ships with a blanked password and a placeholder JWT key, so you must provide real values via **.NET user-secrets**. These two secrets are:

- **`ConnectionStrings:Default`** — your **PostgreSQL** connection string (see [field meanings above](#connection-string--what-each-field-means)).
- **`Jwt:Key`** — the signing key for JWTs (local dev value is fine).

Run these **inside the `DocAnalytics.Api` project folder**, or add `--project DocAnalytics.Api`:

```Windows Powershell
# 1) PostgreSQL connection string  (replace YOUR_LOCAL_PW with your postgres password)
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=docanalytics;Username=postgres;Password=YOUR_LOCAL_PW" --project DocAnalytics.Api

# 2) JWT signing key  (must be at least 32 characters)
dotnet user-secrets set "Jwt:Key" "any-32+-character-secret-key-for-local-dev" --project DocAnalytics.Api
```

> ⚠️ **This is your PostgreSQL login.** `Username`/`Password` must match a real PostgreSQL role on your machine (the `postgres` superuser). If they don't match, you'll get a `28P01 password authentication failed` error.
>
> ⚠️ **`Jwt:Key` must be ≥ 32 characters**, or startup throws `IDX10720`.

Verify your secrets at any time:
```Windows Powershell
dotnet user-secrets list --project DocAnalytics.Api
```

### 3. Create the database

Follow [Database Setup](#database-setup), then:

```Windows Powershell
dotnet ef database update --project DocAnalytics.Data --startup-project DocAnalytics.Api
```

> **Why two `--project` flags?** `--project DocAnalytics.Data` is where the `DbContext` + migrations live; `--startup-project DocAnalytics.Api` is the runnable app that holds the connection string + DI. EF needs both — omitting `--startup-project` causes the `Unable to resolve service for type 'DbContextOptions<AppDbContext>'` error.

---

## Running & Testing

### Run

There are two ways to run, depending on whether you want Swagger to open for you:

```Windows Powershell
# Option 1 — auto-opens Swagger in your browser on launch (and hot-reloads on code changes)
dotnet watch run --project DocAnalytics.Api

# Option 2 — plain run; you copy the printed URL into a browser yourself
dotnet run --project DocAnalytics.Api
```

- **`dotnet watch run`** — launches the browser straight to Swagger automatically (per `launchSettings.json`: `launchBrowser: true`, `launchUrl: swagger`). It also **hot-reloads** when you edit code, so it's the nicest option during development.
- **`dotnet run`** — does **not** open a browser. Note the port from the console (e.g. `Now listening on: http://localhost:5256`) and open Swagger yourself:
  ```
  http://localhost:<port>/swagger
  ```

> The first run also seeds the database. Keep this terminal open — it's running the app. Use a **second** terminal for any `curl` / `git` / `dotnet ef` commands. Press `Ctrl + C` in the app's terminal to stop it.

### Seed credentials

| Tenant | Email | Password |
|---|---|---|
| Acme | `viewer@acme.com` | `Password123!` |
| Globex | `viewer@globex.com` | `Password123!` |

### End-to-end smoke test (Swagger, no frontend)

1. **`GET /api/v1/health`** → `{ "status": "healthy", "db": "connected" }`
2. **`POST /api/v1/auth/login`** (Acme user) → JWT + sites
3. Click **Authorize**, paste the token (no `Bearer ` prefix)
4. **`GET /api/v1/auth/me`** → your profile + sites
5. **`GET /api/v1/sites`** → your authorized sites
6. Wrong password → clean **401** with an `INVALID_CREDENTIALS` envelope
7. (Batches slice) **`GET /api/v1/batches`** → your tenant's batches
8. (Later rounds) log in as the **Globex** user and confirm you **cannot** see Acme's data — tenant isolation is the headline result.

> 💡 **Tip:** after resetting the database, **always log in again** to get a fresh token. A token issued before a DB reset points to tenant/user GUIDs that no longer exist, so authenticated calls return empty results.

---

## API Reference

All endpoints are served under **`/api/v1`**. Every endpoint **except** `POST /auth/login` and `GET /health` requires `Authorization: Bearer <jwt>`.

### Response envelope

Single resource: `{ data, error }`. List: `{ data, meta, error }`. On success `error` is `null`; on failure `data` is `null`.

```json
{
  "data": [ /* items */ ],
  "meta": { "total_count": 1250, "page": 1, "page_size": 50, "total_pages": 25 },
  "error": null
}
```

### Implemented

| Method | Route | Description |
|---|---|---|
| GET | `/api/v1/health` | DB connectivity health check (unauthenticated) |
| POST | `/api/v1/auth/login` | Verify password, issue JWT, return user + sites |
| GET | `/api/v1/auth/me` | Current user profile + sites (session rehydration) |
| GET | `/api/v1/sites` | Sites the authenticated user can access |
| GET | `/api/v1/batches` | Paginated/filterable/sortable batch list (in review) |

### Planned (DT-2)

| Method | Route | Description |
|---|---|---|
| GET | `/api/v1/dashboard/summary` | Status counters |
| GET | `/api/v1/dashboard/throughput?range=7d` | Throughput chart series |
| GET | `/api/v1/dashboard/status-distribution` | Status breakdown |
| GET | `/api/v1/dashboard/recent-failures` | Paginated recent failures |
| GET | `/api/v1/batches/{id}` | Batch detail summary |
| GET | `/api/v1/batches/{id}/files` | Files in a batch (paginated) |
| GET | `/api/v1/files/{id}/details` | File step history + error + remediation |
| GET | `/api/v1/files/{id}/line-items` | Invoice line items + categories + totals |
| GET | `/api/v1/errors/top-frequencies` | Top 10 errors + remediation |
| GET | `/api/v1/errors/trend?range=30d` | Error trend series |
| GET | `/api/v1/errors` | Filtered/paginated error list |
| GET | `/api/v1/errors/export` | CSV export of filtered errors |
| GET | `/api/v1/activity-log` | Paginated, filtered audit trail |

### HTTP status codes

| Code | When |
|---|---|
| 200 | Success |
| 400 | Validation failure (bad date, pageSize > 100, bad enum) |
| 401 | Missing / invalid / expired JWT |
| 403 | Authenticated but resource belongs to another tenant/site |
| 404 | Resource ID does not exist |
| 500 | Unhandled exception (logged, generic message returned) |

---

## Project Structure

```
DocAnalytics.slnx
│
├─ DocAnalytics.Domain        # entities + contracts (no dependencies)
├─ DocAnalytics.Data          # AppDbContext, Migrations/, DbSeeder, AddPersistence()
├─ DocAnalytics.Service       # business logic; per-feature folders + AddXxxFeature()
│   └─ Auth/                  # AuthDtos, IAuthService/AuthService, JwtTokenService, AddAuthFeature
└─ DocAnalytics.Api           # controllers, middleware, JWT, Program.cs
    ├─ Auth/                  # JwtSettings
    ├─ Common/                # ApiResponse<T>, CurrentUser, BaseController
    ├─ Controllers/           # AuthController, SitesController, BatchesController, ...
    ├─ Extensions/            # ApiServiceExtensions (AddJwtAuth, AddSwaggerWithJwt, ...)
    └─ Middleware/            # TenantSiteMiddleware
```

---

## Conventions & Contracts

- **Naming:** C# PascalCase → DB snake_case (EFCore.NamingConventions).
- **Response envelope:** `ApiResponse<T> { Data, Meta, Error }` with `.Ok(...)`, `.OkList(...)`, `.Fail(...)` factories.
- **Tenant context:** `ICurrentUser { UserId, TenantId, SiteId, Role }` — interface in Domain, concrete `CurrentUser` in Api, populated by `TenantSiteMiddleware`.
- **JWT claims:** `userId`, `tenantId`, `role`. The middleware reads **these exact claim names** (case-sensitive).
- **Site selection:** global tenant scoping is always enforced; **site is optional** — supplied via `?site_id=` or the `X-Site-Id` header. When no site is supplied, services should scope by tenant only.
- **Base route:** everything under `/api/v1`.
- **Pagination:** offset-based (`page` + `pageSize`, max 100).
- **Filtering/sorting:** query-string params; `sortBy` validated against an allow-list (no string concatenation).
- **DI:** each feature exposes `AddXxxFeature()` called from `Program.cs` (no shared-file edits).

---

## Git Workflow

`main` is the merged, runnable baseline. Work happens on feature branches, reviewed via PR, then merged.

```Windows Powershell
git checkout main
git pull
git checkout -b feature/<thing>
# ...build, commit...
git push -u origin feature/<thing>
# open PR → teammate reviews → merge → both pull main
```

- **Commit style** (Conventional Commits): `feat:`, `fix:`, `chore:`, `refactor:`, `docs:`, `test:`
- **Branch names:** `feature/<thing>`, `fix/<thing>`
- Pull `main` before starting a new branch.

---

## Roadmap & Status

Backend is built as **vertical feature slices** across **5 rounds** (two developers in parallel).

- [x] **Phase 0 — Foundation:** solution, 12-table schema + migration, seed data, JWT validation config, tenant/site middleware, response envelope, Swagger, `/health`.
- [x] **Round 1 — Auth slice:** `POST /auth/login`, `GET /auth/me`, `GET /sites` (JWT issuance, BCrypt verify, `UserSiteAccess` join). *(reference implementation)*
- [ ] **Round 1 — Batches list:** `GET /batches` (pagination, filtering, sorting, search) *(in review)*
- [ ] **Round 2 —** Dashboard summary / recent failures; Batch detail + files
- [ ] **Round 3 —** File details (joins + remediation); Invoice line items
- [ ] **Round 4 —** Errors list + CSV export; Charts (throughput, distribution, trend)
- [ ] **Round 5 —** Activity log; Input validation + global exception handling
- [ ] **Phase 2 —** Frontend (Angular, DT-3)
- [ ] **Phase 3 —** Stretch goals: PdfPig extraction, real charts, RBAC `[Authorize(Roles)]`, WebSockets

---

## Team

- **Dev A** — Akash Goswami
- **Dev B** — Shubh Gupta

### Design Docs

The full design rationale lives in the design-task document: `Design_Tasks_1-3_updated.pdf`.
