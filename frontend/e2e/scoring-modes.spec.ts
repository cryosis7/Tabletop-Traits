import { test, expect } from "@playwright/test";
import { syncCollection } from "./helpers";

test.describe("Scoring modes", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
    await syncCollection(page);
  });

  test("defaults to average scoring mode", async ({ page }) => {
    const averageBtn = page.getByRole("button", { name: "Average Rating" });
    const cumulativeBtn = page.getByRole("button", { name: "Cumulative" });

    await expect(averageBtn).toHaveAttribute("aria-pressed", "true");
    await expect(cumulativeBtn).toHaveAttribute("aria-pressed", "false");
  });

  test("switches scoring mode between average and cumulative", async ({ page }) => {
    const averageBtn = page.getByRole("button", { name: "Average Rating" });
    const cumulativeBtn = page.getByRole("button", { name: "Cumulative" });

    await cumulativeBtn.click();
    await expect(cumulativeBtn).toHaveAttribute("aria-pressed", "true");
    await expect(averageBtn).toHaveAttribute("aria-pressed", "false");

    await averageBtn.click();
    await expect(averageBtn).toHaveAttribute("aria-pressed", "true");
    await expect(cumulativeBtn).toHaveAttribute("aria-pressed", "false");
  });

  test("cumulative mode changes the chart axis values", async ({ page }) => {
    const chartSection = page.locator(".chart-section");

    // In average mode, capture the Y axis tick values
    await expect(chartSection.locator(".recharts-cartesian-axis-tick-value").first()).toBeVisible();
    const avgTicks = await chartSection
      .locator(".recharts-cartesian-axis-tick-value")
      .allTextContents();
    const avgMax = Math.max(...avgTicks.map(Number).filter((n) => !isNaN(n)));

    await page.getByRole("button", { name: "Cumulative" }).click();

    // Wait for chart to re-render with new values
    await expect(chartSection.locator(".recharts-cartesian-axis-tick-value").first()).toBeVisible();
    const cumTicks = await chartSection
      .locator(".recharts-cartesian-axis-tick-value")
      .allTextContents();
    const cumMax = Math.max(...cumTicks.map(Number).filter((n) => !isNaN(n)));

    // Cumulative totals should produce a higher max value than averages
    expect(cumMax).toBeGreaterThan(avgMax);
  });
});
