export interface MechanismScore {
  mechanismId: number;
  mechanismName: string;
  averageRating: number;
  totalRating: number;
  gameCount: number;
}

export interface BoardGame {
  id: number;
  name: string;
  yearPublished: number | null;
  thumbnailUrl: string | null;
  userRating: number;
  mechanisms: string[];
}

export interface SyncStatus {
  status: string;
  gamesProcessed: number;
  totalGames: number;
  lastSyncTime: string | null;
}

export type ScoringMode = "average" | "cumulative";

export type FilterMode = "any" | "all";
