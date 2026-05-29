interface Props {
  message?: string;
}

export function LoadingSpinner({ message = "Loading analysis..." }: Props): React.ReactElement {
  return (
    <div className="loading-spinner" role="status" aria-live="polite">
      <div className="spinner" />
      <span>{message}</span>
    </div>
  );
}
