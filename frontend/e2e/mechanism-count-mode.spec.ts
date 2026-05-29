import { test, expect } from "@playwright/test";
import { syncCollection } from "./helpers";

test.describe("Mechanism count mode", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/");
    await syncCollection(page);
  });

  test("defaults to Top 20 selected", async ({ page }) => {
    const top20Btn = page.getByRole("button", { name: "Top 20" });
    const allBtn = page.getByRole("button", { name: "All", exact: true });
    const bottom20Btn = page.getByRole("button", { name: "Bottom 20" });

    await expect(top20Btn).toHaveAttribute("aria-pressed", "true");
    await expect(allBtn).toHaveAttribute("aria-pressed", "false");
    await expect(bottom20Btn).toHaveAttribute("aria-pressed", "false");
  });

  test("switches to All mode", async ({ page }) => {
    const allBtn = page.getByRole("button", { name: "All", exact: true });
    await allBtn.click();

    await expect(allBtn).toHaveAttribute("aria-pressed", "true");
    await expect(page.getByRole("button", { name: "Top 20" })).toHaveAttribute("aria-pressed", "false");
    await expect(page.getByRole("button", { name: "Bottom 20" })).toHaveAttribute("aria-pressed", "false");
  });

  test("switches to Bottom 20 mode", async ({ page }) => {
    const bottom20Btn = page.getByRole("button", { name: "Bottom 20" });
    await bottom20Btn.click();

    await expect(bottom20Btn).toHaveAttribute("aria-pressed", "true");
    await expect(page.getByRole("button", { name: "Top 20" })).toHaveAttribute("aria-pressed", "false");
    await expect(page.getByRole("button", { name: "All", exact: true })).toHaveAttribute("aria-pressed", "false");
  });

  test("count mode persists when switching chart tabs", async ({ page }) => {
    const allBtn = page.getByRole("button", { name: "All", exact: true });
    await allBtn.click();
    await expect(allBtn).toHaveAttribute("aria-pressed", "true");

    await page.getByRole("button", { name: "Radar" }).click();
    await expect(allBtn).toHaveAttribute("aria-pressed", "true");

    await page.getByRole("button", { name: "Scatter" }).click();
    await expect(allBtn).toHaveAttribute("aria-pressed", "true");

    await page.getByRole("button", { name: "Bar Chart" }).click();
    await expect(allBtn).toHaveAttribute("aria-pressed", "true");
  });

  test("count mode buttons remain visible across chart types", async ({ page }) => {
    const countToggle = page.locator(".count-toggle");
    await expect(countToggle).toBeVisible();

    await page.getByRole("button", { name: "Radar" }).click();
    await expect(countToggle).toBeVisible();

    await page.getByRole("button", { name: "Scatter" }).click();
    await expect(countToggle).toBeVisible();
  });
});
