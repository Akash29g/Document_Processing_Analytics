# ADR-0001: Transport Hardening (HSTS, Security Headers, CORS Lock-down)

| | |
|---|---|
| **Status** | Accepted |
| **Date** | Round 1 (Security); reinforced in the `harden/prod-security` pass (item #4) |
| **Deciders** | Akash (Dev A), cross-reviewed by Shubh (Dev B) |
| **Round** | R1 — transport hardening (HTTPS/HSTS, security headers, CORS lock-down) |
| **Related** | ADR-0002 (CSP is set by the same middleware) |

---

## Context

DocAnalytics is a live, internet-facing, multi-tenant application. TLS is terminated at the **edge** (nginx in the container / ALB in AWS), and the API container speaks **plain HTTP internally on `:8080`**. We needed to harden the HTTP transport layer without breaking that topology and without introducing redirect loops.

Concrete constraints and forces at play:

1. **Cross-origin dev.** In development the Angular SPA runs on `localhost:4200` and calls the API on a different origin, so CORS must be **correct but not permissive** (no `*` with credentials).
2. **TLS terminates at the edge.** If `UseHttpsRedirection()` runs *inside* the container, the app only ever sees plain HTTP on the wire and issues an infinite 307/308 redirect loop behind nginx/ALB. The app must instead trust the `X-Forwarded-Proto` header from the single proxy hop.
3. **JSON-only API.** The API never returns HTML, so browsers should be explicitly told not to MIME-sniff, not to frame it, and not to leak referrers.
4. **No hard-coded origins.** Earlier code had an inline CORS policy hard-coding `http://localhost:4200`. Production must be lockable to the real domain without a code change.
5. **Header ordering matters.** ASP.NET Core middleware is order-sensitive; forwarded-headers must run before any scheme-aware code, and exception handling must be outermost so failures still produce a clean envelope.

## Alternatives considered

- **`UseHttpsRedirection()` in the app** — rejected: causes redirect loops behind a TLS-terminating proxy. HTTP→HTTPS redirection belongs at the ALB/nginx edge instead.
- **Hard-coded CORS origins** — rejected: not environment-portable; a prod origin change would require a code change + redeploy.
- **A permissive `AllowAnyOrigin()`** — rejected outright: incompatible with credentialed requests (the refresh-cookie flow needs `AllowCredentials()`, which forbids `*`) and a security anti-pattern.
- **Setting headers per-controller / via attributes** — rejected: easy to forget on a new controller; a single middleware guarantees coverage on every response.

## Decision

1. **Config-driven CORS.** Allowed origins come from `Security:Cors:AllowedOrigins` (never hard-coded).
   - `appsettings.Development.json` lists `http://localhost:4200` + `https://localhost:4200`.
   - `appsettings.Production.json` locks the policy to the real domain(s) only.
   - Exactly **one** `app.UseCors(CorsOptions.PolicyName)` call; the old inline `"frontend"` policy and its duplicate `UseCors` call were removed.
   - The policy enables `AllowCredentials()` so the HttpOnly refresh cookie works (see ADR-0004).
2. **HSTS in production only.** `app.UseHsts()` runs when **not** Development and when `Security:Hsts:Enabled` is true: **1-year** max-age, `includeSubDomains`, `preload`. HSTS is intentionally off in dev (localhost, self-signed).
3. **Forwarded headers first.** `app.UseForwardedHeaders()` is the first middleware so scheme/host-aware code sees the real client protocol from nginx (`X-Forwarded-Proto`, `X-Forwarded-For`). `UseHttpsRedirection` stays **off** in-container by design (TLS terminates at the edge). Trust is scoped to the single known proxy hop (`KnownIPNetworks` / `KnownProxies` cleared appropriately).
4. **`SecurityHeadersMiddleware`** sets on **every** response:
   - `X-Content-Type-Options: nosniff`
   - `X-Frame-Options: DENY`
   - `Referrer-Policy: no-referrer`
   - `X-Permitted-Cross-Domain-Policies: none`
   - strips `X-Powered-By`
   - and the API Content-Security-Policy (see ADR-0002).
5. **Fixed, documented pipeline order** in `Program.cs`:

   ```
   ForwardedHeaders
     → ExceptionHandling            (outermost app-level handler)
     → HSTS (prod only)
     → SecurityHeaders (+ API CSP)
     → CORS
     → Swagger (dev only)
     → seeding (dev only demo data)
     → Authentication
     → RateLimiter
     → Authorization
     → TenantSiteMiddleware
     → MapControllers / MapHub
   ```

## Implementation notes

- **Files:** `DocAnalytics.Api/Middleware/SecurityHeadersMiddleware.cs` (+ `UseSecurityHeaders()` extension), `DocAnalytics.Api/Configuration/SecurityOptions.cs` + `SecurityFoundationExtensions.cs` (`AddSecurityFoundation`, registered in R0), `Program.cs` (pipeline wiring), `appsettings.json` / `appsettings.Development.json` / `appsettings.Production.json` (the `Security` section), `.env.example` (`Security__*` env keys).
- **Config surface:** `Security:Cors:AllowedOrigins[]`, `Security:Hsts:Enabled`, `Security:ForwardedHeaders:Enabled`.
- **.NET 10 quirks:**
  - `KnownNetworks` is deprecated → use `KnownIPNetworks`.
  - The `ForwardedHeaders` enum name collides with the middleware options property → fully-qualify `Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | XForwardedProto`.
  - `CorsOptions.PolicyName` resolves to the project’s own `DocAnalytics.Api.Configuration.CorsOptions` — fully-qualify only if a framework-type clash appears.
- **nginx forwarding:** the SPA container’s nginx sets `proxy_set_header X-Forwarded-For` and `X-Forwarded-Proto $scheme` so the forwarded-headers middleware has correct values in the containerized/local path.

## Consequences

**Positive**
- Consistent, testable security headers on **every** response (verified by E2E `headers.spec.ts`).
- CORS and HSTS are environment-driven — dev stays frictionless; prod is locked to one origin without a code change.
- No redirect loop behind nginx/ALB; forwarded-headers is the correct pattern for a TLS-terminating proxy.
- Removing the duplicate/inline CORS policy eliminated a subtle source of drift.

**Negative / trade-offs**
- HTTP→HTTPS redirection is now an **infrastructure** concern (ALB/nginx), not an app concern — easy to overlook when reasoning about the app in isolation. Documented here and in the deploy runbook.
- Forwarded-headers trust must be scoped to the known proxy hop; misconfiguring it (trusting arbitrary proxies) could allow spoofed `X-Forwarded-*`. Mitigated by clearing/known-proxy config.
- HSTS `preload` is a long-lived commitment (browsers cache it) — acceptable for a dedicated domain (`docanalytics.dev`).

## Testing & verification

- **Manual:** confirm security headers present on API responses; HSTS **absent** in dev, **present** in prod; `ng serve` (4200) CORS intact; `docker compose up --build` produces **no** 307/308 redirect loop.
- **Automated:** Playwright `e2e/headers.spec.ts` asserts the security headers on responses.
- **Behind the ALB:** the deploy smoke test hits `GET /api/v1/health` over HTTPS and expects `200` + `"db":"connected"`.

## Operational notes

- Corporate networks running Cisco Umbrella / Secure Access may **inject or rewrite** headers (including CSP) — this is external to our policy and was confirmed as a red herring during rollout.
- The known benign log noise `Cannot load library libgssapi_krb5.so.2` (Kerberos) does not affect Postgres/TLS.

## Related PRs / commits

- `feat(security): R1 transport hardening (HSTS, security headers, CORS lock-down)`
- `chore(round-0): Security & CI/CD pre-flight foundation` (config-driven `SecurityOptions`)
- `security: production hardening (seeding, CORS/HSTS, CSP, password policy, HttpOnly refresh cookie, dependabot)` (PR #113 — item #4)

## Follow-ups

- Keep the ALB HTTP→HTTPS redirect + ACM cert codified in Terraform so it can’t drift.
- Consider adding `Permissions-Policy` / `Cross-Origin-*` headers if new browser features are adopted.
