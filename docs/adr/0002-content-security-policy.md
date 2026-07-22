# ADR-0002: Content Security Policy (API + SPA)

| | |
|---|---|
| **Status** | Accepted (with a documented `script-src 'unsafe-inline'` limitation on the SPA) |
| **Date** | `harden/prod-security` pass (item #5) |
| **Deciders** | Akash (Dev A), cross-reviewed by Shubh (Dev B) |
| **Related** | ADR-0001 (the API CSP is emitted by `SecurityHeadersMiddleware`) |

---

## Context

The application exposes **two very different HTTP surfaces**, and a single Content-Security-Policy cannot serve both well:

1. **The JSON API** (ASP.NET Core) never returns HTML. It should never be able to act as a script host or be framed — the strictest possible CSP is appropriate.
2. **The Angular SPA**, served as static HTML by nginx, legitimately loads Google Fonts (styles + font files), renders data-URI images, executes application JavaScript, and opens `https`/`wss` connections to the API and the SignalR hub.

Additional forces discovered during rollout of the `harden/prod-security` branch (the CSP header broke the deployed app in a cascade before being tuned):

- A **strict `script-src 'self'`** (no `unsafe-inline`) **broke dark mode** in production. The theme toggle code itself is clean (Angular `(click)` + `setAttribute` in `theme.service.ts`), but empirically some inline execution on the runtime/chart path is blocked under strict CSP, and the theme visually failed (icon flipped but background did not).
- **Fonts/icons broke** under an over-strict early CSP: Material Icons rendered as literal text (“notifications”, “expand_more”) because `font-src` / `style-src` did not allow Google Fonts.
- **Corporate networks** (Cisco Umbrella / Secure Access, `*.sse.cisco.com`, `*.opendns.com`) can inject or rewrite CSP headers on the wire — an environment artifact, not a defect in our policy (initially a red herring during debugging).

## Alternatives considered

- **One shared CSP for both surfaces** — rejected: the API wants `default-src 'none'` while the SPA needs fonts, inline styles, images and socket connections. A shared policy would be either too loose for the API or too strict for the SPA.
- **Strict `script-src 'self'` on the SPA (no `unsafe-inline`)** — attempted and reverted: it broke dark mode, and diagnosing/rewriting the offending inline execution (suspected chart/runtime path) was not worth blocking the release.
- **Nonce/hash-based `script-src`** — the correct long-term answer, but requires build-time nonce injection or per-inline hashes; deferred as a low-priority follow-up.
- **CSP via `<meta>` tag in `index.html`** — rejected for the SPA: `frame-ancestors` and some directives are ignored in meta form; the header on the nginx layer is authoritative.

## Decision

**Two separate policies, each set at the layer that owns its surface.**

### API (JSON-only) — set in `SecurityHeadersMiddleware`

Maximally restrictive; the API is not a browser rendering context:

```
Content-Security-Policy: default-src 'none'; frame-ancestors 'none'; base-uri 'none'; form-action 'none'
```

### SPA — set on the nginx layer (`docanalytics-web/nginx.conf`)

```
default-src 'self';
script-src 'self' 'unsafe-inline';
style-src 'self' 'unsafe-inline' https://fonts.googleapis.com;
font-src 'self' https://fonts.gstatic.com data:;
img-src 'self' data:;
connect-src 'self' https: wss:;
frame-ancestors 'none';
base-uri 'self';
object-src 'none'
```

Rationale for each SPA directive:

- `script-src 'self' 'unsafe-inline'` — app scripts from same origin; `'unsafe-inline'` is a **deliberate, documented trade-off** (see Consequences) kept because strict `'self'` broke dark mode.
- `style-src 'self' 'unsafe-inline' https://fonts.googleapis.com` — Angular emits inline styles; Google Fonts stylesheet.
- `font-src 'self' https://fonts.gstatic.com data:` — Material Icons / Google font files + data-URI fonts.
- `img-src 'self' data:` — app images + inlined data-URI images.
- `connect-src 'self' https: wss:` — API calls (https) + SignalR hub (wss).
- `frame-ancestors 'none'` + `base-uri 'self'` + `object-src 'none'` — anti-clickjacking / injection hardening.

## Implementation notes

- **API CSP** lives in `DocAnalytics.Api/Middleware/SecurityHeadersMiddleware.cs` alongside the other security headers (ADR-0001).
- **SPA CSP** lives as an `add_header Content-Security-Policy "..."` in `docanalytics-web/nginx.conf`, on the `location /` that serves the SPA. `nginx.conf` is copied verbatim into the image (`COPY nginx.conf /etc/nginx/conf.d/default.conf`) — the earlier envsubst/templating approach was abandoned because `${...}` placeholders were left un-substituted.
- In production the ALB path-routes `/api` to the API before nginx, so the SPA CSP and the API CSP apply on their respective responses.

## Consequences

**Positive**
- The API cannot be used as a script or framing context (`default-src 'none'`).
- The SPA gets a real, meaningful CSP that blocks unexpected origins while still allowing Google Fonts, data-URI assets, and the API/WebSocket connections it needs.
- Clear ownership: **API CSP in middleware, SPA CSP in nginx** — each policy is edited where its surface is served.

**Negative / trade-offs**
- **`script-src 'unsafe-inline'` weakens the SPA’s XSS mitigation.** This is the primary accepted risk. It was retained because strict `'self'` broke dark mode and the cost/benefit did not justify blocking the release. Reclaiming strict `script-src 'self'` is a tracked, low-priority follow-up.
- CSP can appear “broken” on corporate networks that inject/rewrite headers — external, not a defect in our policy; documented to avoid future misdiagnosis.
- Two policies to maintain instead of one; they must be reviewed together when a new external origin (e.g. a CDN) is introduced.

## Testing & verification

- **Manual:** after a rebuild, always “Empty Cache and Hard Reload” (cache was a recurring villain during CSP debugging); verify Material Icons render (not literal text), dark mode toggles the background, and no CSP violations appear in DevTools console.
- **Automated:** the E2E `headers.spec.ts` can assert the CSP header is present on responses.

## Related PRs / commits

- `security: production hardening (… CSP …)` (PR #113 — item #5)
- `fix(web): restore /api+/hubs nginx proxy + lazy DNS + CSP font sources` (the follow-up that tuned the SPA CSP so fonts/icons/dark mode worked)

## Follow-ups

- **(Low priority)** Diagnose the inline-execution source (suspected chart/runtime path), then move the SPA to a **nonce/hash-based `script-src 'self'`** and drop `'unsafe-inline'`.
- Codify the nginx CSP header alongside the SPA build in IaC so it cannot silently drift between environments.
- Consider a CSP report-only endpoint to gather violations before tightening.
