import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Cell,
} from "recharts";
import type { MechanismScore, ScoringMode } from "../../types";
import { SCORING_MODES } from "../../types";

interface Props {
  scores: MechanismScore[];
  mode: ScoringMode;
  maxItems?: number;
  selectedMechanisms?: string[];
  onBarClick?: (mechanismName: string, ctrlKey: boolean) => void;
  descriptions?: Map<string, string>;
}

function getRatingColor(value: number, max: number): string {
  const ratio = value / max;
  if (ratio >= 0.8) return "#22c55e";
  if (ratio >= 0.6) return "#84cc16";
  if (ratio >= 0.4) return "#eab308";
  if (ratio >= 0.2) return "#f97316";
  return "#ef4444";
}

export function MechanismBarChart({ scores, mode, maxItems = 20, selectedMechanisms = [], onBarClick, descriptions }: Props) {
  const config = SCORING_MODES.find((m) => m.key === mode)!;
  const data = scores.slice(0, maxItems).map((s) => ({
    name: s.mechanismName,
    value: s[config.scoreKey] as number,
    gameCount: s.gameCount,
  }));

  const maxValue = Math.max(...data.map((d) => d.value), 1);

  return (
    <div style={{ width: "100%", height: Math.max(400, data.length * 28) }}>
      <ResponsiveContainer>
        <BarChart data={data} layout="vertical" margin={{ left: 140, right: 20, top: 10, bottom: 10 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis type="number" domain={[0, "auto"]} />
          <YAxis type="category" dataKey="name" width={130} tick={{ fontSize: 12 }} />
          <Tooltip
            content={({ active, payload }) => {
              if (!active || !payload?.length) return null;
              const data = payload[0].payload as { name: string; value: number; gameCount: number };
              const desc = descriptions?.get(data.name);
              return (
                <div className="mechanism-tooltip-content" style={{ background: "#1e293b", color: "#f1f5f9", borderRadius: 6, padding: "8px 12px", maxWidth: 300 }}>
                  <strong style={{ color: "#a5b4fc" }}>{data.name}</strong>
                  {desc && <p style={{ margin: "4px 0 6px", fontSize: "0.8rem", opacity: 0.85 }}>{desc}</p>}
                  <p style={{ margin: 0, fontSize: "0.85rem" }}>
                    {config.label}: {data.value.toFixed(2)} ({data.gameCount} games)
                  </p>
                </div>
              );
            }}
          />
          <Bar
            dataKey="value"
            radius={[0, 4, 4, 0]}
            onClick={(_data, _index, e) => {
              const entry = _data as { name: string };
              if (onBarClick && entry.name) {
                onBarClick(entry.name, e.ctrlKey || e.metaKey);
              }
            }}
            style={{ cursor: onBarClick ? "pointer" : undefined }}
          >
            {data.map((entry, index) => (
              <Cell
                key={index}
                fill={getRatingColor(entry.value, maxValue)}
                stroke={selectedMechanisms.includes(entry.name) ? "#fff" : "none"}
                strokeWidth={selectedMechanisms.includes(entry.name) ? 2 : 0}
              />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}
