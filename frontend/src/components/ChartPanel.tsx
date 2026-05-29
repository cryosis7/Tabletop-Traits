import { useState, useMemo } from "react";
import { MechanismBarChart } from "./charts/MechanismBarChart";
import { MechanismRadarChart } from "./charts/MechanismRadarChart";
import { MechanismScatterChart } from "./charts/MechanismScatterChart";
import { ScoringModeSelector } from "./ScoringModeSelector";
import type { MechanismScore, ScoringMode, MechanismCountMode } from "../types";
import { SCORING_MODES } from "../types";

interface Props {
  scores: MechanismScore[];
  selectedMechanisms: string[];
  onBarClick: (mechanismName: string, ctrlKey: boolean) => void;
  descriptions: Map<string, string>;
}

export function ChartPanel({ scores, selectedMechanisms, onBarClick, descriptions }: Props): React.ReactElement {
  const [mode, setMode] = useState<ScoringMode>("arithmetic");
  const [activeTab, setActiveTab] = useState<"bar" | "radar" | "scatter">("bar");
  const [countMode, setCountMode] = useState<MechanismCountMode>("top20");

  const scoreKey = SCORING_MODES.find((m) => m.key === mode)!.scoreKey;

  const sortedScores = useMemo(
    () => [...scores].sort((a, b) => (b[scoreKey] as number) - (a[scoreKey] as number)),
    [scores, scoreKey]
  );

  const displayedScores = useMemo(() => {
    if (countMode === "all") return sortedScores;
    if (countMode === "bottom20") return sortedScores.slice(-20);
    return sortedScores.slice(0, 20);
  }, [sortedScores, countMode]);

  return (
    <>
      <section className="controls">
        <ScoringModeSelector mode={mode} onModeChange={setMode} />

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
        <div className="chart-header">
          <div className="count-toggle">
            <button
              type="button"
              aria-pressed={countMode === "top20"}
              className={countMode === "top20" ? "active" : ""}
              onClick={() => setCountMode("top20")}
            >
              Top 20
            </button>
            <button
              type="button"
              aria-pressed={countMode === "all"}
              className={countMode === "all" ? "active" : ""}
              onClick={() => setCountMode("all")}
            >
              All
            </button>
            <button
              type="button"
              aria-pressed={countMode === "bottom20"}
              className={countMode === "bottom20" ? "active" : ""}
              onClick={() => setCountMode("bottom20")}
            >
              Bottom 20
            </button>
          </div>
        </div>

        {activeTab === "bar" && (
          <MechanismBarChart
            scores={displayedScores}
            mode={mode}
            maxItems={displayedScores.length}
            selectedMechanisms={selectedMechanisms}
            onBarClick={onBarClick}
            descriptions={descriptions}
          />
        )}
        {activeTab === "radar" && (
          <MechanismRadarChart
            scores={displayedScores}
            mode={mode}
            maxItems={displayedScores.length}
            selectedMechanisms={selectedMechanisms}
            onSegmentClick={onBarClick}
            descriptions={descriptions}
          />
        )}
        {activeTab === "scatter" && (
          <MechanismScatterChart
            scores={displayedScores}
            mode={mode}
            selectedMechanisms={selectedMechanisms}
            onPointClick={onBarClick}
            descriptions={descriptions}
          />
        )}
      </section>
    </>
  );
}
