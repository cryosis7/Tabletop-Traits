import {
  RadarChart,
  Radar,
  PolarGrid,
  PolarAngleAxis,
  PolarRadiusAxis,
  ResponsiveContainer,
  Tooltip,
} from "recharts";
import type { MechanismScore, ScoringMode } from "../../types";

interface Props {
  scores: MechanismScore[];
  mode: ScoringMode;
  maxItems?: number;
  selectedMechanisms?: string[];
  onSegmentClick?: (mechanismName: string, ctrlKey: boolean) => void;
  descriptions?: Map<string, string>;
}

export function MechanismRadarChart({ scores, mode, maxItems = 10, selectedMechanisms = [], onSegmentClick, descriptions }: Props) {
  const data = scores.slice(0, maxItems).map((s) => ({
    mechanism: s.mechanismName,
    rating: mode === "average" ? s.averageRating : s.totalRating,
    fullMark: mode === "average" ? 10 : Math.max(...scores.slice(0, maxItems).map((x) => x.totalRating), 1),
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
          <Tooltip
            content={({ active, payload }) => {
              if (!active || !payload?.length) return null;
              const entry = payload[0].payload as { mechanism: string; rating: number };
              const desc = descriptions?.get(entry.mechanism);
              return (
                <div className="mechanism-tooltip-content" style={{ background: "#1e293b", color: "#f1f5f9", borderRadius: 6, padding: "8px 12px", maxWidth: 300 }}>
                  <strong style={{ color: "#a5b4fc" }}>{entry.mechanism}</strong>
                  {desc && <p style={{ margin: "4px 0 6px", fontSize: "0.8rem", opacity: 0.85 }}>{desc}</p>}
                  <p style={{ margin: 0, fontSize: "0.85rem" }}>
                    {mode === "average" ? "Avg Rating" : "Total Rating"}: {entry.rating.toFixed(2)}
                  </p>
                </div>
              );
            }}
          />
          <Radar
            name="Rating"
            dataKey="rating"
            stroke="#6366f1"
            fill="#6366f1"
            fillOpacity={0.3}
            dot={{ r: 4, fill: "#6366f1", className: "recharts-radar-dot" }}
            activeDot={{ r: 6, fill: "#a5b4fc" }}
          />
        </RadarChart>
      </ResponsiveContainer>
    </div>
  );
}
