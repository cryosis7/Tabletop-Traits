#!/usr/bin/env bash
set -e

ROOT="$(cd "$(dirname "$0")" && pwd)"
MODE="${1:-dev}"

if [ "$MODE" = "prod" ]; then
  echo "Building frontend..."
  (cd "$ROOT/frontend" && npm run build)

  echo "Starting backend (Production)..."
  (cd "$ROOT/backend" && dotnet run --project src/BoardGameRankings.Api --launch-profile Production) &
  BACKEND_PID=$!

  echo "Starting frontend preview..."
  (cd "$ROOT/frontend" && npm run preview) &
  FRONTEND_PID=$!

  echo ""
  echo "Backend:  http://localhost:5237"
  echo "Frontend: http://localhost:4173"
else
  echo "Starting dev services (Seq)..."
  (cd "$ROOT/backend" && docker compose up -d)

  echo "Starting backend..."
  (cd "$ROOT/backend" && dotnet run --project src/BoardGameRankings.Api) &
  BACKEND_PID=$!

  echo "Starting frontend..."
  (cd "$ROOT/frontend" && npm run dev) &
  FRONTEND_PID=$!

  echo ""
  echo "Backend:  http://localhost:5237"
  echo "Frontend: http://localhost:5173"
  echo "Seq:      http://localhost:8081"
fi

echo ""
echo "Press Ctrl+C to stop all services."

trap "kill $BACKEND_PID $FRONTEND_PID 2>/dev/null; exit" INT TERM
wait $BACKEND_PID $FRONTEND_PID
