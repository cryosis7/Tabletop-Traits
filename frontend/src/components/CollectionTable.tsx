import { useMemo } from "react";
import { useTranslation } from "react-i18next";
import { MechanismTooltip } from "./MechanismTooltip";
import type { BoardGame, FilterMode } from "../types";

interface Props {
  collection: BoardGame[];
  selectedMechanisms: string[];
  filterMode: FilterMode;
  descriptions: Map<string, string>;
}

export function CollectionTable({ collection, selectedMechanisms, filterMode, descriptions }: Props): React.ReactElement {
  const { t } = useTranslation();
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
        {t("collection.title")}
        {selectedMechanisms.length > 0
          ? ` (${t("collection.showing", { filtered: filteredCollection.length, total: collection.length })})`
          : ` (${collection.length})`}
      </h2>
      {filteredCollection.length > 0 && (
        <table className="collection-table">
          <thead>
            <tr>
              <th>{t("collection.game")}</th>
              <th>{t("collection.year")}</th>
              <th>{t("collection.yourRating")}</th>
              <th>{t("collection.mechanisms")}</th>
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
