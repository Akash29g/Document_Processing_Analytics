import { test, expect } from '@playwright/test';

// A7 runs AUTHENTICATED. Verifies auth-site.interceptor adds
// Authorization: Bearer <token> AND X-Site-Id on API calls made from a site page.

test.describe('Auth-site interceptor headers', () => {
  test('API calls carry Authorization and X-Site-Id', async ({ page }) => {
    await page.goto('/login');
    await page.waitForURL(/\/site\/[^/]+\/dashboard/, { timeout: 15_000 });

    // Reload and capture a data API call (exclude /auth/* which fires before site is set).
    const [req] = await Promise.all([
      page.waitForRequest(
        (r) => r.url().includes('/api/v1/') && !r.url().includes('/auth/'),
      ),
      page.reload(),
    ]);

    const headers = req.headers();
    expect(headers['authorization']).toMatch(/^Bearer .+/);
    expect(headers['x-site-id']).toBeTruthy();
  });
});
