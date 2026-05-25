import { defineConfig } from "@playwright/test";

export default defineConfig({
  testDir: "./e2e",
  fullyParallel: false,
  retries: 0,
  use: {
    baseURL: "http://localhost:5173",
    headless: true,
  },
  projects: [
    {
      name: "chromium",
      use: { browserName: "chromium" },
    },
  ],
  webServer: [
    {
      command: "dotnet run --project ../backend/src/BoardGameRankings.Api",
      url: "http://localhost:5237/openapi/v1.json",
      reuseExistingServer: !process.env.CI,
      timeout: 30000,
      env: {
        ASPNETCORE_ENVIRONMENT: "Development",
      },
    },
    {
      command: "npm run dev",
      url: "http://localhost:5173",
      reuseExistingServer: !process.env.CI,
      timeout: 15000,
    },
  ],
});
