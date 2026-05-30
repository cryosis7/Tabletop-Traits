import { test, expect } from "@playwright/test";
import { syncCollection, fixtureGameCount, fixtureUsername } from "./helpers";

test.describe("Collection table", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
    await syncCollection(page);
  });

  test("displays correct column headers", async ({ page }) => {
    const table = page.getByRole("table");
    await expect(table.getByRole("columnheader", { name: "Game" })).toBeVisible();
    await expect(table.getByRole("columnheader", { name: "Year" })).toBeVisible();
    await expect(table.getByRole("columnheader", { name: "Your Rating" })).toBeVisible();
    await expect(table.getByRole("columnheader", { name: "Mechanisms" })).toBeVisible();
  });

  test("displays mechanisms for known games", async ({ page }) => {
    const table = page.getByRole("table");

    await expect(table.getByRole("row", { name: /Dune: Imperium/ })).toContainText(
      "Worker Placement"
    );
    await expect(table.getByRole("row", { name: /5-Minute Dungeon/ })).toContainText(
      "Hand Management"
    );
    await expect(table.getByRole("row", { name: /Dominion/ })).toContainText(
      "Deck, Bag, and Pool Building"
    );
  });

  test("games are sorted by rating descending", async ({ page }) => {
    const ratingCells = page.getByRole("table").locator("tbody td.rating");
    const ratings = await ratingCells.allTextContents();
    const numericRatings = ratings.map(Number);

    for (let i = 0; i < numericRatings.length - 1; i++) {
      expect(numericRatings[i]).toBeGreaterThanOrEqual(numericRatings[i + 1]);
    }
  });

  test("highlights mechanisms that are selected in the filter", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("Worker");
    await page.locator(".filter-dropdown").getByText("Worker Placement", { exact: true }).click();

    // Mechanism text in the table should have highlight class
    const highlighted = page.getByRole("table").locator(".mechanism-highlight");
    await expect(highlighted.first()).toBeVisible();
    await expect(highlighted.first()).toContainText("Worker Placement");
  });

  test("shows correct row count matching game count", async ({ page }) => {
    const rows = page.getByRole("table").locator("tbody tr");
    await expect(rows).toHaveCount(fixtureGameCount);
  });

  test("displays a link to the user's BGG collection", async ({ page }) => {
    const link = page.getByRole("link", { name: "View on BGG" });
    await expect(link).toBeVisible();
    await expect(link).toHaveAttribute(
      "href",
      `https://boardgamegeek.com/collection/user/${fixtureUsername}`
    );
    await expect(link).toHaveAttribute("target", "_blank");
  });
});
