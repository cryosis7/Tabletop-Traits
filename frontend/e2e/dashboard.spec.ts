import { test, expect, type Page } from "@playwright/test";

const fixtureUsername = "testuser";
const fixtureGames = ["5-Minute Dungeon", "Dune: Imperium", "Dominion"];
const paginatedFixtureUsername = "Brezman";
const paginatedFixtureGames = ["1775: Rebellion", "Star Wars: Outer Rim", "Wingspan"];
const paginatedFixtureGameCount = 370;

async function syncCollection(
  page: Page,
  username: string = fixtureUsername,
  expectedGameCount: number = 5
): Promise<void> {
  await page.getByPlaceholder("Enter your BGG username").fill(username);
  await page.getByRole("button", { name: "Sync & Analyze" }).click();

  await expect(page.getByRole("status")).toContainText(`Synced ${expectedGameCount} games from BGG`);
  await expect(page.getByRole("region", { name: "Mechanism analysis" })).toBeVisible();
  await expect(
    page.getByRole("heading", { level: 2, name: `Your Rated Games (${expectedGameCount})` })
  ).toBeVisible();
  await expect(page.getByRole("table")).toBeVisible();
}

test.describe("Board Game Rankings E2E", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
  });

  test("syncs a user collection and displays mechanism analysis", async ({ page }) => {
    await expect(
      page.getByRole("heading", {
        level: 1,
        name: "Board Game Mechanism Analyzer",
      })
    ).toBeVisible();

    await syncCollection(page);

    await expect(page.getByRole("button", { name: "Average Rating" })).toHaveAttribute(
      "aria-pressed",
      "true"
    );
    await expect(page.getByRole("button", { name: "Bar Chart" })).toHaveAttribute(
      "aria-pressed",
      "true"
    );

    const collectionTable = page.getByRole("table");
    await expect(collectionTable.getByRole("row")).toHaveCount(6);

    for (const gameName of fixtureGames) {
      await expect(collectionTable.getByRole("cell", { name: gameName })).toBeVisible();
    }
  });

  test("syncs Brezman's paginated collection and includes games from page 2", async ({ page }) => {
    test.slow();

    await syncCollection(page, paginatedFixtureUsername, paginatedFixtureGameCount);

    const collectionTable = page.getByRole("table");
    for (const gameName of paginatedFixtureGames) {
      await expect(collectionTable.getByRole("cell", { name: gameName })).toBeVisible();
    }
  });

  test("switches scoring mode between average and cumulative", async ({ page }) => {
    await syncCollection(page);

    const averageBtn = page.getByRole("button", { name: "Average Rating" });
    const cumulativeBtn = page.getByRole("button", { name: "Cumulative" });
    await expect(averageBtn).toHaveAttribute("aria-pressed", "true");
    await expect(cumulativeBtn).toHaveAttribute("aria-pressed", "false");

    await cumulativeBtn.click();
    await expect(cumulativeBtn).toHaveAttribute("aria-pressed", "true");
    await expect(averageBtn).toHaveAttribute("aria-pressed", "false");
  });

  test("switches between chart tabs", async ({ page }) => {
    await syncCollection(page);

    const barBtn = page.getByRole("button", { name: "Bar Chart" });
    const radarBtn = page.getByRole("button", { name: "Radar" });
    const scatterBtn = page.getByRole("button", { name: "Scatter" });
    await expect(barBtn).toHaveAttribute("aria-pressed", "true");
    await expect(radarBtn).toHaveAttribute("aria-pressed", "false");
    await expect(scatterBtn).toHaveAttribute("aria-pressed", "false");

    await radarBtn.click();
    await expect(radarBtn).toHaveAttribute("aria-pressed", "true");
    await expect(barBtn).toHaveAttribute("aria-pressed", "false");

    await scatterBtn.click();
    await expect(scatterBtn).toHaveAttribute("aria-pressed", "true");
    await expect(radarBtn).toHaveAttribute("aria-pressed", "false");
  });

  test("displays mechanisms in the collection table", async ({ page }) => {
    await syncCollection(page);

    const collectionTable = page.getByRole("table");
    await expect(collectionTable.getByRole("row", { name: /Dune: Imperium/ })).toContainText(
      "Worker Placement"
    );
    await expect(collectionTable.getByRole("row", { name: /5-Minute Dungeon/ })).toContainText(
      "Hand Management"
    );
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
    await expect(page.getByRole("status")).toContainText("Synced 5 games from BGG");
  });
});
