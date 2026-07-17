import { test as setup, expect } from '@playwright/test';
import path from 'node:path';

// Where the reusable authenticated state is saved (matches playwright.config.ts).
const authFile = path.join(__dirname, '.auth', 'state.json');

// Creds from env in CI; fall back to local dev account.
const EMAIL = process.env.E2E_EMAIL ?? 'admin@acme.com';
const PASSWORD = process.env.E2E_PASSWORD ?? 'Password123!';

setup('authenticate via UI and save storageState', async ({ page }) => {
  await page.goto('/login');
  await page.fill('input[formControlName="email"]', EMAIL);
  await page.fill('input[formControlName="password"]', PASSWORD);
  await page.click('button[type="submit"].btn');

  // Admin -> routed to first site dashboard
  await page.waitForURL('**/site/*/dashboard', { timeout: 15_000 });

  // Verify JWT persisted under the agreed key
  const token = await page.evaluate(() => localStorage.getItem('da_token'));
  expect(token, 'da_token should be set in localStorage after login').toBeTruthy();

  // Save cookies + localStorage (captures da_token) for reuse
  await page.context().storageState({ path: authFile });
});
