import React, { useState, useRef, useEffect } from "react";

interface Props {
  text: string;
  description: string | undefined;
  className?: string;
  children?: React.ReactNode;
}

export function MechanismTooltip({ text, description, className, children }: Props): React.ReactElement {
  const [visible, setVisible] = useState(false);
  const [position, setPosition] = useState<{ top: number; left: number }>({ top: 0, left: 0 });
  const triggerRef = useRef<HTMLSpanElement>(null);
  const tooltipRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (visible && triggerRef.current) {
      const rect = triggerRef.current.getBoundingClientRect();
      setPosition({
        top: rect.top - 8,
        left: rect.left + rect.width / 2,
      });
    }
  }, [visible]);

  if (!description) {
    return <span className={className}>{children ?? text}</span>;
  }

  return (
    <>
      <span
        ref={triggerRef}
        className={className}
        onMouseEnter={() => setVisible(true)}
        onMouseLeave={() => setVisible(false)}
        style={{ cursor: "help" }}
      >
        {children ?? text}
      </span>
      {visible && (
        <div
          ref={tooltipRef}
          role="tooltip"
          className="mechanism-tooltip"
          style={{
            position: "fixed",
            top: position.top,
            left: position.left,
            transform: "translate(-50%, -100%)",
            zIndex: 9999,
          }}
        >
          <div className="mechanism-tooltip-content">
            <strong>{text}</strong>
            <p>{description}</p>
          </div>
        </div>
      )}
    </>
  );
}
