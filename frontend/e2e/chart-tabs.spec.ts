import { test, expect } from "@playwright/test";
import { syncCollection } from "./helpers";

test.describe("Chart tabs", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
    await syncCollection(page);
  });

  test("defaults to bar chart tab", async ({ page }) => {
    const barBtn = page.getByRole("button", { name: "Bar Chart" });
    const radarBtn = page.getByRole("button", { name: "Radar" });
    const scatterBtn = page.getByRole("button", { name: "Scatter" });

    await expect(barBtn).toHaveAttribute("aria-pressed", "true");
    await expect(radarBtn).toHaveAttribute("aria-pressed", "false");
    await expect(scatterBtn).toHaveAttribute("aria-pressed", "false");
  });

  test("switches between chart tabs", async ({ page }) => {
    const barBtn = page.getByRole("button", { name: "Bar Chart" });
    const radarBtn = page.getByRole("button", { name: "Radar" });
    const scatterBtn = page.getByRole("button", { name: "Scatter" });

    await radarBtn.click();
    await expect(radarBtn).toHaveAttribute("aria-pressed", "true");
    await expect(barBtn).toHaveAttribute("aria-pressed", "false");
    await expect(scatterBtn).toHaveAttribute("aria-pressed", "false");

    await scatterBtn.click();
    await expect(scatterBtn).toHaveAttribute("aria-pressed", "true");
    await expect(radarBtn).toHaveAttribute("aria-pressed", "false");
    await expect(barBtn).toHaveAttribute("aria-pressed", "false");

    await barBtn.click();
    await expect(barBtn).toHaveAttribute("aria-pressed", "true");
    await expect(scatterBtn).toHaveAttribute("aria-pressed", "false");
  });

  test("bar chart renders chart elements", async ({ page }) => {
    const chartSection = page.locator(".chart-section");
    await expect(chartSection.locator(".recharts-bar-rectangle").first()).toBeVisible();
  });

  test("radar chart renders chart elements", async ({ page }) => {
    await page.getByRole("button", { name: "Radar" }).click();
    const chartSection = page.locator(".chart-section");
    await expect(chartSection.locator(".recharts-radar")).toBeVisible();
  });

  test("scatter chart renders chart elements", async ({ page }) => {
    await page.getByRole("button", { name: "Scatter" }).click();
    const chartSection = page.locator(".chart-section");
    await expect(chartSection.locator(".recharts-symbols").first()).toBeVisible();
  });
});
