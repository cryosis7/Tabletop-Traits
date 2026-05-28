import { useState, useCallback } from "react";
import type { MechanismScore, BoardGame, SyncStatus, ScoringMode } from "../types";
import * as api from "../services/api";

export function useSync() {
  const [syncing, setSyncing] = useState(false);
  const [syncStatus, setSyncStatus] = useState<SyncStatus | null>(null);
  const [error, setError] = useState<string | null>(null);

  const sync = useCallback(async (username: string) => {
    setSyncing(true);
    setError(null);
    try {
      const status = await api.syncCollection(username);
      setSyncStatus(status);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : "Sync failed";
      setError(message);
    } finally {
      setSyncing(false);
    }
  }, []);

  return { sync, syncing, syncStatus, error };
}

export function useMechanismScores() {
  const [scores, setScores] = useState<MechanismScore[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchScores = useCallback(async (username: string, mode: ScoringMode) => {
    setLoading(true);
    setError(null);
    try {
      const data = await api.getMechanismScores(username, mode);
      setScores(data);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : "Failed to load scores";
      setError(message);
    } finally {
      setLoading(false);
    }
  }, []);

  return { scores, loading, error, fetchScores };
}

export function useCollection() {
  const [collection, setCollection] = useState<BoardGame[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchCollection = useCallback(async (username: string) => {
    setLoading(true);
    setError(null);
    try {
      const data = await api.getCollection(username);
      setCollection(data);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : "Failed to load collection";
      setError(message);
    } finally {
      setLoading(false);
    }
  }, []);

  return { collection, loading, error, fetchCollection };
}

export function useMechanismDescriptions() {
  const [descriptions, setDescriptions] = useState<Map<string, string>>(new Map());
  const [loading, setLoading] = useState(false);

  const fetchDescriptions = useCallback(async () => {
    setLoading(true);
    try {
      const data = await api.getMechanismDescriptions();
      const map = new Map(data.map((d) => [d.name, d.description]));
      setDescriptions(map);
    } catch {
      // Non-critical - tooltips just won't show
    } finally {
      setLoading(false);
    }
  }, []);

  return { descriptions, loading, fetchDescriptions };
}
