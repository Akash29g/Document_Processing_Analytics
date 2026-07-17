import { test, expect } from '@playwright/test';

// A4 login tests must run LOGGED OUT — override the shared storageState.
test.use({ storageState: { cookies: [], origins: [] } });

const EMAIL = process.env.E2E_EMAIL ?? 'admin@acme.com';
const PASSWORD = process.env.E2E_PASSWORD ?? 'Password123!';

const DEV_EMAIL = process.env.E2E_DEV_EMAIL ?? 'developer@platform.com';
const DEV_PASSWORD = process.env.E2E_DEV_PASSWORD ?? 'Password123!';

test.describe('Login', () => {
  test('valid admin login routes to a site dashboard', async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[formControlName="email"]', EMAIL);
    await page.fill('input[formControlName="password"]', PASSWORD);
    await page.click('button[type="submit"].btn');

    await expect(page).toHaveURL(/\/site\/[^/]+\/dashboard/, { timeout: 15_000 });

    const token = await page.evaluate(() => localStorage.getItem('da_token'));
    expect(token).toBeTruthy();
  });

  test('valid Developer login routes to /provision', async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[formControlName="email"]', DEV_EMAIL);
    await page.fill('input[formControlName="password"]', DEV_PASSWORD);
    await page.click('button[type="submit"].btn');

    await expect(page).toHaveURL(/\/provision$/, { timeout: 15_000 });
  });

  test('invalid credentials show inline error and stay on /login', async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[formControlName="email"]', EMAIL);
    await page.fill('input[formControlName="password"]', 'WrongPassword!');
    await page.click('button[type="submit"].btn');

    await expect(page.locator('.alert[role="alert"]')).toHaveText('Invalid email or password.');
    await expect(page).toHaveURL(/\/login$/);

    const token = await page.evaluate(() => localStorage.getItem('da_token'));
    expect(token).toBeFalsy();
  });
});
