import { useState } from "react";
import { useTranslation } from "react-i18next";
import type { SyncStatus } from "../types";

interface Props {
  onSync: (username: string) => void;
  syncing: boolean;
  syncStatus: SyncStatus | null;
  syncError: string | null;
  loading: boolean;
}

export function SyncPanel({ onSync, syncing, syncStatus, syncError, loading }: Props): React.ReactElement {
  const { t } = useTranslation();
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
          placeholder={t("sync.placeholder")}
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && handleSync()}
          disabled={syncing}
          autoComplete="username"
        />
        <button onClick={handleSync} disabled={syncing || !username.trim()}>
          {syncing ? t("sync.syncing") : t("sync.button")}
        </button>
      </div>

      {syncError && <p className="error">{syncError}</p>}
      {syncStatus && !syncing && !loading && (
        <p className="sync-info" role="status" aria-live="polite">
          {t("sync.syncedGames", { count: syncStatus.gamesProcessed })}
          {syncStatus.lastSyncTime && t("sync.syncedAt", { time: new Date(syncStatus.lastSyncTime).toLocaleTimeString() })}
        </p>
      )}
    </section>
  );
}
