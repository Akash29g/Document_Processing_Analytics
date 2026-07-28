import { test, expect } from '@playwright/test';

// A6 runs AUTHENTICATED (shared storageState = admin@acme.com, Acme sites only).
// admin@acme has NO access to Globex sites, so visiting one must redirect
// to the first allowed (Acme) site.

test.describe('Site-access guard', () => {
  test('visiting a site without access redirects to the first allowed site', async ({ page }) => {
    // A real Globex site the Acme admin cannot access (from the DB seed).
    const noAccessSite = 'b1111111-1111-1111-1111-111111111111';

    await page.goto(`/site/${noAccessSite}/dashboard`);

    // The guard's /auth/me + redirect is async — WAIT until we've been moved
    // OFF the no-access site to a real dashboard (not the fake one, not /login).
    await page.waitForURL(
      (url) =>
        /\/site\/[^/]+\/dashboard$/.test(url.pathname) && !url.pathname.includes(noAccessSite),
      { timeout: 15_000 },
    );

    expect(page.url()).not.toContain(noAccessSite);
    expect(page.url()).not.toContain('/login');
  });
});
