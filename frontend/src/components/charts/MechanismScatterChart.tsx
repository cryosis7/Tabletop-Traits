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
import type { MechanismScore } from "../../types";

interface Props {
  scores: MechanismScore[];
  selectedMechanisms?: string[];
  onPointClick?: (mechanismName: string, ctrlKey: boolean) => void;
}

export function MechanismScatterChart({ scores, selectedMechanisms = [], onPointClick }: Props) {
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
            formatter={(value, name) => [
              Number(value).toFixed(2),
              name === "gameCount" ? "Games" : "Avg Rating",
            ]}
            labelFormatter={(_label, payload) =>
              (payload?.[0]?.payload as { name?: string } | undefined)?.name ?? ""
            }
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
