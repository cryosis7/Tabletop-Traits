# React Playwright E2E Best Practices

This guide covers how to write and maintain end-to-end tests for the React frontend in this folder.

## Scope

- Write tests against real user behavior in the browser.
- Exercise the app through the UI, not through React internals.
- Keep tests deterministic against the local backend and fixture data configured in Playwright.

## Core Principles

### Test behavior, not implementation

- Assert what a user can see, click, type, or navigate to.
- Do not reach into component state, hooks, or React instance details.
- Prefer verifying headings, button states, error messages, table rows, and visible chart sections.

### Prefer stable selectors

- Use `getByRole`, `getByLabel`, `getByPlaceholder`, and `getByText` before CSS selectors.
- Treat CSS class selectors as a last resort unless the class is intentionally owned as a test contract.
- If a flow cannot be targeted accessibly, add a stable `data-testid` in the app rather than depending on styling structure.

### Keep tests isolated

- Each test should be able to run independently and in any order.
- Start from a clean page state with `page.goto("/")`.
- Do not rely on state created by earlier tests.

### Use deterministic data

- The fixture user is `cryosis7`. Test expectations should align with this user's data.
- Backend fixture data lives in `backend/data/games.json` (game catalog) and `backend/data/cryosis7/ratings.json` (user ratings).
- HTML mocks of BGG pages are embedded in `backend/src/BoardGameRankings.DevTools/Fixtures/`.
- Playwright config overrides `DataPath` to a temp directory, isolating e2e runs from local dev data.
- When fixture data changes, update test expectations in the same change.

## React-Specific Guidance

### Assert rendered outcomes

- Verify the UI produced by React after state updates settle.
- Use Playwright's web-first assertions such as `await expect(locator).toBeVisible()` and `await expect(locator).toContainText(...)`.
- Avoid manual waits, arbitrary delays, or polling loops.

### Treat charts as user-visible widgets

- Recharts output can be brittle at the SVG node level.
- Prefer asserting that the selected chart tab is active, the chart container is visible, and related labels or summaries are present.
- Only assert low-level SVG details when the user-facing behavior truly depends on them.

### Cover async transitions clearly

- Check loading, disabled, success, and error states around async actions.
- For the sync flow, verify button enablement, the sync summary, and the resulting table or chart state.
- Let Playwright wait on expected UI state instead of waiting on network timing.

## Preferred Test Structure

### Naming

- Use descriptive test names that read like user outcomes.
- Good examples:
  - `syncs a user collection and displays mechanism analysis`
  - `shows an error when sync fails`
  - `switches scoring mode between average and cumulative`

### Setup

- Keep setup close to the test unless it is repeated enough to justify extraction.
- If multiple specs repeat the same sync flow, extract a helper such as `syncCollection(page, username)` in this folder.
- Helpers should model user actions and visible waits, not hidden implementation details.

### Assertions

- Prefer a small number of meaningful assertions over many fragile ones.
- Assert the outcome that proves the behavior, then stop.
- When checking lists or tables, validate a few representative rows or counts instead of every rendered cell unless the full output is the feature.

## What To Avoid

- Do not use `waitForTimeout` except as a temporary debugging aid.
- Do not over-assert exact markup for React-rendered structure.
- Do not couple tests to animation timing, chart internals, or CSS layout.
- Do not depend on external network access.
- Do not duplicate the same setup sequence across many tests once a helper would make the suite clearer.

## Coverage Priorities For This App

- Happy path sync and analysis rendering.
- Validation for empty or invalid usernames.
- Error handling when sync or analysis fails.
- Switching chart tabs and scoring modes.
- Collection table rendering for known fixture games and mechanisms.
- Any regression-prone flow that depends on backend fixture data.

## Local Commands

```powershell
# Run all e2e tests (preferred - starts backend + frontend automatically)
npm run test:e2e

# Run a single spec file
npx playwright test e2e/dashboard.spec.ts

# Run headed for debugging
npx playwright test --headed
```

## Verification

Always run `npm run test:e2e` from the `frontend/` directory after modifying any frontend component, page, hook, or backend controller. The Playwright config automatically starts both the .NET backend (port 5237, Development mode with mock BGG server) and the Vite dev server (port 5173). All tests must pass before committing.

## Maintenance Notes

- Keep the Playwright config, backend fixture data, and spec expectations in sync.
- When a selector feels brittle, fix the app markup or add a test id instead of layering on more waits.
- Prefer small focused specs over one long scenario that tries to prove everything.
