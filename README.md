# Document Processing Analytics Platform

A multi-tenant **document-processing analytics & monitoring** backend built with ASP.NET Core and PostgreSQL. It ingests batches of documents (PDF invoices, CSV manifests), tracks each file through a processing pipeline (Upload → Validate → Transform → Load), records errors and remediation guidance, extracts invoice line items, and exposes dashboards/analytics over a clean REST API.

> **Status:** Backend in active development. Phase 0 (Foundation) and the Round 1 Auth slice are complete. See [Roadmap](#roadmap--status).

---

## Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Database Schema](#database-schema)
- [Tenant Isolation](#tenant-isolation)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Running & Testing](#running--testing)
- [API Reference](#api-reference)
- [Project Structure](#project-structure)
- [Conventions & Contracts](#conventions--contracts)
- [Git Workflow](#git-workflow)
- [Roadmap & Status](#roadmap--status)
- [Team](#team)

---

## Overview

The platform serves **multiple customer companies (tenants)**, each with one or more **sites** (physical locations). Every tenant + site combination is completely isolated — one customer can never see another customer's data.

Core capabilities (target):

- **Dashboard** — status counters, throughput, status distribution, recent failures.
- **Batch Explorer** — paginated/filterable batches, drill-down into files and step history.
- **Error Analysis** — top errors, trends, filtered list, CSV export, remediation messages.
- **Activity Log** — append-only, paginated audit trail.
- **Auth & Tenant/Site selection** — JWT login, session rehydration, site listing.

---

## Tech Stack

| Layer | Choice |
|-------|--------|
| Backend | ASP.NET Core Web API (controller-based), **.NET 10** |
| ORM / Data access | Entity Framework Core 10 (migration-based, parameterized LINQ) |
| Database | PostgreSQL 18 |
| Auth | JWT Bearer tokens (ASP.NET Core authentication middleware) |
| Password hashing | BCrypt.Net-Next |
| API docs | Swagger / OpenAPI (Swashbuckle) |
| Naming | C# PascalCase → DB snake_case via EFCore.NamingConventions |

> Note: the original design docs referenced “.NET 8”, but the project targets **.NET 10** (`net10.0`). Treat .NET 10 as the source of truth.

### Key package versions

- EF Core 10.0.9 (`Microsoft.EntityFrameworkCore` + `.Relational` pinned to 10.0.9 in the Data project)
- Npgsql.EntityFrameworkCore.PostgreSQL 10.0.2
- EFCore.NamingConventions 10.0.1 (snake_case)
- Microsoft.AspNetCore.Authentication.JwtBearer 10.0.9
- Swashbuckle.AspNetCore 10.2.1 (Microsoft.OpenApi 2.x)
- BCrypt.Net-Next 4.2.0
- System.IdentityModel.Tokens.Jwt (Service project, for JWT issuance)

---

## Architecture

Layered solution with strict one-way dependencies:

```
Api  →  Service  →  Data  →  Domain   (Domain depends on nothing)
```

| Project | Responsibility |
|---------|----------------|
| **DocAnalytics.Domain** | Entities + contracts (`ITenantScoped`, `ICurrentUser`). No dependencies. |
| **DocAnalytics.Data** | `AppDbContext`, migrations, `DbSeeder`, `AddPersistence()`. References Domain. |
| **DocAnalytics.Service** | Business logic (`IXxxService` + `XxxService`), per-feature `AddXxxFeature()` DI. References Data + Domain. |
| **DocAnalytics.Api** | Controllers, middleware, JWT, `Program.cs` (composition root). References Service. |

**Golden rule:** controllers never touch `AppDbContext` directly — they call a service. The service runs the EF query; the global query filter auto-applies tenant/site scoping.

Each feature ships as its own files — `IXxxService` + `XxxService` + `XxxController` + `XxxDtos` + an `AddXxxFeature()` DI extension — so contributors don’t edit shared files and merges stay clean.

---

## Database Schema

12 tables across 4 layers (UUID primary keys throughout):

- **Identity:** `tenants`, `sites`, `users`, `user_site_access`
- **Pipeline:** `transactions` (the “batch”), `files`, `file_step_history`
- **Support:** `error_catalog` (global), `activity_log`
- **Extraction:** `document_types` (global), `invoice_line_items`, `item_categories` (global)

Notable entity/table mappings:

- `FileRecord` → `files` (avoids clashing with `System.IO.File`)
- `Transaction` → `transactions` (exposed in the API as “batch” / `/batches`)
- `InvoiceLineItem` → `invoice_line_items`, `ItemCategory` → `item_categories`

Schema is managed via **EF Core migrations** (no manual DDL). The initial migration creates all 12 tables, indexes, and foreign keys.

---

## Tenant Isolation

Three layers of protection ensure Customer A can never see Customer B:

1. Every pipeline/support/extraction table carries `tenant_id` + `site_id`.
2. The JWT carries `userId`, `tenantId`, `role`; `site_id` comes from the request (`?site_id=` or `X-Site-Id` header).
3. An EF Core **global query filter** auto-injects `WHERE tenant_id = X AND site_id = Y` on every query against `ITenantScoped` entities (`Transaction`, `FileRecord`, `ActivityLog`, `InvoiceLineItem`).

Identity catalogs (`users`, `sites`, `user_site_access`) and global catalogs (`document_types`, `item_categories`, `error_catalog`) are intentionally not tenant-filtered.

Even if a developer forgets a `WHERE` clause, the filter protects the data.

---

## Prerequisites

Install locally (not committed to the repo):

- **.NET 10 SDK**
- **PostgreSQL 18** (+ pgAdmin)
- **Git**
- **Visual Studio 2026** (or VS Code / Rider)
- **dotnet-ef** global tool:
  ```bash
  dotnet tool install --global dotnet-ef
  ```
  (open a fresh terminal afterwards so it’s on your PATH)

---

## Getting Started

### 1. Clone & open

```bash
git clone https://github.com/Akash29g/Document_Processing_Analytics.git
cd Document_Processing_Analytics
```

Open `DocAnalytics.slnx` via **File → Open → Project/Solution** (do NOT create a new project).

### 2. Set local secrets

Secrets are git-ignored; `appsettings.json` ships with a blanked password and a placeholder JWT key. Set real values via user-secrets (run inside `DocAnalytics.Api` or pass `--project DocAnalytics.Api`):

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=docanalytics;Username=postgres;Password=YOUR_LOCAL_PW"
dotnet user-secrets set "Jwt:Key" "any-32+-character-secret-key-for-local-dev"
```

> `Jwt:Key` must be at least 32 characters, or startup throws `IDX10720`.

### 3. Create the database

```bash
dotnet ef database update --project DocAnalytics.Data --startup-project DocAnalytics.Api
```

Seed data loads automatically on first run (2 tenants: Acme + Globex, with sites, users, transactions, files, steps, invoice line items, categories, and an activity log entry).

---

## Running & Testing

### Run

```bash
dotnet run --project DocAnalytics.Api
```

Note the port from the console (e.g. `Now listening on: http://localhost:5256`) and open Swagger:

```
http://localhost:<port>/swagger
```

### Seed credentials

| Tenant | Email | Password |
|--------|-------|----------|
| Acme   | `viewer@acme.com`   | `Password123!` |
| Globex | `viewer@globex.com` | `Password123!` |

### End-to-end smoke test (Swagger, no frontend)

1. `GET /api/v1/health` → `{ "status": "healthy", "db": "connected" }`
2. `POST /api/v1/auth/login` (Acme user) → JWT + sites
3. Click **Authorize**, paste the token
4. `GET /api/v1/auth/me` → your profile + sites
5. `GET /api/v1/sites` → your authorized sites
6. Wrong password → clean `401` with an `INVALID_CREDENTIALS` envelope
7. (Later rounds) log in as the Globex user and confirm you cannot see Acme’s data — tenant isolation is the headline result.

---

## API Reference

All endpoints are served under `/api/v1`. Every endpoint except `POST /auth/login` and `GET /health` requires `Authorization: Bearer <jwt>`.

### Response envelope

Single resource: `{ data, error }`. List: `{ data, meta, error }`. On success `error` is null; on failure `data` is null.

```json
{
  "data": [ /* items */ ],
  "meta": { "total_count": 1250, "page": 1, "page_size": 50, "total_pages": 25 },
  "error": null
}
```

### Implemented

| Method | Route | Description |
|--------|-------|-------------|
| GET  | `/api/v1/health` | DB connectivity health check (unauthenticated) |
| POST | `/api/v1/auth/login` | Verify password, issue JWT, return user + sites |
| GET  | `/api/v1/auth/me` | Current user profile + sites (session rehydration) |
| GET  | `/api/v1/sites` | Sites the authenticated user can access |

### Planned (DT-2)

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/v1/dashboard/summary` | Status counters |
| GET | `/api/v1/dashboard/throughput?range=7d` | Throughput chart series |
| GET | `/api/v1/dashboard/status-distribution` | Status breakdown |
| GET | `/api/v1/dashboard/recent-failures` | Paginated recent failures |
| GET | `/api/v1/batches` | Paginated/filterable/sortable batch list |
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
|------|------|
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
├─ DocAnalytics.Data          # AppDbContext, migrations, DbSeeder, AddPersistence()
├─ DocAnalytics.Service       # business logic; per-feature folders + AddXxxFeature()
│   └─ Auth/                  # AuthDtos, IAuthService/AuthService, JwtTokenService, AddAuthFeature
└─ DocAnalytics.Api           # controllers, middleware, JWT, Program.cs
    ├─ Auth/                  # JwtSettings
    ├─ Common/                # ApiResponse<T>, CurrentUser, BaseController
    ├─ Controllers/           # AuthController, SitesController, ...
    ├─ Extensions/            # ApiServiceExtensions (AddJwtAuth, AddSwaggerWithJwt, ...)
    └─ Middleware/            # TenantSiteMiddleware
```

---

## Conventions & Contracts

- **Naming:** C# PascalCase → DB snake_case (EFCore.NamingConventions).
- **Response envelope:** `ApiResponse<T> { Data, Meta, Error }` with `.Ok(...)`, `.OkList(...)`, `.Fail(...)` factories.
- **Tenant context:** `ICurrentUser { UserId, TenantId, SiteId, Role }` — interface in Domain, concrete `CurrentUser` in Api, populated by `TenantSiteMiddleware`.
- **JWT claims:** `userId`, `tenantId`, `role`. The middleware reads these exact claim names.
- **Base route:** everything under `/api/v1`.
- **Pagination:** offset-based (`page` + `pageSize`, max 100).
- **Filtering/sorting:** query-string params; `sortBy` validated against an allow-list (no string concatenation).
- **DI:** each feature exposes `AddXxxFeature()` called from `Program.cs` (no shared-file edits).

---

## Git Workflow

`main` is the merged, runnable baseline. Work happens on feature branches, reviewed via PR, then merged.

```bash
git checkout main
git pull
git checkout -b feature/<thing>
# ...build, commit...
git push -u origin feature/<thing>
# open PR → teammate reviews → merge → both pull main
```

- **Commit style (Conventional Commits):** `feat:`, `fix:`, `chore:`, `refactor:`, `docs:`, `test:`
- **Branch names:** `feature/<thing>`, `fix/<thing>`
- Pull `main` before starting a new branch.

---

## Roadmap & Status

Backend is built as vertical feature slices across 5 rounds (two developers in parallel).

- [x] **Phase 0 — Foundation:** solution, 12-table schema + migration, seed data, JWT validation config, tenant/site middleware, response envelope, Swagger, `/health`.
- [x] **Round 1 — Auth slice:** `POST /auth/login`, `GET /auth/me`, `GET /sites` (JWT issuance, BCrypt verify, UserSiteAccess join). *(reference implementation)*
- [ ] **Round 1 — Batches list:** `GET /batches` (pagination, filtering, sorting, search)
- [ ] **Round 2 — Dashboard summary / recent failures; Batch detail + files**
- [ ] **Round 3 — File details (joins + remediation); Invoice line items**
- [ ] **Round 4 — Errors list + CSV export; Charts (throughput, distribution, trend)**
- [ ] **Round 5 — Activity log; Input validation + global exception handling**
- [ ] **Phase 2 — Frontend (Angular, DT-3)**
- [ ] **Phase 3 — Stretch goals:** PdfPig extraction, real charts, RBAC `[Authorize(Roles)]`, WebSockets

---

## Team

- **Dev A** — @akash29g
- **Dev B** — @g9shubh

---

## Design Docs

The full design rationale lives in the design-task document: Design_Tasks_1-3_updated.pdf
