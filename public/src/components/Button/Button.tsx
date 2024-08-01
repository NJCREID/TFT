import { ButtonHTMLAttributes, ReactNode } from "react";
import "./button.css";
interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children: ReactNode;
}

const Button = ({ children, onClick, type = "button", className, ...rest }: ButtonProps) => {
  return (
    <button type={type} onClick={onClick} className={`app__button p__bold p__opensans ${className || ""}`} {...rest}>
      {children}
    </button>
  );
};
export default Button;
