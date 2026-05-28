# Board Game Rankings - Mechanism Preference Analyzer

Discover which board game mechanisms you love (and hate) based on your BoardGameGeek ratings.

This application fetches your rated games from BGG, extracts every mechanism tagged on those games, and scores each mechanism by your ratings. Interactive charts help you visualize your preferences at a glance.

## Quick Start

### Script (recommended)

```bash
# Development (starts Seq, backend, and frontend dev server)
./run.sh

# Production (builds frontend, serves via preview + Production backend profile)
./run.sh prod
```

| Service | Dev | Prod |
|---------|-----|------|
| Backend | http://localhost:5237 | http://localhost:5237 |
| Frontend | http://localhost:5173 | http://localhost:4173 |
| Seq logs | http://localhost:8081 | - |

Press `Ctrl+C` to stop all services.

### Manual

**Backend**

```bash
cd backend
docker compose up -d          # optional: Seq log viewer
dotnet run --project src/BoardGameRankings.Api
# or for production:
dotnet run --project src/BoardGameRankings.Api --launch-profile Production
```

**Frontend**

```bash
cd frontend
npm install

npm run dev       # development server at http://localhost:5173
# or for production:
npm run build && npm run preview   # preview at http://localhost:4173
```

## How It Works

1. Enter your BGG username and click "Sync & Analyze"
2. The backend fetches your rated collection via the BGG XML API2
3. For each game, it retrieves the tagged mechanisms (Hand Management, Worker Placement, etc.)
4. Mechanism scores are calculated using your ratings
5. Interactive charts display your mechanism preferences

## Architecture

```
board-game-rankings/
├── backend/              .NET 9 Web API (DDD)
│   ├── src/
│   │   ├── Domain/       Entities, interfaces, value objects
│   │   ├── Application/  Services, DTOs, use cases
│   │   ├── Infrastructure/  BGG API client, JSON persistence
│   │   └── Api/          Controllers, DI, middleware
│   └── tests/
├── frontend/             React + TypeScript + Vite
│   └── src/
│       ├── components/charts/  Recharts visualizations
│       ├── hooks/              Data fetching hooks
│       ├── pages/              Dashboard page
│       ├── services/           Axios API client
│       └── types/              TypeScript interfaces
└── README.md
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/sync/{username}` | Sync user's rated games from BGG |
| GET | `/api/analysis/{username}/mechanisms?mode=average\|cumulative` | Get mechanism scores |
| GET | `/api/collection/{username}` | Get rated games with mechanisms |

When the backend is running in development, Swagger UI is available at `http://localhost:5237/swagger` and the raw OpenAPI document is available at `http://localhost:5237/openapi/v1.json`.

## Charts

- **Bar Chart** - Mechanisms ranked by score (top 20), color-coded by rating
- **Radar Chart** - Spider chart of your top 10 mechanisms
- **Scatter Chart** - Frequency vs. average rating (identify well-liked vs. frequently-encountered mechanisms)

## Scoring Modes

- **Average**: Mean of your ratings across all games with that mechanism. Shows pure preference.
- **Cumulative**: Sum of ratings. Rewards mechanisms you encounter frequently in highly-rated games.

## Tech Stack

- **Backend**: .NET 9, ASP.NET Core, DDD, JSON file storage
- **Frontend**: React 19, TypeScript, Vite, Recharts, Axios
- **Data Source**: BoardGameGeek XML API2
- **Storage**: Local JSON files (no database required)
