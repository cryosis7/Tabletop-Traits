import { test, expect } from "@playwright/test";
import { syncCollection, fixtureGameCount } from "./helpers";

test.describe("Mechanism filter", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
    await syncCollection(page);
  });

  test("filters collection table when a mechanism is selected", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("Worker");

    const dropdown = page.locator(".filter-dropdown");
    await expect(dropdown).toBeVisible();
    await dropdown.getByText("Worker Placement", { exact: true }).click();

    // Table should show filtered count
    await expect(
      page.getByRole("heading", {
        level: 2,
        name: /Showing \d+ of \d+/,
      })
    ).toBeVisible();

    // All visible rows should have Worker Placement
    const rows = page.getByRole("table").locator("tbody tr");
    const rowCount = await rows.count();
    expect(rowCount).toBeGreaterThan(0);
    expect(rowCount).toBeLessThan(fixtureGameCount);

    for (let i = 0; i < rowCount; i++) {
      await expect(rows.nth(i)).toContainText("Worker Placement");
    }
  });

  test("ANY mode shows games with any selected mechanism", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");

    // Select Worker Placement
    await searchInput.fill("Worker Placement");
    await page.locator(".filter-dropdown").getByText("Worker Placement", { exact: true }).click();

    const heading = page.getByRole("heading", { level: 2 });
    const afterFirst = await heading.textContent();
    const matchFirst = afterFirst!.match(/Showing (\d+) of (\d+)/);
    expect(matchFirst).not.toBeNull();
    const countWithOne = parseInt(matchFirst![1]);

    // Select Deck, Bag, and Pool Building (overlaps with Dune: Imperium but adds Dominion)
    await searchInput.fill("Deck, Bag");
    await page.locator(".filter-dropdown").getByText("Deck, Bag, and Pool Building", { exact: true }).click();

    // ANY mode (default) - should show more games than just one mechanism
    const afterSecond = await heading.textContent();
    const matchSecond = afterSecond!.match(/Showing (\d+) of (\d+)/);
    expect(matchSecond).not.toBeNull();
    const countWithTwo = parseInt(matchSecond![1]);
    expect(countWithTwo).toBeGreaterThanOrEqual(countWithOne);
  });

  test("ALL mode shows only games with all selected mechanisms", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");

    // Select Hand Management (appears in many games)
    await searchInput.fill("Hand Management");
    await page.locator(".filter-dropdown").getByText("Hand Management", { exact: true }).click();

    // Select Cooperative Game (appears in fewer)
    await searchInput.fill("Cooperative Game");
    await page.locator(".filter-dropdown").getByText("Cooperative Game", { exact: true }).click();

    // Switch to ALL mode
    await page.getByRole("button", { name: "ALL", exact: true }).click();

    // Should show only games with BOTH mechanisms
    const rows = page.getByRole("table").locator("tbody tr");
    const rowCount = await rows.count();
    expect(rowCount).toBeGreaterThan(0);

    for (let i = 0; i < rowCount; i++) {
      await expect(rows.nth(i)).toContainText("Hand Management");
      await expect(rows.nth(i)).toContainText("Cooperative Game");
    }
  });

  test("removes a mechanism chip and restores filtered games", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("Worker");
    await page.locator(".filter-dropdown").getByText("Worker Placement", { exact: true }).click();

    // Verify filter is active
    await expect(page.getByRole("heading", { level: 2 })).toContainText("Showing");

    // Click the remove button on the chip
    const chip = page.locator(".chip", { hasText: "Worker Placement" });
    await chip.getByRole("button", { name: "\u00d7" }).click();

    // Table should show all games again
    await expect(
      page.getByRole("heading", { level: 2, name: `Your Rated Games (${fixtureGameCount})` })
    ).toBeVisible();
  });

  test("clear all button removes all filters", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");

    await searchInput.fill("Worker Placement");
    await page.locator(".filter-dropdown").getByText("Worker Placement", { exact: true }).click();

    // After selecting first mechanism, add a second one
    await searchInput.click();
    await searchInput.pressSequentially("Take That");
    const dropdown = page.locator(".filter-dropdown");
    await expect(dropdown).toBeVisible();
    await dropdown.getByText("Take That", { exact: true }).click();

    // Verify multiple chips
    await expect(page.locator(".chip")).toHaveCount(2);

    await page.getByRole("button", { name: "Clear all" }).click();

    // Chips should be gone
    await expect(page.locator(".chip")).toHaveCount(0);

    // Table should show all games
    await expect(
      page.getByRole("heading", { level: 2, name: `Your Rated Games (${fixtureGameCount})` })
    ).toBeVisible();
  });

  test("clicking a bar chart bar filters by that mechanism", async ({ page }) => {
    const chartSection = page.locator(".chart-section");
    const bars = chartSection.locator(".recharts-bar-rectangle");
    await expect(bars.first()).toBeVisible();

    // Click the first bar
    await bars.first().click();

    // A filter chip should appear
    await expect(page.locator(".chip").first()).toBeVisible();

    // Table heading should show filtered count
    await expect(page.getByRole("heading", { level: 2 })).toContainText("Showing");
  });

  test("filter search dropdown shows matching mechanisms with descriptions", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("Deck");

    const dropdown = page.locator(".filter-dropdown");
    await expect(dropdown).toBeVisible();
    await expect(dropdown.getByText("Deck, Bag, and Pool Building")).toBeVisible();
    // Should include a description
    await expect(dropdown.locator(".mechanism-description").first()).toBeVisible();
  });

  test("filter dropdown closes when clicking outside", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("Worker");

    const dropdown = page.locator(".filter-dropdown");
    await expect(dropdown).toBeVisible();

    // Click the page heading (outside the filter)
    await page.getByRole("heading", { level: 1 }).click();
    await expect(dropdown).not.toBeVisible();
  });

  test("shows BGG search link when mechanisms are selected", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("Worker");
    await page.locator(".filter-dropdown").getByText("Worker Placement", { exact: true }).click();

    const link = page.getByRole("link", { name: "Search BGG" });
    await expect(link).toBeVisible();
    await expect(link).toHaveAttribute("target", "_blank");

    const href = await link.getAttribute("href");
    expect(href).toContain("boardgamegeek.com/geeksearch.php");
    expect(href).toContain("propertyids");
    expect(href).toContain("2082");
  });

  test("BGG search link disappears when all mechanisms are cleared", async ({ page }) => {
    const searchInput = page.getByPlaceholder("Search mechanisms...");
    await searchInput.fill("Worker");
    await page.locator(".filter-dropdown").getByText("Worker Placement", { exact: true }).click();

    await expect(page.getByRole("link", { name: "Search BGG" })).toBeVisible();

    await page.getByRole("button", { name: "Clear all" }).click();
    await expect(page.getByRole("link", { name: "Search BGG" })).not.toBeVisible();
  });
});
