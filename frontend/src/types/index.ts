export interface MechanismScore {
  mechanismId: number;
  mechanismName: string;
  arithmeticMean: number;
  bayesianAverage: number;
  median: number;
  trimmedMean: number;
  confidenceAdjusted: number;
  positiveRate: number;
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

export type ScoringMode =
  | "arithmetic"
  | "bayesian"
  | "median"
  | "trimmed"
  | "confidence"
  | "positiveRate";

export interface ScoringModeConfig {
  key: ScoringMode;
  label: string;
  description: string;
  scoreKey: keyof MechanismScore;
}

export const SCORING_MODES: ScoringModeConfig[] = [
  {
    key: "bayesian",
    label: "Bayesian Average",
    description:
      "Accounts for sample size - mechanisms with fewer games are pulled closer to your overall average.",
    scoreKey: "bayesianAverage",
  },
  {
    key: "trimmed",
    label: "Trimmed Mean",
    description:
      "Average after dropping your highest and lowest 10% of ratings.",
    scoreKey: "trimmedMean",
  },
  {
    key: "median",
    label: "Median",
    description:
      "The middle rating when sorted. Ignores unusually high or low scores.",
    scoreKey: "median",
  },
  {
    key: "confidence",
    label: "Confidence-Adjusted",
    description:
      "Favors mechanisms where your ratings are consistently high across many games.",
    scoreKey: "confidenceAdjusted",
  },
  {
    key: "positiveRate",
    label: "Positive Rate",
    description:
      "Percentage of games you rated 7 or higher ('Good game' on BGG).",
    scoreKey: "positiveRate",
  },
  {
    key: "arithmetic",
    label: "Arithmetic Mean",
    description: "Simple average of all your ratings for games with this mechanism.",
    scoreKey: "arithmeticMean",
  },
];

export type FilterMode = "any" | "all";

export type MechanismCountMode = "top20" | "all" | "bottom20";

export interface MechanismDescription {
  id: number;
  name: string;
  description: string;
}
