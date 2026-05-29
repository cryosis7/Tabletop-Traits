import { useMemo } from "react";
import { MechanismTooltip } from "./MechanismTooltip";
import type { BoardGame, FilterMode } from "../types";

interface Props {
  collection: BoardGame[];
  selectedMechanisms: string[];
  filterMode: FilterMode;
  descriptions: Map<string, string>;
}

export function CollectionTable({ collection, selectedMechanisms, filterMode, descriptions }: Props): React.ReactElement {
  const filteredCollection = useMemo(() => {
    if (selectedMechanisms.length === 0) return collection;
    return collection.filter((game) => {
      if (filterMode === "any") {
        return selectedMechanisms.some((m) => game.mechanisms.includes(m));
      }
      return selectedMechanisms.every((m) => game.mechanisms.includes(m));
    });
  }, [collection, selectedMechanisms, filterMode]);

  return (
    <section className="collection-section">
      <h2>
        Your Rated Games
        {selectedMechanisms.length > 0
          ? ` (Showing ${filteredCollection.length} of ${collection.length})`
          : ` (${collection.length})`}
      </h2>
      {filteredCollection.length > 0 && (
        <table className="collection-table">
          <thead>
            <tr>
              <th>Game</th>
              <th>Year</th>
              <th>Your Rating</th>
              <th>Mechanisms</th>
            </tr>
          </thead>
          <tbody>
            {filteredCollection.map((game) => (
              <tr key={game.id}>
                <td>{game.name}</td>
                <td>{game.yearPublished ?? "-"}</td>
                <td className="rating">{game.userRating}</td>
                <td className="mechanisms">
                  {game.mechanisms.map((m, i) => (
                    <span key={m}>
                      {i > 0 && ", "}
                      <MechanismTooltip
                        text={m}
                        description={descriptions.get(m)}
                        className={selectedMechanisms.includes(m) ? "mechanism-highlight" : ""}
                      />
                    </span>
                  ))}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </section>
  );
}
