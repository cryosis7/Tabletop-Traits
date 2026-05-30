import { test, expect } from "@playwright/test";
import AxeBuilder from "@axe-core/playwright";
import { syncCollection } from "./helpers";

test.describe("Accessibility", () => {
  test("landing page has no accessibility violations", async ({ page }) => {
    await page.goto("/");

    const results = await new AxeBuilder({ page }).analyze();

    expect(results.violations).toEqual([]);
  });

  test("dashboard after sync has no accessibility violations", async ({ page }) => {
    await page.goto("/");
    await syncCollection(page);

    const results = await new AxeBuilder({ page }).analyze();

    expect(results.violations).toEqual([]);
  });
});
