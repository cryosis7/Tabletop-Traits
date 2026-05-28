import { test, expect, type Page } from "@playwright/test";

const fixtureUsername = "testuser";

async function syncCollection(page: Page): Promise<void> {
  await page.getByPlaceholder("Enter your BGG username").fill(fixtureUsername);
  await page.getByRole("button", { name: "Sync & Analyze" }).click();

  await expect(page.getByRole("status")).toContainText(/Synced \d+ games from BGG/);
  await expect(page.getByRole("table")).toBeVisible();
}

test.describe("Mechanism Tooltips", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
    await syncCollection(page);
  });

  test.describe("Collection table", () => {
    test("mechanism names in the table show a tooltip with the description on hover", async ({
      page,
    }) => {
      const table = page.getByRole("table");
      // "Cooperative Game" appears in 5-Minute Dungeon's mechanisms
      const mechanismSpan = table.getByRole("cell", { name: /Cooperative Game/ }).first();
      await expect(mechanismSpan).toBeVisible();

      // Find the specific mechanism text element and hover
      const cooperativeGameText = mechanismSpan.getByText("Cooperative Game");
      await cooperativeGameText.hover();

      // Tooltip should appear with the description
      const tooltip = page.getByRole("tooltip");
      await expect(tooltip).toBeVisible();
      await expect(tooltip).toContainText(
        "Players coordinate their actions to achieve a common win condition"
      );
    });

    test("tooltip disappears when mouse leaves the mechanism", async ({ page }) => {
      const table = page.getByRole("table");
      const mechanismSpan = table.getByRole("cell", { name: /Cooperative Game/ }).first();
      const cooperativeGameText = mechanismSpan.getByText("Cooperative Game");

      await cooperativeGameText.hover();
      await expect(page.getByRole("tooltip")).toBeVisible();

      // Move mouse away
      await page.getByRole("heading", { level: 2 }).hover();
      await expect(page.getByRole("tooltip")).not.toBeVisible();
    });
  });

  test.describe("Mechanism filter", () => {
    test("filter dropdown items show mechanism descriptions", async ({ page }) => {
      const searchInput = page.getByPlaceholder("Search mechanisms...");
      await searchInput.fill("Cooperative");

      // Dropdown should show the mechanism with its description
      const dropdown = page.locator(".filter-dropdown");
      await expect(dropdown).toBeVisible();

      const item = dropdown.getByText("Cooperative Game");
      await expect(item).toBeVisible();
      await expect(dropdown).toContainText(
        "Players coordinate their actions to achieve a common win condition"
      );
    });

    test("selected mechanism chips show tooltip on hover", async ({ page }) => {
      // Select a mechanism via the filter
      const searchInput = page.getByPlaceholder("Search mechanisms...");
      await searchInput.fill("Cooperative");
      await page.locator(".filter-dropdown").getByText("Cooperative Game").click();

      // Chip should be visible
      const chip = page.locator(".chip", { hasText: "Cooperative Game" });
      await expect(chip).toBeVisible();

      // Hover the chip to see tooltip
      await chip.hover();
      const tooltip = page.getByRole("tooltip");
      await expect(tooltip).toBeVisible();
      await expect(tooltip).toContainText(
        "Players coordinate their actions to achieve a common win condition"
      );
    });
  });

  test.describe("Chart tooltips", () => {
    test("bar chart tooltip includes mechanism description when hovering a bar", async ({
      page,
    }) => {
      // The bar chart should be visible by default
      const chartSection = page.locator(".chart-section");
      await expect(chartSection).toBeVisible();

      // Find a bar element in the chart and hover over it to trigger the Recharts tooltip
      const bars = chartSection.locator(".recharts-bar-rectangle");
      await expect(bars.first()).toBeVisible();
      await bars.first().hover();

      // The Recharts tooltip should show with a description
      const rechartsTooltip = chartSection.locator(".recharts-tooltip-wrapper");
      await expect(rechartsTooltip).toBeVisible();
      // The custom tooltip content includes a mechanism description (non-empty paragraph)
      await expect(rechartsTooltip.locator("p").first()).not.toBeEmpty();
    });
  });
});
