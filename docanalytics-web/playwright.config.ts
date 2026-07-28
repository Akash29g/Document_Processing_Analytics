import { defineConfig, devices } from '@playwright/test';

// Env override lets the SAME config run locally now and on ECS later.
const BASE_URL = process.env.E2E_BASE_URL ?? 'http://localhost:4200';

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI, // fail CI if a stray .only is committed
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  timeout: 30_000,
  expect: { timeout: 5_000 },

  reporter: [['html', { outputFolder: 'playwright-report', open: 'never' }], ['list']],

  use: {
    baseURL: BASE_URL,
    trace: 'on-first-retry', // trace on failure
    screenshot: 'only-on-failure', // screenshot on failure
    video: 'retain-on-failure',
  },

  projects: [
    // A3: runs first, logs in once, writes e2e/.auth/state.json
    { name: 'setup', testMatch: /.*\.setup\.ts/ },

    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'], storageState: 'e2e/.auth/state.json' },
      dependencies: ['setup'],
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'], storageState: 'e2e/.auth/state.json' },
      dependencies: ['setup'],
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'], storageState: 'e2e/.auth/state.json' },
      dependencies: ['setup'],
    },
  ],

  // Auto-start the Angular dev server for tests.
  // NOTE: backend (:7001) must be running — ng serve only PROXIES to it.
  webServer: {
    command: 'npm start',
    url: BASE_URL,
    reuseExistingServer: !process.env.CI,
    timeout: 120_000,
  },
});
