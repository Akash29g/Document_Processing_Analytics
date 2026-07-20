# API Rate Limiting

> Round 5 (Akash / Dev A). Branch: `feat/api-rate-limiting`.
> Protects the API from request floods and brute-force abuse using ASP.NET Core's
> built-in rate limiter (`Microsoft.AspNetCore.RateLimiting` + `System.Threading.RateLimiting`)
> — part of the .NET 10 shared framework, **no extra NuGet package** in the API project.

---

## Why (the problem)

Without limits, any client can hammer the API: brute-force the login endpoint,
spam the expensive CSV export, or DoS the server with a runaway polling loop.
Rate limiting caps requests per time window and replies **429 Too Many Requests**
(with a `Retry-After` header) once the cap is exceeded.

## Rate limiting vs. login lockout (they are different layers)

| | Rate limiting (this feature) | Login lockout (`LoginLockoutService`, earlier round) |
|---|---|---|
| Scope | Whole API / any endpoint | A single user **account** |
| Counts | All requests (success or fail) | Failed password attempts only |
| State | In-memory (framework) | Database (`LoginAttempt` rows) |
| Trigger | N requests / window → 429 | X failures → that account locked |

Both protect `/auth/login` and both return the same `RATE_LIMITED` + `Retry-After`
shape, so the frontend handles them identically. They are complementary, not duplicates.

---

## Policies (config-driven — NFR-5)

All limits live in `appsettings.json` → `"RateLimiting"`. No hardcoded numbers in code.

| Policy   | Partition key            | Default limit | Applied to |
|----------|--------------------------|---------------|------------|
| `login`  | client IP                | 5 / 60s       | `POST /api/v1/auth/login` |
| `reads`  | user id (JWT `userId`)   | 100 / 60s     | dashboard analytics, error analytics, errors list |
| `export` | user id (JWT `userId`)   | 3 / 60s       | `GET /api/v1/errors/export` |

- `login` is partitioned by **IP** because the user is not authenticated yet
  (brute-force protection). IP is correct behind nginx because `UseForwardedHeaders()`
  runs first.
- `reads` / `export` are partitioned per **user** (fallback to IP if the claim is
  missing), so one heavy user cannot exhaust the quota for everyone sharing an office IP.
- `export` is tight because CSV generation is heavy.

### Config block

```json
"RateLimiting": {
  "Login":  { "PermitLimit": 5,   "WindowSeconds": 60, "QueueLimit": 0 },
  "Reads":  { "PermitLimit": 100, "WindowSeconds": 60, "QueueLimit": 0 },
  "Export": { "PermitLimit": 3,   "WindowSeconds": 60, "QueueLimit": 0 }
}
```

`QueueLimit = 0` means over-limit requests are **rejected immediately** (rate limiting),
not queued/delayed (throttling). This is intentional for security use cases.

Tune per environment: bump the numbers up (or widen windows) in
`appsettings.Development.json` so local dev and E2E runs do not trip the limiter.

---

## 429 response contract (locked — do not change the shape)

Rejections use the standard API envelope plus a `Retry-After` header:

```json
{
  "data": null,
  "error": {
    "code": "RATE_LIMITED",
    "message": "Too many requests. Please try again later."
  }
}
```

Header: `Retry-After: <seconds>` (emitted when the limiter exposes retry metadata).

---

## Where it lives (code map)

| File | Responsibility |
|------|----------------|
| `DocAnalytics.Api/Configuration/RateLimitOptions.cs` | strongly-typed config (`RateLimiting` section) |
| `DocAnalytics.Api/Configuration/RateLimitingExtensions.cs` | registers the limiter, the 3 policies, and the custom `OnRejected` 429 |
| `DocAnalytics.Api/Program.cs` | `AddRateLimitingFeature(...)` + `app.UseRateLimiter()` |
| `DocAnalytics.Api/appsettings.json` | the config-driven limits |
| controllers | `[EnableRateLimiting("...")]` attributes |
| `docanalytics-web/.../core/interceptors/error.interceptor.ts` | maps 429 → friendly toast |
| `DocAnalytics.Api.Tests/RateLimiting/RateLimitTests.cs` | Nth-request → 429 integration test |

### Middleware order (important)

`app.UseRateLimiter()` runs **after** `app.UseAuthentication()` so the `userId` claim
is available for per-user partitioning (`reads` / `export`):

```
UseCors → ... → UseAuthentication → UseRateLimiter → UseAuthorization → TenantSiteMiddleware → MapControllers
```

### Applying a policy

```csharp
[EnableRateLimiting("login")]   // action-level on POST /auth/login
[EnableRateLimiting("reads")]   // class-level on analytics/errors controllers
[EnableRateLimiting("export")]  // action-level; overrides the class-level "reads"
```

An action-level attribute overrides the controller-level one for that action
(e.g. `ErrorsController` is `reads` at class level, but `ExportErrors` overrides to `export`).

---

## Frontend behavior

`error.interceptor.ts` catches `status === 429`, reads `Retry-After`, and shows a
friendly warning toast ("You're doing that too fast. Try again in Ns."). The 429 branch
sits above the login early-return so it also surfaces on the login screen.

---

## Testing

`DocAnalytics.Api.Tests/RateLimiting/RateLimitTests.cs` boots the app with a
`WebApplicationFactory` (using `HealthController` to locate the assembly — no need to
expose `Program`), swaps the DB to EF in-memory, strips background workers, and forces
`RateLimiting:Login:PermitLimit = 5` via in-memory config. It fires 7 login requests and
asserts the last is `429` with a `Retry-After` header.

```bash
dotnet test DocAnalytics.Api.Tests/DocAnalytics.Api.Tests.csproj
```
