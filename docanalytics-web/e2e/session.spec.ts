import { test, expect } from '@playwright/test';

// A5 runs AUTHENTICATED (uses the shared storageState with da_token).
// We intentionally do NOT clear storageState here.

test.describe('Session rehydration', () => {
  test('reload with token but empty signals recovers via /auth/me', async ({ page }) => {
    // With a live token, hitting /login auto-routes us to the dashboard.
    await page.goto('/login');
    await page.waitForURL(/\/site\/[^/]+\/dashboard/, { timeout: 15_000 });
    const dashUrl = page.url();

    // Sanity: token is present in localStorage.
    const token = await page.evaluate(() => localStorage.getItem('da_token'));
    expect(token).toBeTruthy();

    // Reload wipes in-memory signals; the guard must call /auth/me to rehydrate.
    const [meResponse] = await Promise.all([
      page.waitForResponse((r) => r.url().includes('/auth/me') && r.request().method() === 'GET'),
      page.reload(),
    ]);
    expect(meResponse.ok()).toBeTruthy();

    // Session recovered -> we STAY on the dashboard, not bounced to /login.
    await expect(page).toHaveURL(dashUrl);
  });
});
