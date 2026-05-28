import {
  RadarChart,
  Radar,
  PolarGrid,
  PolarAngleAxis,
  PolarRadiusAxis,
  ResponsiveContainer,
  Tooltip,
} from "recharts";
import type { MechanismScore } from "../../types";

interface Props {
  scores: MechanismScore[];
  maxItems?: number;
  selectedMechanisms?: string[];
  onSegmentClick?: (mechanismName: string, ctrlKey: boolean) => void;
}

export function MechanismRadarChart({ scores, maxItems = 10, selectedMechanisms = [], onSegmentClick }: Props) {
  const data = scores.slice(0, maxItems).map((s) => ({
    mechanism: s.mechanismName,
    rating: s.averageRating,
    fullMark: 10,
  }));

  return (
    <div style={{ width: "100%", height: 400 }}>
      <ResponsiveContainer>
        <RadarChart
          data={data}
          onClick={(state, e) => {
            if (onSegmentClick && state?.activeLabel) {
              onSegmentClick(String(state.activeLabel), e.ctrlKey || e.metaKey);
            }
          }}
          style={{ cursor: onSegmentClick ? "pointer" : undefined }}
        >
          <PolarGrid />
          <PolarAngleAxis
            dataKey="mechanism"
            tick={(props) => {
              const { payload, x, y, textAnchor } = props as { payload: { value: string }; x: number; y: number; textAnchor: string };
              return (
                <text
                  x={x}
                  y={y}
                  textAnchor={textAnchor as "start" | "middle" | "end" | "inherit"}
                  fontSize={11}
                  fill={selectedMechanisms.includes(payload.value) ? "#a5b4fc" : "#94a3b8"}
                  fontWeight={selectedMechanisms.includes(payload.value) ? 700 : 400}
                >
                  {payload.value}
                </text>
              );
            }}
          />
          <PolarRadiusAxis domain={[0, 10]} tick={{ fontSize: 10 }} />
          <Tooltip formatter={(value) => [Number(value).toFixed(2), "Avg Rating"]} />
          <Radar
            name="Rating"
            dataKey="rating"
            stroke="#6366f1"
            fill="#6366f1"
            fillOpacity={0.3}
          />
        </RadarChart>
      </ResponsiveContainer>
    </div>
  );
}
