import { useState, useMemo } from "react";
import { useTranslation } from "react-i18next";
import { useSync, useMechanismScores, useCollection, useMechanismDescriptions } from "../hooks/useApi";
import { SyncPanel } from "../components/SyncPanel";
import { ChartPanel } from "../components/ChartPanel";
import { CollectionTable } from "../components/CollectionTable";
import { MechanismFilter } from "../components/MechanismFilter";
import { LoadingSpinner } from "../components/LoadingSpinner";
import type { FilterMode } from "../types";

export function Dashboard() {
  const { t } = useTranslation();
  const [selectedMechanisms, setSelectedMechanisms] = useState<string[]>([]);
  const [filterMode, setFilterMode] = useState<FilterMode>("any");

  const { sync, syncing, syncStatus, error: syncError } = useSync();
  const { scores, loading: scoresLoading, fetchScores } = useMechanismScores();
  const { collection, loading: collectionLoading, fetchCollection } = useCollection();
  const { descriptions, fetchDescriptions } = useMechanismDescriptions();

  const dataLoading = scoresLoading || collectionLoading;

  const handleSync = async (username: string) => {
    await sync(username);
    await fetchScores(username);
    await fetchCollection(username);
    await fetchDescriptions();
  };

  const availableMechanisms = useMemo(
    () => [...scores].sort((a, b) => b.arithmeticMean - a.arithmeticMean).map((s) => s.mechanismName),
    [scores]
  );

  function handleChartClick(mechanismName: string, ctrlKey: boolean) {
    if (ctrlKey) {
      setSelectedMechanisms((prev) =>
        prev.includes(mechanismName)
          ? prev.filter((m) => m !== mechanismName)
          : [...prev, mechanismName]
      );
    } else {
      setSelectedMechanisms([mechanismName]);
    }
  }

  return (
    <div className="dashboard">
      <header className="header">
        <h1>{t("app.title")}</h1>
        <p>{t("app.subtitle")}</p>
      </header>

      <SyncPanel
        onSync={handleSync}
        syncing={syncing}
        syncStatus={syncStatus}
        syncError={syncError}
        loading={dataLoading}
      />

      {(scores.length > 0 || dataLoading) && (
        <>
          {dataLoading ? (
            <LoadingSpinner />
          ) : (
            <>
              <ChartPanel
                scores={scores}
                selectedMechanisms={selectedMechanisms}
                onBarClick={handleChartClick}
                descriptions={descriptions}
              />

              <section className="filter-section">
                <MechanismFilter
                  mechanisms={availableMechanisms}
                  selected={selectedMechanisms}
                  filterMode={filterMode}
                  onSelectionChange={setSelectedMechanisms}
                  onFilterModeChange={setFilterMode}
                  descriptions={descriptions}
                  games={collection}
                />
              </section>

              <CollectionTable
                collection={collection}
                selectedMechanisms={selectedMechanisms}
                filterMode={filterMode}
                descriptions={descriptions}
              />
            </>
          )}
        </>
      )}
    </div>
  );
}
