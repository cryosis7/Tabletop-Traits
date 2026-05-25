import os from "node:os";
import path from "node:path";
import { defineConfig } from "@playwright/test";

const playwrightDataPath = path.join(os.tmpdir(), "board-game-rankings-playwright-data");

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
        DataPath: playwrightDataPath,
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
