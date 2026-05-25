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
}

export function MechanismRadarChart({ scores, maxItems = 10 }: Props) {
  const data = scores.slice(0, maxItems).map((s) => ({
    mechanism: s.mechanismName,
    rating: s.averageRating,
    fullMark: 10,
  }));

  return (
    <div style={{ width: "100%", height: 400 }}>
      <ResponsiveContainer>
        <RadarChart data={data}>
          <PolarGrid />
          <PolarAngleAxis dataKey="mechanism" tick={{ fontSize: 11 }} />
          <PolarRadiusAxis domain={[0, 10]} tick={{ fontSize: 10 }} />
          <Tooltip formatter={(value: number) => [value.toFixed(2), "Avg Rating"]} />
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
