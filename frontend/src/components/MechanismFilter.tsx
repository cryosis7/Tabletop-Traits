import React, { useState, useRef, useEffect } from "react";
import type { FilterMode } from "../types";
import { MechanismTooltip } from "./MechanismTooltip";

interface Props {
  mechanisms: string[];
  selected: string[];
  filterMode: FilterMode;
  onSelectionChange: (selected: string[]) => void;
  onFilterModeChange: (mode: FilterMode) => void;
  descriptions: Map<string, string>;
}

export function MechanismFilter({
  mechanisms,
  selected,
  filterMode,
  onSelectionChange,
  onFilterModeChange,
  descriptions,
}: Props): React.ReactElement {
  const [query, setQuery] = useState("");
  const [open, setOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  const filtered = mechanisms.filter(
    (m) => m.toLowerCase().includes(query.toLowerCase()) && !selected.includes(m)
  );

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  function addMechanism(name: string) {
    onSelectionChange([...selected, name]);
    setQuery("");
    setOpen(false);
  }

  function removeMechanism(name: string) {
    onSelectionChange(selected.filter((s) => s !== name));
  }

  return (
    <div className="mechanism-filter" ref={containerRef}>
      <div className="filter-header">
        <div className="filter-search-wrapper">
          <input
            type="text"
            className="filter-search"
            placeholder="Search mechanisms..."
            value={query}
            onChange={(e) => {
              setQuery(e.target.value);
              setOpen(true);
            }}
            onFocus={() => setOpen(true)}
          />
          {open && filtered.length > 0 && (
            <ul className="filter-dropdown">
              {filtered.slice(0, 15).map((m) => (
                <li key={m} onMouseDown={() => addMechanism(m)}>
                  <span>{m}</span>
                  {descriptions.get(m) && (
                    <span className="mechanism-description">{descriptions.get(m)}</span>
                  )}
                </li>
              ))}
            </ul>
          )}
        </div>

        <div className="filter-mode-toggle">
          <button
            type="button"
            className={filterMode === "any" ? "active" : ""}
            onClick={() => onFilterModeChange("any")}
          >
            ANY
          </button>
          <button
            type="button"
            className={filterMode === "all" ? "active" : ""}
            onClick={() => onFilterModeChange("all")}
          >
            ALL
          </button>
        </div>
      </div>

      {selected.length > 0 && (
        <div className="filter-chips">
          {selected.map((m) => (
            <span key={m} className="chip">
              <MechanismTooltip text={m} description={descriptions.get(m)} />
              <button type="button" className="chip-remove" onClick={() => removeMechanism(m)}>
                &times;
              </button>
            </span>
          ))}
          <button type="button" className="clear-all" onClick={() => onSelectionChange([])}>
            Clear all
          </button>
        </div>
      )}
    </div>
  );
}
