# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: mechanism-filter.spec.ts >> Mechanism filter >> filters collection table when a mechanism is selected
- Location: e2e\mechanism-filter.spec.ts:10:3

# Error details

```
Error: locator.click: Error: strict mode violation: locator('.filter-dropdown').getByText('Worker Placement') resolved to 2 elements:
    1) <span>Worker Placement</span> aka getByRole('listitem').getByText('Worker Placement', { exact: true })
    2) <span class="mechanism-description">A stylized form of Action Drafting, players place…</span> aka getByText('A stylized form of Action')

Call log:
  - waiting for locator('.filter-dropdown').getByText('Worker Placement')

```

# Page snapshot

```yaml
- generic [ref=e1]:
  - generic [ref=e2]:
    - generic [ref=e3]:
      - banner [ref=e4]:
        - heading "Board Game Mechanism Analyzer" [level=1] [ref=e5]
        - paragraph [ref=e6]: Discover which board game mechanisms you love (and hate) based on your BGG ratings.
      - generic [ref=e7]:
        - generic [ref=e8]:
          - textbox "Enter your BGG username" [ref=e9]: testuser
          - button "Sync & Analyze" [ref=e10] [cursor=pointer]
        - status [ref=e11]: Synced 5 games from BGG at 5:22:37 PM
      - generic [ref=e12]:
        - button "Arithmetic Mean" [ref=e15] [cursor=pointer]:
          - generic [ref=e16]: Arithmetic Mean
          - generic [ref=e17]: ▼
        - generic [ref=e18]:
          - button "Bar Chart" [pressed] [ref=e19] [cursor=pointer]
          - button "Radar" [ref=e20] [cursor=pointer]
          - button "Scatter" [ref=e21] [cursor=pointer]
      - region "Mechanism analysis" [ref=e22]:
        - generic [ref=e24]:
          - button "Top 20" [pressed] [ref=e25] [cursor=pointer]
          - button "All" [ref=e26] [cursor=pointer]
          - button "Bottom 20" [ref=e27] [cursor=pointer]
        - application [ref=e31]:
          - generic [ref=e77]:
            - generic [ref=e78]:
              - generic [ref=e80]: "0"
              - generic [ref=e82]: "2"
              - generic [ref=e84]: "4"
              - generic [ref=e86]: "6"
              - generic [ref=e88]: "8"
            - generic [ref=e89]:
              - generic [ref=e91]: Card Play ConflictResolution
              - generic [ref=e93]: CooperativeGame
              - generic [ref=e95]: Real-Time
              - generic [ref=e97]: Worker Placement
              - generic [ref=e99]: End GameBonuses
              - generic [ref=e101]: Contracts
              - generic [ref=e103]: Deck, Bag, andPool Building
              - generic [ref=e105]: Variable PlayerPowers
              - generic [ref=e107]: Take That
              - generic [ref=e109]: HandManagement
      - generic [ref=e112]:
        - generic [ref=e113]:
          - textbox "Search mechanisms..." [active] [ref=e114]: Worker
          - list [ref=e115]:
            - listitem [ref=e116] [cursor=pointer]:
              - text: Worker Placement
              - generic [ref=e117]: "A stylized form of Action Drafting, players place tokens (typically the classic person-shaped \"meeple\") to trigger an action from a set of actions available to all players, generally one-at-a-time and in turn order. Some games achieve the same effect in reverse: the turn begins with action spaces filled by markers, which are claimed by players for some cost. Each player usually has a limited number of tokens with which to participate in the process, although these may increase as the game progresses. There is usually(*) a limit on the number of times a single action may be taken. Once that limit for an action is reached, it typically either becomes more expensive to take again or can no longer be taken for the remainder of the round. As such, not all actions can be taken by all players in a given round, and \"action blocking\" occurs. If the game is structured in rounds, then all actions are usually refreshed at the start or end of each round so that they become available again. From a thematic standpoint, the game pieces which players use to draft actions often represent \"workers\" of a given trade (this category of mechanism, however, is not necessarily limited to or by this thematic representation). In other words, players often thematically \"place workers\" to show which actions have been drafted by individual players. For example, in Agricola each player starts with two pieces representing family members that can be placed on action spaces to collect resources or take other actions like building fences. When someone places a piece on a given space, that action is no longer available until the next round. Keydom, which was published in 1998, is widely recognized as the first of the worker placement genre of games. Other early design experiments with the mechanism include Bus (1999) and Way Out West (2000). Well known examples of worker placement include Agricola (2007), Caylus (2005) and Stone Age (2008). If there are several types of worker, use Worker Placement, Different Worker Types instead. (*) The use of the word \"usually\" in this context is a somewhat controversial point of discussion. For purposes of BGG classification, \"action blocking\" is a defining element of worker placement. In that case, there must always be a limit on the number of times a single action may be drafted each round."
        - generic [ref=e118]:
          - button "ANY" [ref=e119] [cursor=pointer]
          - button "ALL" [ref=e120] [cursor=pointer]
      - generic [ref=e121]:
        - heading "Your Rated Games (5)" [level=2] [ref=e122]
        - table [ref=e123]:
          - rowgroup [ref=e124]:
            - row "Game Year Your Rating Mechanisms" [ref=e125]:
              - columnheader "Game" [ref=e126]
              - columnheader "Year" [ref=e127]
              - columnheader "Your Rating" [ref=e128]
              - columnheader "Mechanisms" [ref=e129]
          - rowgroup [ref=e130]:
            - 'row "Dune: Imperium 2020 9 Deck, Bag, and Pool Building, Worker Placement, End Game Bonuses" [ref=e131]':
              - 'cell "Dune: Imperium" [ref=e132]'
              - cell "2020" [ref=e133]
              - cell "9" [ref=e134]
              - cell "Deck, Bag, and Pool Building, Worker Placement, End Game Bonuses" [ref=e135]:
                - generic [ref=e136]: Deck, Bag, and Pool Building
                - generic [ref=e137]: ", Worker Placement"
                - generic [ref=e138]: ", End Game Bonuses"
            - row "5-Minute Dungeon 2017 8 Card Play Conflict Resolution, Cooperative Game, Hand Management, Real-Time" [ref=e139]:
              - cell "5-Minute Dungeon" [ref=e140]
              - cell "2017" [ref=e141]
              - cell "8" [ref=e142]
              - cell "Card Play Conflict Resolution, Cooperative Game, Hand Management, Real-Time" [ref=e143]:
                - generic [ref=e144]: Card Play Conflict Resolution
                - generic [ref=e145]: ", Cooperative Game"
                - generic [ref=e146]: ", Hand Management"
                - generic [ref=e147]: ", Real-Time"
            - row "Architects of the West Kingdom 2018 7 Worker Placement, Contracts, End Game Bonuses" [ref=e148]:
              - cell "Architects of the West Kingdom" [ref=e149]
              - cell "2018" [ref=e150]
              - cell "7" [ref=e151]
              - cell "Worker Placement, Contracts, End Game Bonuses" [ref=e152]:
                - generic [ref=e153]: Worker Placement
                - generic [ref=e154]: ", Contracts"
                - generic [ref=e155]: ", End Game Bonuses"
            - row "Citadels 2000 7 Hand Management, Variable Player Powers, Take That" [ref=e156]:
              - cell "Citadels" [ref=e157]
              - cell "2000" [ref=e158]
              - cell "7" [ref=e159]
              - cell "Hand Management, Variable Player Powers, Take That" [ref=e160]:
                - generic [ref=e161]: Hand Management
                - generic [ref=e162]: ", Variable Player Powers"
                - generic [ref=e163]: ", Take That"
            - row "Dominion 2008 5 Deck, Bag, and Pool Building, Hand Management" [ref=e164]:
              - cell "Dominion" [ref=e165]
              - cell "2008" [ref=e166]
              - cell "5" [ref=e167]
              - cell "Deck, Bag, and Pool Building, Hand Management" [ref=e168]:
                - generic [ref=e169]: Deck, Bag, and Pool Building
                - generic [ref=e170]: ", Hand Management"
    - contentinfo [ref=e171]:
      - link "Powered by BoardGameGeek" [ref=e172] [cursor=pointer]:
        - /url: https://boardgamegeek.com
        - img "Powered by BoardGameGeek" [ref=e173]
  - generic [ref=e174]: Card Play Conflict Resolution
```

# Test source

```ts
  1   | import { test, expect } from "@playwright/test";
  2   | import { syncCollection, fixtureGameCount } from "./helpers";
  3   | 
  4   | test.describe("Mechanism filter", () => {
  5   |   test.beforeEach(async ({ page }) => {
  6   |     await page.goto("/");
  7   |     await syncCollection(page);
  8   |   });
  9   | 
  10  |   test("filters collection table when a mechanism is selected", async ({ page }) => {
  11  |     const searchInput = page.getByPlaceholder("Search mechanisms...");
  12  |     await searchInput.fill("Worker");
  13  | 
  14  |     const dropdown = page.locator(".filter-dropdown");
  15  |     await expect(dropdown).toBeVisible();
> 16  |     await dropdown.getByText("Worker Placement").click();
      |                                                  ^ Error: locator.click: Error: strict mode violation: locator('.filter-dropdown').getByText('Worker Placement') resolved to 2 elements:
  17  | 
  18  |     // Table should show filtered count
  19  |     await expect(
  20  |       page.getByRole("heading", {
  21  |         level: 2,
  22  |         name: /Showing \d+ of \d+/,
  23  |       })
  24  |     ).toBeVisible();
  25  | 
  26  |     // All visible rows should have Worker Placement
  27  |     const rows = page.getByRole("table").locator("tbody tr");
  28  |     const rowCount = await rows.count();
  29  |     expect(rowCount).toBeGreaterThan(0);
  30  |     expect(rowCount).toBeLessThan(fixtureGameCount);
  31  | 
  32  |     for (let i = 0; i < rowCount; i++) {
  33  |       await expect(rows.nth(i)).toContainText("Worker Placement");
  34  |     }
  35  |   });
  36  | 
  37  |   test("ANY mode shows games with any selected mechanism", async ({ page }) => {
  38  |     const searchInput = page.getByPlaceholder("Search mechanisms...");
  39  | 
  40  |     // Select Worker Placement
  41  |     await searchInput.fill("Worker Placement");
  42  |     await page.locator(".filter-dropdown").getByText("Worker Placement", { exact: true }).click();
  43  | 
  44  |     const heading = page.getByRole("heading", { level: 2 });
  45  |     const afterFirst = await heading.textContent();
  46  |     const matchFirst = afterFirst!.match(/Showing (\d+) of (\d+)/);
  47  |     expect(matchFirst).not.toBeNull();
  48  |     const countWithOne = parseInt(matchFirst![1]);
  49  | 
  50  |     // Select Deck, Bag, and Pool Building (overlaps with Dune: Imperium but adds Dominion)
  51  |     await searchInput.fill("Deck, Bag");
  52  |     await page.locator(".filter-dropdown").getByText("Deck, Bag, and Pool Building", { exact: true }).click();
  53  | 
  54  |     // ANY mode (default) - should show more games than just one mechanism
  55  |     const afterSecond = await heading.textContent();
  56  |     const matchSecond = afterSecond!.match(/Showing (\d+) of (\d+)/);
  57  |     expect(matchSecond).not.toBeNull();
  58  |     const countWithTwo = parseInt(matchSecond![1]);
  59  |     expect(countWithTwo).toBeGreaterThanOrEqual(countWithOne);
  60  |   });
  61  | 
  62  |   test("ALL mode shows only games with all selected mechanisms", async ({ page }) => {
  63  |     const searchInput = page.getByPlaceholder("Search mechanisms...");
  64  | 
  65  |     // Select Hand Management (appears in many games)
  66  |     await searchInput.fill("Hand Management");
  67  |     await page.locator(".filter-dropdown").getByText("Hand Management", { exact: true }).click();
  68  | 
  69  |     // Select Cooperative Game (appears in fewer)
  70  |     await searchInput.fill("Cooperative Game");
  71  |     await page.locator(".filter-dropdown").getByText("Cooperative Game", { exact: true }).click();
  72  | 
  73  |     // Switch to ALL mode
  74  |     await page.getByRole("button", { name: "ALL", exact: true }).click();
  75  | 
  76  |     // Should show only games with BOTH mechanisms
  77  |     const rows = page.getByRole("table").locator("tbody tr");
  78  |     const rowCount = await rows.count();
  79  |     expect(rowCount).toBeGreaterThan(0);
  80  | 
  81  |     for (let i = 0; i < rowCount; i++) {
  82  |       await expect(rows.nth(i)).toContainText("Hand Management");
  83  |       await expect(rows.nth(i)).toContainText("Cooperative Game");
  84  |     }
  85  |   });
  86  | 
  87  |   test("removes a mechanism chip and restores filtered games", async ({ page }) => {
  88  |     const searchInput = page.getByPlaceholder("Search mechanisms...");
  89  |     await searchInput.fill("Worker");
  90  |     await page.locator(".filter-dropdown").getByText("Worker Placement").click();
  91  | 
  92  |     // Verify filter is active
  93  |     await expect(page.getByRole("heading", { level: 2 })).toContainText("Showing");
  94  | 
  95  |     // Click the remove button on the chip
  96  |     const chip = page.locator(".chip", { hasText: "Worker Placement" });
  97  |     await chip.getByRole("button", { name: "\u00d7" }).click();
  98  | 
  99  |     // Table should show all games again
  100 |     await expect(
  101 |       page.getByRole("heading", { level: 2, name: `Your Rated Games (${fixtureGameCount})` })
  102 |     ).toBeVisible();
  103 |   });
  104 | 
  105 |   test("clear all button removes all filters", async ({ page }) => {
  106 |     const searchInput = page.getByPlaceholder("Search mechanisms...");
  107 | 
  108 |     await searchInput.fill("Worker Placement");
  109 |     await page.locator(".filter-dropdown").getByText("Worker Placement", { exact: true }).click();
  110 | 
  111 |     // After selecting first mechanism, add a second one
  112 |     await searchInput.click();
  113 |     await searchInput.pressSequentially("Take That");
  114 |     const dropdown = page.locator(".filter-dropdown");
  115 |     await expect(dropdown).toBeVisible();
  116 |     await dropdown.getByText("Take That", { exact: true }).click();
```