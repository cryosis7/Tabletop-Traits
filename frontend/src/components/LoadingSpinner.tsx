import { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";

const COLD_START_DELAY_MS = 7_000;

interface Props {
  message?: string;
}

export function LoadingSpinner({ message }: Props): React.ReactElement {
  const { t } = useTranslation();
  const [showColdStartHint, setShowColdStartHint] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => setShowColdStartHint(true), COLD_START_DELAY_MS);
    return () => clearTimeout(timer);
  }, []);

  return (
    <div className="loading-spinner" role="status" aria-live="polite">
      <div className="spinner" />
      <span>{message ?? t("sync.loadingMessage")}</span>
      {showColdStartHint && (
        <p className="cold-start-hint">{t("sync.coldStartHint")}</p>
      )}
    </div>
  );
}
