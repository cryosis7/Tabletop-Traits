import { expect, type Page } from "@playwright/test";

export const fixtureUsername = "testuser";
export const fixtureGameCount = 5;
export const fixtureGames = ["5-Minute Dungeon", "Dune: Imperium", "Dominion"];

export const paginatedFixtureUsername = "Brezman";
export const paginatedFixtureGameCount = 370;
export const paginatedFixtureGames = ["1775: Rebellion", "Star Wars: Outer Rim", "Wingspan"];

export async function syncCollection(
  page: Page,
  username: string = fixtureUsername,
  expectedGameCount: number = fixtureGameCount
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
