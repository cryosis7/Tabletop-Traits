import { useTranslation } from "react-i18next";

interface Props {
  message?: string;
}

export function LoadingSpinner({ message }: Props): React.ReactElement {
  const { t } = useTranslation();

  return (
    <div className="loading-spinner" role="status" aria-live="polite">
      <div className="spinner" />
      <span>{message ?? t("sync.loadingMessage")}</span>
    </div>
  );
}
