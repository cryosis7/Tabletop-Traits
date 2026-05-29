import axios from "axios";
import type { MechanismScore, BoardGame, SyncStatus, ScoringMode, MechanismDescription } from "../types";

const api = axios.create({
  baseURL: "http://localhost:5237/api",
});

export async function syncCollection(username: string): Promise<SyncStatus> {
  const response = await api.post<SyncStatus>(`/sync/${encodeURIComponent(username)}`);
  return response.data;
}

export async function getMechanismScores(
  username: string,
  mode: ScoringMode = "arithmetic"
): Promise<MechanismScore[]> {
  const response = await api.get<MechanismScore[]>(
    `/analysis/${encodeURIComponent(username)}/mechanisms`,
    { params: { mode } }
  );
  return response.data;
}

export async function getCollection(username: string): Promise<BoardGame[]> {
  const response = await api.get<BoardGame[]>(`/collection/${encodeURIComponent(username)}`);
  return response.data;
}

export async function getMechanismDescriptions(): Promise<MechanismDescription[]> {
  const response = await api.get<MechanismDescription[]>("/mechanism");
  return response.data;
}
