# ADR-0004: JWT Auth + DB-Backed Rotating Refresh Tokens

| | |
|---|---|
| **Status** | Accepted |
| **Date** | Round 4 (Security); access-token TTL + HttpOnly cookie finalized in `harden/prod-security` (item #7) |
| **Deciders** | Shubh (Dev B, refresh tokens) & Akash (Dev A, token hardening), cross-reviewed |
| **Round** | R4 — DB-backed rotating refresh tokens (short access token + silent refresh) |
| **Related** | ADR-0001 (CORS `AllowCredentials`), ADR-0003 (JWT drives `CurrentUser`) |

---

## Context

The initial authentication design used a **long-lived (120-minute) JWT access token** with **no refresh mechanism**, stored client-side in `localStorage` (`da_token`). Two problems:

1. **Large blast radius on leak.** A 120-minute bearer token in `localStorage` is readable by any injected script (XSS) and is valid for a long time. There was no way to **revoke** a session before natural expiry.
2. **Poor UX vs security trade-off.** Shortening the token lifetime without a refresh mechanism would force users to log in constantly.

We needed **short-lived access tokens** plus a **secure, revocable refresh mechanism** that supports silent background refresh — and to harden the surrounding login/password flows.

The DataProtection key ring is also relevant: cookie/data-protection operations must survive container restarts and run across multiple ECS tasks, so keys cannot be ephemeral in-container.

## Alternatives considered

- **Keep the long-lived JWT in `localStorage`** — rejected: XSS-readable, non-revocable, large blast radius.
- **Stateless JWT-only with short expiry, no refresh** — rejected: bad UX (frequent re-login) and still non-revocable.
- **Refresh token in the JSON body / `localStorage`** — rejected: still readable by JavaScript (XSS), defeating the point.
- **Opaque server-side sessions (no JWT)** — viable but discards the stateless-access-token benefits and the existing claim-based authorization plumbing; the hybrid (short JWT + DB refresh) keeps stateless authz on the hot path while adding revocation only on refresh.
- **Non-rotating refresh tokens** — rejected: rotation enables reuse-detection (a stolen-then-reused token is caught).

## Decision

1. **Short-lived JWT access tokens.** Access-token TTL cut to **20 minutes** in production (was 120). Custom claims `{ userId, tenantId, role }`; `MapInboundClaims = false`; `RoleClaimType = "role"`. Validated on startup (`Jwt:Key` must be ≥ 32 chars; `Jwt:Issuer` / `Jwt:Audience` configured). The access token is still sent as `Authorization: Bearer` on every API/hub call and drives `CurrentUser` (ADR-0003).
2. **DB-backed rotating refresh tokens.** Opaque refresh tokens are stored **hashed** in the database (`RefreshToken` entity; migration `AddRefreshTokens`) and **rotated on every use** (old token invalidated, new one issued). This makes sessions **revocable** and **reuse-detectable** — capabilities a pure stateless JWT cannot provide. Implemented in `RefreshTokenService` / `IRefreshTokenService` + `JwtTokenService` / `IJwtTokenService`.
3. **Refresh token in an HttpOnly cookie.** The refresh token is delivered as an **HttpOnly / Secure / SameSite=Strict** cookie scoped to `Path=/api/v1/auth`, moved **out of the JSON body** so client JavaScript can never read it (XSS-resistant):

   ```csharp
   Response.Cookies.Append("refresh_token", rawRefreshToken, new CookieOptions {
       HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict,
       Expires = refreshExpiresAt, Path = "/api/v1/auth" });
   ```

   The Angular client uses `withCredentials`; the backend CORS policy sets `AllowCredentials()` (see ADR-0001). The refresh endpoint reads the token from `Request.Cookies`.
4. **Silent refresh.** Short access token + cookie-based refresh lets the SPA refresh transparently in the background; the access token continues to be attached as `Authorization: Bearer` by the `authSiteInterceptor`.
5. **Login protection & password hashing.** Login **rate-limiting + account lockout** (`LoginLockoutService`; migration `AddLoginAttempts`; 5 failures → 15-min lock, window re-arms after expiry, case/whitespace-insensitive email). Passwords hashed/verified with **BCrypt** (`BCrypt.Net.BCrypt.HashPassword` / `.Verify`).
6. **Supporting controls (`harden/prod-security`).**
   - **DataProtection keys persisted to Postgres** (R0; `AppDbContext : IDataProtectionKeyContext`, migration `AddDataProtectionKeys`) so cookie/data-protection is stable across restarts and ECS tasks.
   - **Super-admin bootstrap** (`SuperAdminSeeder.EnsureSuperAdminAsync`) creates the platform admin (`Role="Developer"`, `TenantId=null`) from **secrets/env vars** (`SuperAdmin__Email` / `SuperAdmin__Password`) — never hard-coded — with `MustChangePassword = true`. Fails loud in prod if not configured, so a fresh DB never deploys with no way to log in.
   - **Password policy** on change-password: min-length + complexity + **HaveIBeenPwned** k-anonymity breach check (`IPasswordPolicy` / `PasswordPolicy`, typed `HttpClient`, ~3s **fail-open** timeout; returns a human-readable rejection reason).

## Implementation notes

- **Files:** `DocAnalytics.Service/Auth/*` (`AuthService`, `JwtTokenService`, `RefreshTokenService`, `LoginLockoutService`, `PasswordPolicy`, `AuthDtos`, `AuthFeatureExtensions`), `DocAnalytics.Api/Controllers/AuthController.cs`, `DocAnalytics.Domain/Entities/{RefreshToken,LoginAttempt,User}.cs`, `DocAnalytics.Data/Seeding/SuperAdminSeeder.cs`, migrations `AddRefreshTokens` / `AddLoginAttempts` / `AddDataProtectionKeys`.
- **Frontend:** `auth.service.ts` (signals `currentUser`/`sites`/`token`), `auth-site.interceptor.ts`, `error.interceptor.ts`, `refresh-timer.service.ts`; `withCredentials` on refresh; SignalR `accessTokenFactory`.
- **Config:** `Jwt:Key` / `Jwt:ExpiryMinutes` (20 in prod) / `Jwt:Issuer` / `Jwt:Audience`; `SuperAdmin__Email` / `SuperAdmin__Password` via user-secrets (dev) or Secrets Manager (prod).
- **CI edge case:** Dependabot-triggered workflows don’t receive normal Actions secrets, so the E2E job supplies a low-entropy **test-only** `Jwt__Key` fallback (obvious, non-secret so gitleaks won’t flag it) purely so the API can boot during Playwright runs.

## Consequences

**Positive**
- A leaked access token is useful for at most **~20 minutes**.
- Refresh tokens are **revocable** and **reuse-detectable** (DB-backed + rotating) and **unreadable by JavaScript** (HttpOnly cookie).
- Login brute-force is throttled; known-breached passwords are rejected at change time.
- Roles + policies (ADR-0003) sit cleanly on top of the short-lived access token.

**Negative / trade-offs**
- The refresh path is now **stateful** (a DB read + rotation write on every refresh) — more moving parts than a pure stateless JWT, but that statefulness is exactly what buys revocation and reuse-detection.
- The cookie approach is a **full-stack coupling**: the SPA must send `withCredentials` and the backend CORS must `AllowCredentials()` — both must stay in sync, and `AllowCredentials()` forbids a wildcard CORS origin (hence the locked origin list in ADR-0001).
- **HaveIBeenPwned** is an external dependency; we deliberately **fail-open** on outage (availability over strictness) with a short timeout — a breached password could slip through during an HIBP outage. Accepted.
- Shorter tokens mean the SPA will occasionally hit a transient `401` on a long-idle session until the refresh completes (see Follow-ups).

## Testing & verification

- **Unit:** `JwtTokenServiceTests` (claims, audience, missing-key throws), `LoginLockoutServiceTests` (locks after 5, re-arms after window, case-insensitive), `AuthServiceTests`.
- **Manual smoke:** login → confirm `refresh_token` cookie is present with **HttpOnly/Secure/Strict** → wait for access-token expiry → confirm silent refresh via cookie → logout clears the cookie.
- **E2E:** `e2e/login.spec.ts`, `session.spec.ts`, `auth.setup.ts`.

## Related PRs / commits

- `R4 Security: DB-backed rotating refresh tokens (15-min access + silent refresh)`
- `security: production hardening (… password policy, HttpOnly refresh cookie …)` (PR #113 — items #6, #7)
- `R2 Security: login rate-limiting + account lockout + secrets fail-fast`
- `security: bootstrap platform super-admin from secrets with forced password change`

## Follow-ups

- Wire the frontend `error.interceptor` to catch a `401` → call the refresh endpoint → retry the original request, so long sessions don’t surface transient 401s (dashboard auto-refresh symptom).
- **Infra (out of code scope):** move `Jwt__Key` / `ConnectionStrings__*` / `SuperAdmin__*` into Secrets Manager; **rotate** any key ever committed to git history; create a DML-only Postgres role for the app.
