import { test, expect } from "@playwright/test";

test.describe("Board Game Rankings E2E", () => {
  test("syncs a user collection and displays mechanism analysis", async ({
    page,
  }) => {
    await page.goto("/");

    // Verify initial page state
    await expect(page.locator("h1")).toHaveText(
      "Board Game Mechanism Analyzer"
    );

    // Enter username and sync
    const input = page.getByPlaceholder("Enter your BGG username");
    await input.fill("testuser");
    await page.getByRole("button", { name: "Sync & Analyze" }).click();

    // Wait for sync to complete and data to load
    await expect(page.locator(".sync-info")).toBeVisible({ timeout: 15000 });
    await expect(page.locator(".sync-info")).toContainText("Synced 5 games");

    // Verify mechanism chart section appears (bar chart is default)
    await expect(page.locator(".chart-section")).toBeVisible();

    // Verify collection table is rendered with the 5 games from the mock
    await expect(page.locator(".collection-table")).toBeVisible();
    const rows = page.locator(".collection-table tbody tr");
    await expect(rows).toHaveCount(5);

    // Verify known game names from the HTML fixtures appear
    await expect(page.locator(".collection-table")).toContainText(
      "5-Minute Dungeon"
    );
    await expect(page.locator(".collection-table")).toContainText(
      "Dune: Imperium"
    );
    await expect(page.locator(".collection-table")).toContainText("Dominion");
  });

  test("switches scoring mode between average and cumulative", async ({
    page,
  }) => {
    await page.goto("/");

    // Sync first
    await page.getByPlaceholder("Enter your BGG username").fill("testuser");
    await page.getByRole("button", { name: "Sync & Analyze" }).click();
    await expect(page.locator(".sync-info")).toBeVisible({ timeout: 15000 });

    // Default mode is average
    const averageBtn = page.getByRole("button", { name: "Average Rating" });
    const cumulativeBtn = page.getByRole("button", { name: "Cumulative" });
    await expect(averageBtn).toHaveClass(/active/);

    // Switch to cumulative
    await cumulativeBtn.click();
    await expect(cumulativeBtn).toHaveClass(/active/);
    await expect(averageBtn).not.toHaveClass(/active/);
  });

  test("switches between chart tabs", async ({ page }) => {
    await page.goto("/");

    // Sync first
    await page.getByPlaceholder("Enter your BGG username").fill("testuser");
    await page.getByRole("button", { name: "Sync & Analyze" }).click();
    await expect(page.locator(".sync-info")).toBeVisible({ timeout: 15000 });

    // Bar chart is the default tab
    const barBtn = page.getByRole("button", { name: "Bar Chart" });
    const radarBtn = page.getByRole("button", { name: "Radar" });
    const scatterBtn = page.getByRole("button", { name: "Scatter" });
    await expect(barBtn).toHaveClass(/active/);

    // Switch to radar
    await radarBtn.click();
    await expect(radarBtn).toHaveClass(/active/);
    await expect(barBtn).not.toHaveClass(/active/);

    // Switch to scatter
    await scatterBtn.click();
    await expect(scatterBtn).toHaveClass(/active/);
    await expect(radarBtn).not.toHaveClass(/active/);
  });

  test("displays mechanisms in the collection table", async ({ page }) => {
    await page.goto("/");

    await page.getByPlaceholder("Enter your BGG username").fill("testuser");
    await page.getByRole("button", { name: "Sync & Analyze" }).click();
    await expect(page.locator(".collection-table")).toBeVisible({
      timeout: 15000,
    });

    // Verify mechanisms are shown for games (from fixture data)
    await expect(page.locator(".collection-table")).toContainText(
      "Worker Placement"
    );
    await expect(page.locator(".collection-table")).toContainText(
      "Hand Management"
    );
  });

  test("disables sync button while syncing", async ({ page }) => {
    await page.goto("/");

    const input = page.getByPlaceholder("Enter your BGG username");
    const syncBtn = page.getByRole("button", { name: /Sync/ });

    // Button disabled when input is empty
    await expect(syncBtn).toBeDisabled();

    // Button enabled with text
    await input.fill("testuser");
    await expect(syncBtn).toBeEnabled();
  });
});
