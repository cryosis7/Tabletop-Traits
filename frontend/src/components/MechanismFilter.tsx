import React, { useState, useRef, useEffect } from "react";
import type { BoardGame, FilterMode } from "../types";
import { MechanismTooltip } from "./MechanismTooltip";

interface Props {
  mechanisms: string[];
  selected: string[];
  filterMode: FilterMode;
  onSelectionChange: (selected: string[]) => void;
  onFilterModeChange: (mode: FilterMode) => void;
  descriptions: Map<string, string>;
  games?: BoardGame[];
}

export function MechanismFilter({
  mechanisms,
  selected,
  filterMode,
  onSelectionChange,
  onFilterModeChange,
  descriptions,
  games = [],
}: Props): React.ReactElement {
  const [query, setQuery] = useState("");
  const [open, setOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  const filtered = mechanisms.filter(
    (m) => m.toLowerCase().includes(query.toLowerCase()) && !selected.includes(m)
  );

  const filteredGames = query
    ? games.filter(
        (g) =>
          g.name.toLowerCase().includes(query.toLowerCase()) &&
          g.mechanisms.some((m) => !selected.includes(m))
      )
    : [];

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

  function addGameMechanisms(game: BoardGame) {
    const newMechanisms = game.mechanisms.filter((m) => !selected.includes(m));
    onSelectionChange([...selected, ...newMechanisms]);
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
          {open && (filtered.length > 0 || filteredGames.length > 0) && (
            <ul className="filter-dropdown">
              {filtered.slice(0, 10).map((m) => (
                <li key={m} onMouseDown={() => addMechanism(m)}>
                  <span>{m}</span>
                  {descriptions.get(m) && (
                    <span className="mechanism-description">{descriptions.get(m)}</span>
                  )}
                </li>
              ))}
              {filteredGames.length > 0 && (
                <>
                  <li className="dropdown-section-header" aria-hidden="true">
                    Games
                  </li>
                  {filteredGames.slice(0, 5).map((g) => (
                    <li
                      key={`game-${g.id}`}
                      className="game-item"
                      onMouseDown={() => addGameMechanisms(g)}
                    >
                      <span>{g.name}</span>
                      <span className="mechanism-description">
                        {g.mechanisms.filter((m) => !selected.includes(m)).length} mechanisms
                      </span>
                    </li>
                  ))}
                </>
              )}
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
