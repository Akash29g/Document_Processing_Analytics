import { test, expect, Page } from '@playwright/test';

// A8 smoke tests run AUTHENTICATED (shared storageState).

async function gotoDashboard(page: Page): Promise<string> {
  await page.goto('/login');
  await page.waitForURL(/\/site\/[^/]+\/dashboard/, { timeout: 15_000 });
  return page.url().match(/\/site\/([^/]+)\//)![1];
}

test.describe('Smoke', () => {
  test('dashboard renders stat cards and charts', async ({ page }) => {
    await gotoDashboard(page);
    await expect(page.locator('app-stat-card').first()).toBeVisible({ timeout: 10_000 });
    await expect(page.locator('app-chart-card').first()).toBeVisible({ timeout: 10_000 });
  });

  test('batches page renders the data table', async ({ page }) => {
    const siteId = await gotoDashboard(page);
    await page.goto(`/site/${siteId}/batches`);
    await expect(page.locator('app-data-table')).toBeVisible({ timeout: 10_000 });
  });

  test('batches list pagination works', async ({ page }) => {
    const siteId = await gotoDashboard(page);
    await page.goto(`/site/${siteId}/batches`);

    const footer = page.locator('.dt-footer');
    await expect(footer).toBeVisible({ timeout: 10_000 });

    const pageInfo = page.locator('.page-info');
    await expect(pageInfo).toContainText('Page 1');

    // Only exercise navigation if there's more than one page of data.
    const nextBtn = page.getByRole('button', { name: 'Next' });
    if (await nextBtn.isEnabled()) {
      await nextBtn.click();
      await expect(pageInfo).toContainText('Page 2');
      await page.getByRole('button', { name: 'Prev' }).click();
      await expect(pageInfo).toContainText('Page 1');
    }
  });

  test('errors CSV export triggers a download', async ({ page }) => {
    const siteId = await gotoDashboard(page);
    await page.goto(`/site/${siteId}/errors`);

    const downloadPromise = page.waitForEvent('download');
    await page.click('button.export-btn');
    const download = await downloadPromise;

    expect(download.suggestedFilename()).toContain('.csv');
  });
});
