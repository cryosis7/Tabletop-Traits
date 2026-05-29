import { test, expect } from "@playwright/test";
import {
  syncCollection,
  fixtureUsername,
  fixtureGameCount,
  fixtureGames,
  paginatedFixtureUsername,
  paginatedFixtureGameCount,
  paginatedFixtureGames,
} from "./helpers";

test.describe("Sync flow", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
  });

  test("syncs a user collection and displays mechanism analysis", async ({ page }) => {
    await expect(
      page.getByRole("heading", { level: 1, name: "Board Game Mechanism Analyzer" })
    ).toBeVisible();

    await syncCollection(page);

    await expect(page.locator("#scoring-mode-select")).toContainText("Arithmetic Mean");
    await expect(page.getByRole("button", { name: "Bar Chart" })).toHaveAttribute(
      "aria-pressed",
      "true"
    );

    const collectionTable = page.getByRole("table");
    await expect(collectionTable.getByRole("row")).toHaveCount(fixtureGameCount + 1);

    for (const gameName of fixtureGames) {
      await expect(collectionTable.getByRole("cell", { name: gameName })).toBeVisible();
    }
  });

  test("syncs a paginated collection and includes games from page 2", async ({ page }) => {
    test.slow();

    await syncCollection(page, paginatedFixtureUsername, paginatedFixtureGameCount);

    const collectionTable = page.getByRole("table");
    for (const gameName of paginatedFixtureGames) {
      await expect(collectionTable.getByRole("cell", { name: gameName })).toBeVisible();
    }
  });

  test("disables sync button while syncing", async ({ page }) => {
    const input = page.getByPlaceholder("Enter your BGG username");
    const syncBtn = page.getByRole("button", { name: /Sync/ });

    await expect(syncBtn).toBeDisabled();

    await input.fill(fixtureUsername);
    await expect(syncBtn).toBeEnabled();

    await page.route(`**/api/sync/${fixtureUsername}`, async (route) => {
      const response = await route.fetch();
      await new Promise((resolve) => setTimeout(resolve, 250));
      await route.fulfill({ response });
    });

    await syncBtn.click();
    await expect(syncBtn).toBeDisabled();
    await expect(syncBtn).toHaveText("Syncing...");
    await expect(page.getByRole("status")).toContainText(
      `Synced ${fixtureGameCount} games from BGG`
    );
  });

  test("keeps sync button disabled when username is empty or whitespace", async ({ page }) => {
    const input = page.getByPlaceholder("Enter your BGG username");
    const syncBtn = page.getByRole("button", { name: /Sync/ });

    await expect(syncBtn).toBeDisabled();

    await input.fill("   ");
    await expect(syncBtn).toBeDisabled();

    await input.fill("validuser");
    await expect(syncBtn).toBeEnabled();

    await input.fill("");
    await expect(syncBtn).toBeDisabled();
  });

  test("triggers sync with Enter key", async ({ page }) => {
    const input = page.getByPlaceholder("Enter your BGG username");
    await input.fill(fixtureUsername);
    await input.press("Enter");

    await expect(page.getByRole("status")).toContainText(
      `Synced ${fixtureGameCount} games from BGG`
    );
    await expect(page.getByRole("table")).toBeVisible();
  });

  test("shows an error when sync fails", async ({ page }) => {
    await page.route("**/api/sync/baduser", (route) =>
      route.fulfill({ status: 500, body: "Internal Server Error" })
    );

    await page.getByPlaceholder("Enter your BGG username").fill("baduser");
    await page.getByRole("button", { name: "Sync & Analyze" }).click();

    await expect(page.locator(".error")).toBeVisible();
  });
});
