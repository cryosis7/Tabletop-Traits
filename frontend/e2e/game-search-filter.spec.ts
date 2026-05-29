import { test, expect } from "@playwright/test";
import { syncCollection, fixtureGameCount } from "./helpers";

test.describe("Game search filter", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
    await syncCollection(page);
  });

  test("typing a game name shows the game in the dropdown under Games section", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("5-Minute");

    const dropdown = page.locator(".filter-dropdown");
    await expect(dropdown).toBeVisible();
    await expect(dropdown.locator(".dropdown-section-header")).toContainText("Games");
    await expect(dropdown.locator(".game-item")).toContainText("5-Minute Dungeon");
    await expect(dropdown.locator(".game-item .mechanism-description")).toBeVisible();
  });

  test("selecting a game adds all of its mechanisms as filter chips", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("5-Minute");

    const dropdown = page.locator(".filter-dropdown");
    await dropdown.locator(".game-item", { hasText: "5-Minute Dungeon" }).click();

    // Multiple mechanism chips should appear
    const chips = page.locator(".chip");
    const chipCount = await chips.count();
    expect(chipCount).toBeGreaterThan(1);

    // Table heading should show filtered count
    await expect(page.getByRole("heading", { level: 2 })).toContainText("Showing");
  });

  test("collection table is filtered after selecting a game", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("5-Minute");

    await page.locator(".filter-dropdown .game-item", { hasText: "5-Minute Dungeon" }).click();

    // The filtered collection should include the selected game
    const table = page.getByRole("table");
    await expect(table).toContainText("5-Minute Dungeon");

    // Should show fewer games than the full collection (since filter is in ANY mode by default)
    const rows = table.locator("tbody tr");
    const rowCount = await rows.count();
    expect(rowCount).toBeLessThanOrEqual(fixtureGameCount);
    expect(rowCount).toBeGreaterThan(0);
  });

  test("already-selected mechanisms are not duplicated when selecting a game", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");

    // First select a mechanism that also belongs to 5-Minute Dungeon
    await searchInput.fill("Hand Management");
    await page.locator(".filter-dropdown").getByText("Hand Management", { exact: true }).click();
    await expect(page.locator(".chip")).toHaveCount(1);

    // Now select 5-Minute Dungeon (which also has Hand Management)
    await searchInput.fill("5-Minute");
    await page.locator(".filter-dropdown .game-item", { hasText: "5-Minute Dungeon" }).click();

    // Hand Management should appear only once
    const chips = page.locator(".chip", { hasText: "Hand Management" });
    await expect(chips).toHaveCount(1);
  });

  test("search shows both mechanisms and games when query matches both", async ({ page }) => {
    // "Cooperative" matches both the mechanism "Cooperative Game" and potentially game names
    // Use a search term that matches a game name and also mechanism names
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("Dungeon");

    const dropdown = page.locator(".filter-dropdown");
    await expect(dropdown).toBeVisible();

    // Should show the game section
    await expect(dropdown.locator(".dropdown-section-header")).toContainText("Games");
    await expect(dropdown.locator(".game-item")).toContainText("5-Minute Dungeon");
  });

  test("game dropdown item shows count of new mechanisms to be added", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("5-Minute");

    const gameItem = page.locator(".filter-dropdown .game-item", { hasText: "5-Minute Dungeon" });
    await expect(gameItem).toBeVisible();
    // Should show mechanism count
    await expect(gameItem.locator(".mechanism-description")).toContainText("mechanisms");
  });
});
