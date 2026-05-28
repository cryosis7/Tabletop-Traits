# Board Game Rankings

## Overview

Full-stack application that syncs a user's board game collection from BoardGameGeek, caches it locally, and provides mechanism-based preference analysis with interactive charts.

**Data flow:** BGG XML API / HTML pages -> Backend sync -> In-memory cache -> REST API -> React frontend

## Architecture

| Component | Path | Port | Tech |
|-----------|------|------|------|
| Backend API | `backend/` | 5237 | ASP.NET Core (.NET 10, C#) |
| Frontend SPA | `frontend/` | 5173 | React 19, TypeScript, Vite |
| BGG Mock Server | (auto-started in dev) | 9090 | WireMock.Net |
| Seq (logging) | `backend/docker-compose.yml` | 5341 / 8081 | Docker |

The frontend calls the backend API directly (no proxy). CORS is configured for `http://localhost:5173`.

## Prerequisites

- .NET 10 SDK (`global.json` enforces 10.0.100)
- Node.js 20+ / npm
- Docker (optional, for Seq structured logging)

## Quick Start (full stack)

```powershell
# Optional: start Seq for structured log viewing
docker compose -f backend/docker-compose.yml up -d

# Start backend (mock BGG server auto-starts on port 9090 in Development mode)
cd backend
dotnet watch run --project src/BoardGameRankings.Api

# In a second terminal: start frontend
cd frontend
npm install
npm run dev
```

## Validation

Always verify changes with these commands before committing:

```powershell
# Backend: build + test
cd backend
dotnet build BoardGameRankings.slnx
dotnet test BoardGameRankings.slnx

# Frontend: type check + lint
cd frontend
npx tsc --noEmit
npm run lint

# End-to-end tests (starts both servers automatically)
cd frontend
npm run test:e2e
```

## API Endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| POST | `/api/sync/{username}` | Sync user's BGG collection |
| GET | `/api/collection/{username}` | Return cached rated games |
| GET | `/api/analysis/{username}/mechanisms?mode=average\|cumulative` | Mechanism preference scores |

## Fixture / Test Data

- `backend/data/games.json` - Cached board game data (shared across users)
- `backend/data/cryosis7/ratings.json` - Sample user ratings for the fixture user `cryosis7`
- `backend/src/BoardGameRankings.DevTools/Fixtures/` - HTML fixtures mocking BGG web pages (used by WireMock)
- Playwright e2e tests override `DataPath` to a temp directory to isolate test runs from dev data

## Nested AGENTS.md Files

- `backend/AGENTS.md` - Backend architecture, coding standards, and commands
- `frontend/AGENTS.md` - Frontend conventions and component patterns
- `frontend/e2e/AGENTS.md` - Playwright E2E testing practices
