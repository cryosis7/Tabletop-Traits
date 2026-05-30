import {
  ScatterChart,
  Scatter,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  ZAxis,
  Cell,
} from "recharts";
import { useTranslation } from "react-i18next";
import type { MechanismScore, ScoringMode } from "../../types";
import { SCORING_MODES } from "../../types";

interface Props {
  scores: MechanismScore[];
  mode: ScoringMode;
  selectedMechanisms?: string[];
  onPointClick?: (mechanismName: string, ctrlKey: boolean) => void;
  descriptions?: Map<string, string>;
}

export function MechanismScatterChart({ scores, mode, selectedMechanisms = [], onPointClick, descriptions }: Props) {
  const { t } = useTranslation();
  const config = SCORING_MODES.find((m) => m.key === mode)!;
  const yDomain: [number, number] = mode === "positiveRate" ? [0, 100] : [0, 10];
  const data = scores.map((s) => ({
    name: s.mechanismName,
    gameCount: s.gameCount,
    score: s[config.scoreKey] as number,
  }));

  return (
    <div style={{ width: "100%", height: 400 }}>
      <ResponsiveContainer>
        <ScatterChart margin={{ top: 10, right: 20, bottom: 20, left: 20 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis
            type="number"
            dataKey="gameCount"
            name={t("charts.games")}
            label={{ value: t("charts.numberOfGames"), position: "bottom", offset: 0 }}
          />
          <YAxis
            type="number"
            dataKey="score"
            name={config.label}
            domain={yDomain}
            label={{ value: config.label, angle: -90, position: "insideLeft" }}
          />
          <ZAxis type="number" dataKey="gameCount" range={[40, 400]} />
          <Tooltip
            content={({ active, payload }) => {
              if (!active || !payload?.length) return null;
              const entry = payload[0].payload as { name: string; gameCount: number; score: number };
              const desc = descriptions?.get(entry.name);
              return (
                <div className="mechanism-tooltip-content" style={{ background: "#1e293b", color: "#f1f5f9", borderRadius: 6, padding: "8px 12px" }}>
                  <strong style={{ color: "#a5b4fc" }}>{entry.name}</strong>
                  {desc && <p style={{ margin: "4px 0 6px", fontSize: "0.8rem", opacity: 0.85 }}>{desc}</p>}
                  <p style={{ margin: 0, fontSize: "0.85rem" }}>
                    {config.label}: {entry.score.toFixed(2)} ({entry.gameCount} games)
                  </p>
                </div>
              );
            }}
          />
          <Scatter
            data={data}
            fill="#6366f1"
            fillOpacity={0.7}
            onClick={(point, _index, e) => {
              const name = (point as unknown as { name: string }).name;
              if (onPointClick && name) {
                onPointClick(name, e.ctrlKey || e.metaKey);
              }
            }}
            style={{ cursor: onPointClick ? "pointer" : undefined }}
          >
            {data.map((entry, index) => (
              <Cell
                key={index}
                fill={selectedMechanisms.includes(entry.name) ? "#a5b4fc" : "#6366f1"}
                stroke={selectedMechanisms.includes(entry.name) ? "#fff" : "none"}
                strokeWidth={selectedMechanisms.includes(entry.name) ? 2 : 0}
              />
            ))}
          </Scatter>
        </ScatterChart>
      </ResponsiveContainer>
    </div>
  );
}
