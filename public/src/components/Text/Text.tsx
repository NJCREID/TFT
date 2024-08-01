import { ReactNode } from "react";
import "./text.css";
interface Props {
  color?: string;
  backgroundColor?: string;
  children: ReactNode;
}

export default function Text({ color = "#fff", backgroundColor = "var(--color-lightred)", children }: Props) {
  return (
    <div className="app__text" style={{ color, backgroundColor }}>
      {children}
    </div>
  );
}
