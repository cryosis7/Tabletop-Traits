#!/usr/bin/env bash
set -e

ROOT="$(cd "$(dirname "$0")" && pwd)"
MODE="dev"
NO_SEQ=false

for arg in "$@"; do
  case "$arg" in
    --prod)   MODE="prod" ;;
    --dev)    MODE="dev" ;;
    --no-seq) NO_SEQ=true ;;
    -h|--help)
      echo "Usage: ./run.sh [options]"
      echo ""
      echo "Options:"
      echo "  --dev      Start in development mode (default)"
      echo "  --prod     Build and start in production mode"
      echo "  --no-seq   Skip starting Seq (dev mode only)"
      echo "  -h, --help Show this help message"
      exit 0
      ;;
  esac
done

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
  if [ "$NO_SEQ" = false ]; then
    echo "Starting dev services (Seq)..."
    (cd "$ROOT/backend" && docker compose up -d)
  fi

  echo "Starting backend..."
  (cd "$ROOT/backend" && dotnet run --project src/BoardGameRankings.Api) &
  BACKEND_PID=$!

  echo "Starting frontend..."
  (cd "$ROOT/frontend" && npm run dev) &
  FRONTEND_PID=$!

  echo ""
  echo "Backend:  http://localhost:5237"
  echo "Frontend: http://localhost:5173"
  [ "$NO_SEQ" = false ] && echo "Seq:      http://localhost:8081"
fi

echo ""
echo "Press Ctrl+C to stop all services."

trap "kill $BACKEND_PID $FRONTEND_PID 2>/dev/null; exit" INT TERM
wait $BACKEND_PID $FRONTEND_PID
