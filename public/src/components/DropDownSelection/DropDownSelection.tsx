import { memo, useState } from "react";
import { FaChevronDown } from "react-icons/fa";
import "./dropdownselection.css";
import { Trait } from "../../common/api";

interface DropDownItem {
  key: string;
  name: string;
  imageUrl?: string;
}

interface Props {
  items: Trait[] | DropDownItem[];
  onSelect: (item: string) => void;
  startingIndex?: number;
}

const DropDownSelection = ({ items, onSelect, startingIndex = 0 }: Props) => {
  const [isActive, setIsActive] = useState(false);
  const [currentSelection, setCurrentSelection] = useState<Trait | DropDownItem>(items[startingIndex]);

  const handleSelection = (item: DropDownItem | Trait) => {
    onSelect(item.key);
    setIsActive(false);
    setCurrentSelection(item);
  };

  return (
    <div className="app__dropdownselection">
      <button
        type="button"
        onClick={() => setIsActive((prev) => !prev)}
        className={`app__dropdownselection-button ${isActive ? "active" : ""}`}
      >
        <div className="app__dropdownselection-button-container">
          {currentSelection.imageUrl && (
            <div className="app__dropdownselection-button-icon">
              <img src={currentSelection.imageUrl} alt={`${currentSelection.name}`} />
            </div>
          )}

          <p className="p__opensans">{currentSelection.name}</p>
        </div>
        <FaChevronDown className={`fa-chevron-down ${isActive ? "rotate" : ""}`} />
      </button>
      {isActive && (
        <div className="app__dropdownselection-menulist">
          <ul>
            {items
              .filter((item) => item.name !== currentSelection.name)
              .map((item) => (
                <li
                  key={item.key}
                  className="app__dropdownselection-menulist-item"
                  onClick={() => handleSelection(item)}
                >
                  <div className="app__dropdownselection-menulist-item-container">
                    {item.imageUrl && <img src={item.imageUrl} alt={item.name} />}
                    <p className="p__opensans">{item.name}</p>
                  </div>
                </li>
              ))}
          </ul>
        </div>
      )}
    </div>
  );
};
export default memo(DropDownSelection);
