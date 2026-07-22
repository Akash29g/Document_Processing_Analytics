# ADR-0003: Tenant Isolation via EF Core Global Query Filters

| | |
|---|---|
| **Status** | Accepted |
| **Date** | Established at data-model time (DT-1); **proven** in Round 3 (Security) |
| **Deciders** | Akash (Dev A), cross-reviewed by Shubh (Dev B) |
| **Round** | R3 — cross-tenant 404 test + `[Authorize]` audit + non-scoped-entity guard |
| **Related** | ADR-0004 (the JWT that drives `CurrentUser`) |

---

## Context

DocAnalytics is strictly multi-tenant. The business rule is absolute: **a customer must never see another customer’s data**, isolated to the **tenant + site** level (a user only sees sites they are granted). This is both a functional requirement (FR-5.2/5.3) and a security requirement (NFR-3).

The naive approach — requiring every query to include `WHERE tenant_id = @t AND site_id = @s` — is dangerously fragile: a single forgotten clause anywhere in a growing codebase is a cross-tenant data leak. With ~1M file rows and many controllers/services, we needed isolation enforced **at the data layer** so it cannot be forgotten, plus **automated tests** that prove it holds and fail loudly if it regresses.

A second subtlety: how to respond when a user requests a resource ID that exists but belongs to another tenant. Returning `403 Forbidden` confirms the resource *exists* — an information leak. Returning `404 Not Found` reveals nothing.

## Alternatives considered

- **Manual `WHERE` clauses in every query** — rejected: not forgettable-proof; one miss = a leak.
- **Row-Level Security (RLS) in PostgreSQL** — considered; powerful but adds DB-role/session-variable plumbing and complicates the EF/migration story and local testing. The EF global filter gives equivalent “can’t forget it” safety at the application layer with far less operational complexity for this project.
- **Separate schema/database per tenant** — rejected: heavy operational overhead for the scale/goals here; the shared-schema + `tenant_id`/`site_id` + global filter model is the documented DT-1 design.
- **403 for cross-tenant IDs** — rejected: leaks existence. We deliberately return **404**.

## Decision

1. **Marker interface + global query filter.** Tenant-scoped entities implement `ITenantScoped` (`TenantId`, `SiteId`). `AppDbContext` applies an EF Core **global query filter** to each, auto-appending `WHERE tenant_id = X AND site_id = Y` on every query, driven by an injected `ICurrentUser`:

   ```csharp
   modelBuilder.Entity<FileRecord>()
       .HasQueryFilter(f => f.TenantId == _currentUser.TenantId
                         && f.SiteId  == _currentUser.SiteId);
   ```

   - **Scoped entities:** `Transaction`, `FileRecord`, `ActivityLog`, `InvoiceLineItem`, `InvoiceHeader`, `AlertRule`.
2. **Current-user context from the request.** `TenantSiteMiddleware` runs before controllers: validates the JWT, extracts `userId` / `tenantId` / `role` (custom claims, `MapInboundClaims = false`, `RoleClaimType = "role"`), reads the requested site from the `X-Site-Id` header (or query param), enforces that the user is granted that site (403 `SITE_FORBIDDEN` otherwise), and populates `CurrentUser`. The **`Developer`** role is **hard-blocked** from all data routes — permitted only on `/auth`, `/provisioning`, `/health`.
3. **404, not 403, for cross-tenant resource IDs.** Because the global filter makes another tenant’s row invisible, the service returns `null` → the controller returns **404**. This prevents an existence-leak.
   - **Note:** the DT-2 design doc originally specified 403 for this case; the code deliberately returns **404** and *this ADR is the source of truth*. (403 is reserved for the site-access middleware case, `SITE_FORBIDDEN`.)
4. **Non-scoped entities are driven from a scoped parent.** `FileStepHistory` has **no** global filter (it is reached only via an already-filtered parent `FileRecord`, never by a bare id). This is a deliberate exception, documented and test-guarded. (`InvoiceHeader` *does* implement `ITenantScoped` in the current repo.)
5. **Least-privilege authorization on top of isolation.** Every data controller is `[Authorize(Policy = "DataAccess")]` (Admin|Viewer). Documented exceptions: `AuthController`/`HealthController` (anonymous), `ProvisioningController` (`DeveloperOnly`), `AdminController` (`AdminOnly`).
6. **Automated proof (Round 3).**
   - `DocAnalytics.Service.Tests/Security/CrossTenantAccessTests.cs` — seeds tenant B’s `Transaction` + `File` + `FileStepHistory` + `InvoiceHeader`/line-items; acts as tenant A; asserts `FileDetailsService.GetFileDetailsAsync` and `InvoiceService.GetInvoiceForFileAsync` return **null** → 404 (“no existence leak”).
   - `DocAnalytics.Api.Tests/Security/AuthorizeAuditTests.cs` — reflection scan asserting every data controller carries `[Authorize(Policy = "DataAccess")]`, with an explicit allow-list for the anonymous / Developer / Admin controllers.
   - `DocAnalytics.Data.Tests/Persistence/NonScopedEntityIsolationTests.cs` — proves `FileStepHistory` is unreachable cross-tenant when driven from a filtered parent.
   - `DocAnalytics.Data.Tests/Persistence/TenantIsolationTests.cs` — proves queries return only current-tenant rows and exclude same-tenant/different-site rows.

## Implementation notes

- **Files:** `DocAnalytics.Data/AppDbContext.cs` (filter registration), `DocAnalytics.Domain/Common/ITenantScoped.cs` + `ICurrentUser.cs`, `DocAnalytics.Api/Middleware/TenantSiteMiddleware.cs`, `DocAnalytics.Api/Common/CurrentUser.cs`.
- **Service contracts:** service constructors take **only** `AppDbContext` (e.g. `FileDetailsService(AppDbContext db)`, `InvoiceService(AppDbContext db)`) — no per-service tenant filtering; they rely 100% on the global filter.
- **Background workers** run without an HTTP context/JWT, so they must `IgnoreQueryFilters()` and then explicitly `CurrentUser.Set(...)` to operate on the correct tenant/site (e.g. `ExtractionWorker`). Forgetting this either over-filters (nothing found) or, worse, must be handled carefully to avoid cross-tenant writes.
- **Test harness gotchas (learned in R3):**
  - In-memory test contexts **must** pass the `ICurrentUser` into the `AppDbContext` ctor (`InMemoryDb.Create(user)` / `TestDb.Create(user)`), or the global filter is inert and the test proves nothing.
  - `Program` is implicitly internal (top-level statements) → tests reference the API assembly via a controller type (`typeof(HealthController).Assembly`) instead of `DocAnalytics.Api.Program`.
  - In-memory `FileRecord` needs non-null `FileName`/`FileType`/`Status`/`CurrentStep` on insert (NOT NULL in schema).

## Consequences

**Positive**
- Isolation is enforced **centrally** and **cannot be forgotten** by a developer adding a new query.
- **No existence-leak** across tenants (404 semantics).
- Isolation is **regression-guarded by tests**, not just by convention or code review.
- Denormalized `tenant_id`/`site_id` means the filter needs no JOIN even on the ~1M-row `files` table (NFR-1).

**Negative / trade-offs**
- **Non-scoped entities require discipline.** `FileStepHistory` must always be reached via a scoped parent; this is an exception a newcomer could get wrong. Mitigated by `NonScopedEntityIsolationTests` as living documentation.
- **Background workers must set `CurrentUser` manually** after `IgnoreQueryFilters()` — a sharp edge that must be respected in every new worker.
- Global filters apply to **every** query, including legitimate cross-cutting reads; those must opt out explicitly with `IgnoreQueryFilters()` and re-scope themselves.
- A subtle false-pass risk in tests: a test can “pass” because the filter is dead. **Gut-check:** temporarily flip a seeded `TenantId` to match the current user — the test should then **fail** (row becomes visible). Flip it back. This proves the filter is doing the work.

## Testing & verification

- Run: `dotnet test DocAnalytics.Service.Tests`, `dotnet test DocAnalytics.Data.Tests`, `dotnet test DocAnalytics.Api.Tests`.
- The cross-tenant test asserts **null → 404**; the authorize audit fails the build if any new data controller forgets `[Authorize(Policy="DataAccess")]`.

## Related PRs / commits

- `R3(Security): prove tenant isolation — cross-tenant 404 test + [Authorize] audit + non-scoped-entity guard`
- `fix(security): enforce site-level authorization (FR-5.3) + refreshed seed users`
- DT-1 data-model doc (the original global-filter design) and DT-2 (the 404-vs-403 rule, corrected to 404 here).

## Follow-ups

- If a future feature legitimately needs cross-tenant reporting (platform admin), route it through the `Developer`-scoped surface with `IgnoreQueryFilters()` + explicit, audited access — never by loosening the filter.
- Consider a Roslyn analyzer/CI check that flags any `IgnoreQueryFilters()` usage for mandatory review.
