import { useState } from "react";
import type { SyncStatus } from "../types";

interface Props {
  onSync: (username: string) => void;
  syncing: boolean;
  syncStatus: SyncStatus | null;
  syncError: string | null;
  loading: boolean;
}

export function SyncPanel({ onSync, syncing, syncStatus, syncError, loading }: Props): React.ReactElement {
  const [username, setUsername] = useState("");

  const handleSync = () => {
    if (!username.trim()) return;
    onSync(username.trim());
  };

  return (
    <section className="sync-section">
      <div className="input-group">
        <input
          type="text"
          placeholder="Enter your BGG username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && handleSync()}
          disabled={syncing}
          autoComplete="username"
        />
        <button onClick={handleSync} disabled={syncing || !username.trim()}>
          {syncing ? "Syncing..." : "Sync & Analyze"}
        </button>
      </div>

      {syncError && <p className="error">{syncError}</p>}
      {syncStatus && !syncing && !loading && (
        <p className="sync-info" role="status" aria-live="polite">
          Synced {syncStatus.gamesProcessed} games from BGG
          {syncStatus.lastSyncTime && ` at ${new Date(syncStatus.lastSyncTime).toLocaleTimeString()}`}
        </p>
      )}
    </section>
  );
}
