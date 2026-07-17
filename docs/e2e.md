# End-to-End Tests (Playwright)

Browser-based end-to-end (E2E) tests for the DocAnalytics web app, written with
[Playwright](https://playwright.dev). They drive a **real browser** against the
**real Angular app + backend API**, exercising login, routing, guards, the auth
interceptor, and core page smoke flows.

All E2E files live under `docanalytics-web/e2e/`.

---

## Prerequisites

- **Node 22** and dependencies installed (`npm ci` in `docanalytics-web/`)
- **Playwright browsers** installed:
  ```powershell
  npx playwright install
