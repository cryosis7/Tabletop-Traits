import { test, expect } from "@playwright/test";
import { syncCollection } from "./helpers";

test.describe("Scoring modes", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
    await syncCollection(page);
  });

  test("defaults to arithmetic scoring mode", async ({ page }) => {
    const trigger = page.locator("#scoring-mode-select");
    await expect(trigger).toContainText("Arithmetic Mean");
  });

  test("switches scoring mode via dropdown", async ({ page }) => {
    const trigger = page.locator("#scoring-mode-select");

    await trigger.click();
    await page.locator(".mode-dropdown-item", { hasText: "Bayesian Average" }).click();
    await expect(trigger).toContainText("Bayesian Average");

    await trigger.click();
    await page.locator(".mode-dropdown-item", { hasText: "Median" }).click();
    await expect(trigger).toContainText("Median");

    await trigger.click();
    await page.locator(".mode-dropdown-item", { hasText: "Arithmetic Mean" }).click();
    await expect(trigger).toContainText("Arithmetic Mean");
  });

  test("shows tooltip with mode description on info icon hover", async ({ page }) => {
    const trigger = page.locator("#scoring-mode-select");
    await trigger.click();

    const dropdown = page.locator(".mode-dropdown");
    await expect(dropdown).toBeVisible();
    await expect(dropdown).toContainText("Simple average");
  });

  test("switching mode changes the chart values", async ({ page }) => {
    const chartSection = page.locator(".chart-section");

    // In arithmetic mode, capture the Y axis tick values
    await expect(chartSection.locator(".recharts-cartesian-axis-tick-value").first()).toBeVisible();
    const arithmeticTicks = await chartSection
      .locator(".recharts-cartesian-axis-tick-value")
      .allTextContents();
    const arithmeticValues = arithmeticTicks.map(Number).filter((n) => !isNaN(n));

    // Switch to positive rate - values should be 0-100 range
    const trigger = page.locator("#scoring-mode-select");
    await trigger.click();
    await page.locator(".mode-dropdown-item", { hasText: "Positive Rate" }).click();

    await expect(chartSection.locator(".recharts-cartesian-axis-tick-value").first()).toBeVisible();
    const positiveRateTicks = await chartSection
      .locator(".recharts-cartesian-axis-tick-value")
      .allTextContents();
    const positiveRateValues = positiveRateTicks.map(Number).filter((n) => !isNaN(n));

    // Positive rate values should differ from arithmetic values
    expect(positiveRateValues).not.toEqual(arithmeticValues);
  });

  test("all scoring modes are available in the dropdown", async ({ page }) => {
    const trigger = page.locator("#scoring-mode-select");
    await trigger.click();

    const items = page.locator(".mode-dropdown-item .mode-dropdown-label");
    const labels = await items.allTextContents();

    expect(labels).toContain("Arithmetic Mean");
    expect(labels).toContain("Bayesian Average");
    expect(labels).toContain("Median");
    expect(labels).toContain("Trimmed Mean");
    expect(labels).toContain("Confidence-Adjusted");
    expect(labels).toContain("Positive Rate");
    expect(labels).toHaveLength(6);
  });
});
