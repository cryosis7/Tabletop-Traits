import {
  ScatterChart,
  Scatter,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  ZAxis,
} from "recharts";
import type { MechanismScore } from "../../types";

interface Props {
  scores: MechanismScore[];
}

export function MechanismScatterChart({ scores }: Props) {
  const data = scores.map((s) => ({
    name: s.mechanismName,
    gameCount: s.gameCount,
    averageRating: s.averageRating,
    totalRating: s.totalRating,
  }));

  return (
    <div style={{ width: "100%", height: 400 }}>
      <ResponsiveContainer>
        <ScatterChart margin={{ top: 10, right: 20, bottom: 20, left: 20 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis
            type="number"
            dataKey="gameCount"
            name="Games"
            label={{ value: "Number of Games", position: "bottom", offset: 0 }}
          />
          <YAxis
            type="number"
            dataKey="averageRating"
            name="Avg Rating"
            domain={[0, 10]}
            label={{ value: "Average Rating", angle: -90, position: "insideLeft" }}
          />
          <ZAxis type="number" dataKey="totalRating" range={[40, 400]} />
          <Tooltip
            formatter={(value: number, name: string) => [
              value.toFixed(2),
              name === "gameCount" ? "Games" : "Avg Rating",
            ]}
            labelFormatter={(_: unknown, payload: Array<{ payload?: { name: string } }>) =>
              payload?.[0]?.payload?.name ?? ""
            }
          />
          <Scatter data={data} fill="#6366f1" fillOpacity={0.7} />
        </ScatterChart>
      </ResponsiveContainer>
    </div>
  );
}
