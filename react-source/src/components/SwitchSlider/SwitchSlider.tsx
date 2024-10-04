import { memo, useState } from "react";
import "./switchslider.css";

interface SwitchSliderProps {
  type1: string;
  type2: string;
  setAction: () => void;
}

const SwitchSlider = ({ type1, type2, setAction }: SwitchSliderProps) => {
  const [isChecked, setIsChecked] = useState(false);
  const handleInputChange = () => {
    setIsChecked((prev) => !prev);
    setAction();
  };

  return (
    <label className="app__switch">
      <span className="app__switch-type1 p__opensans">{type1}</span>
      <span className="app__switch-container">
        <input
          type="checkbox"
          checked={isChecked}
          onChange={handleInputChange}
        />
        <span
          className={`app__switch-slider ${isChecked ? "checked" : ""}`}
        ></span>
      </span>
      <span className="app__switch-type2 p__opensans">{type2}</span>
    </label>
  );
};
export default memo(SwitchSlider);
