import { ButtonHTMLAttributes, ReactNode } from "react";
import "./button.css";
interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children: ReactNode;
  "aria-label": string;
}

const Button = ({ children, onClick, type = "button", className, "aria-label": ariaLabel, ...rest }: ButtonProps) => {
  return (
    <button
      type={type}
      onClick={onClick}
      className={`app__button p__bold p__opensans ${className || ""}`}
      aria-label={ariaLabel}
      {...rest}
    >
      {children}
    </button>
  );
};
export default Button;
