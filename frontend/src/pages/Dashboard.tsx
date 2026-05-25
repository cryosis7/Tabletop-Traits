import { useState } from "react";
import { useSync, useMechanismScores, useCollection } from "../hooks/useApi";
import { MechanismBarChart } from "../components/charts/MechanismBarChart";
import { MechanismRadarChart } from "../components/charts/MechanismRadarChart";
import { MechanismScatterChart } from "../components/charts/MechanismScatterChart";
import type { ScoringMode } from "../types";

export function Dashboard() {
  const [username, setUsername] = useState("");
  const [activeUser, setActiveUser] = useState("");
  const [mode, setMode] = useState<ScoringMode>("average");
  const [activeTab, setActiveTab] = useState<"bar" | "radar" | "scatter">("bar");

  const { sync, syncing, syncStatus, error: syncError } = useSync();
  const { scores, loading: scoresLoading, error: scoresError, fetchScores } = useMechanismScores();
  const { collection, loading: collectionLoading, fetchCollection } = useCollection();

  const handleSync = async () => {
    if (!username.trim()) return;
    await sync(username.trim());
    setActiveUser(username.trim());
    await fetchScores(username.trim(), mode);
    await fetchCollection(username.trim());
  };

  const handleModeChange = async (newMode: ScoringMode) => {
    setMode(newMode);
    if (activeUser) {
      await fetchScores(activeUser, newMode);
    }
  };

  return (
    <div className="dashboard">
      <header className="header">
        <h1>Board Game Mechanism Analyzer</h1>
        <p>Discover which board game mechanisms you love (and hate) based on your BGG ratings.</p>
      </header>

      <section className="sync-section">
        <div className="input-group">
          <input
            type="text"
            placeholder="Enter your BGG username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleSync()}
            disabled={syncing}
          />
          <button onClick={handleSync} disabled={syncing || !username.trim()}>
            {syncing ? "Syncing..." : "Sync & Analyze"}
          </button>
        </div>

        {syncError && <p className="error">{syncError}</p>}
        {syncStatus && (
          <p className="sync-info" role="status" aria-live="polite">
            Synced {syncStatus.gamesProcessed} games from BGG
            {syncStatus.lastSyncTime && ` at ${new Date(syncStatus.lastSyncTime).toLocaleTimeString()}`}
          </p>
        )}
      </section>

      {scores.length > 0 && (
        <>
          <section className="controls">
            <div className="mode-toggle">
              <label>Scoring Mode:</label>
              <button
                type="button"
                aria-pressed={mode === "average"}
                className={mode === "average" ? "active" : ""}
                onClick={() => handleModeChange("average")}
              >
                Average Rating
              </button>
              <button
                type="button"
                aria-pressed={mode === "cumulative"}
                className={mode === "cumulative" ? "active" : ""}
                onClick={() => handleModeChange("cumulative")}
              >
                Cumulative
              </button>
            </div>

            <div className="tab-toggle">
              <button
                type="button"
                aria-pressed={activeTab === "bar"}
                className={activeTab === "bar" ? "active" : ""}
                onClick={() => setActiveTab("bar")}
              >
                Bar Chart
              </button>
              <button
                type="button"
                aria-pressed={activeTab === "radar"}
                className={activeTab === "radar" ? "active" : ""}
                onClick={() => setActiveTab("radar")}
              >
                Radar
              </button>
              <button
                type="button"
                aria-pressed={activeTab === "scatter"}
                className={activeTab === "scatter" ? "active" : ""}
                onClick={() => setActiveTab("scatter")}
              >
                Scatter
              </button>
            </div>
          </section>

          <section className="chart-section" aria-label="Mechanism analysis">
            {scoresLoading && <p>Loading scores...</p>}
            {scoresError && <p className="error">{scoresError}</p>}

            {activeTab === "bar" && <MechanismBarChart scores={scores} mode={mode} />}
            {activeTab === "radar" && <MechanismRadarChart scores={scores} />}
            {activeTab === "scatter" && <MechanismScatterChart scores={scores} />}
          </section>

          <section className="collection-section">
            <h2>Your Rated Games ({collection.length})</h2>
            {collectionLoading && <p>Loading collection...</p>}
            {collection.length > 0 && (
              <table className="collection-table">
                <thead>
                  <tr>
                    <th>Game</th>
                    <th>Year</th>
                    <th>Your Rating</th>
                    <th>Mechanisms</th>
                  </tr>
                </thead>
                <tbody>
                  {collection.map((game) => (
                    <tr key={game.id}>
                      <td>{game.name}</td>
                      <td>{game.yearPublished ?? "-"}</td>
                      <td className="rating">{game.userRating}</td>
                      <td className="mechanisms">{game.mechanisms.join(", ")}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </section>
        </>
      )}
    </div>
  );
}
