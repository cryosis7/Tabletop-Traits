import { useState, useRef, useEffect } from "react";
import type { ScoringMode } from "../types";
import { SCORING_MODES } from "../types";

interface Props {
  mode: ScoringMode;
  onModeChange: (mode: ScoringMode) => void;
}

export function ScoringModeSelector({ mode, onModeChange }: Props): React.ReactElement {
  const [open, setOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  const selectedConfig = SCORING_MODES.find((m) => m.key === mode)!;

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  function selectMode(key: ScoringMode) {
    onModeChange(key);
    setOpen(false);
  }

  return (
    <div className="mode-selector" ref={containerRef}>
      <div className="mode-selector-wrapper">
        <button
          type="button"
          id="scoring-mode-select"
          className="mode-selector-trigger"
          aria-expanded={open}
          aria-haspopup="listbox"
          onClick={() => setOpen(!open)}
        >
          <span>{selectedConfig.label}</span>
          <span className="mode-selector-chevron" aria-hidden="true">
            {open ? "\u25B2" : "\u25BC"}
          </span>
        </button>
        {open && (
          <ul className="mode-dropdown" role="listbox" aria-label="Scoring method">
            {SCORING_MODES.map((m) => (
              <li
                key={m.key}
                role="option"
                aria-selected={m.key === mode}
                className={m.key === mode ? "mode-dropdown-item active" : "mode-dropdown-item"}
                onMouseDown={() => selectMode(m.key)}
              >
                <span className="mode-dropdown-label">{m.label}</span>
                <span className="mode-dropdown-description">{m.description}</span>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
