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
import type { MechanismScore } from "../../types";
import type { ScoringMode } from "../../types";

interface Props {
  scores: MechanismScore[];
  mode: ScoringMode;
  maxItems?: number;
}

function getRatingColor(value: number, max: number): string {
  const ratio = value / max;
  if (ratio >= 0.8) return "#22c55e";
  if (ratio >= 0.6) return "#84cc16";
  if (ratio >= 0.4) return "#eab308";
  if (ratio >= 0.2) return "#f97316";
  return "#ef4444";
}

export function MechanismBarChart({ scores, mode, maxItems = 20 }: Props) {
  const data = scores.slice(0, maxItems).map((s) => ({
    name: s.mechanismName,
    value: mode === "average" ? s.averageRating : s.totalRating,
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
            formatter={(value: number, _name: string, props: { payload: { gameCount: number } }) => [
              `${value.toFixed(2)} (${props.payload.gameCount} games)`,
              mode === "average" ? "Avg Rating" : "Total Rating",
            ]}
          />
          <Bar dataKey="value" radius={[0, 4, 4, 0]}>
            {data.map((entry, index) => (
              <Cell key={index} fill={getRatingColor(entry.value, maxValue)} />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}
