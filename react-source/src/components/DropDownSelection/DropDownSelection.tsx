import { memo, useState } from "react";
import { FaChevronDown } from "react-icons/fa";
import "./dropdownselection.css";
import { getImageUrl } from "../../utils";

interface DropDownItem {
  name: string;
  inGameKey?: string;
  imageUrl?: string;
}

interface Props {
  items: DropDownItem[];
  onSelect: (item: string) => void;
  startingIndex?: number;
  includeImages?: boolean;
  includeAll?: boolean;
  allDisplayName?: string;
}

const DropDownSelection = ({
  items,
  onSelect,
  startingIndex = 0,
  includeImages = false,
  includeAll = false,
  allDisplayName = "All",
}: Props) => {
  const [isActive, setIsActive] = useState(false);
  const allItem = { inGameKey: "all", name: allDisplayName, imageUrl: "/images/general/all-option.svg" };
  const extendedItems = includeAll ? [allItem, ...items] : items;
  const [currentSelection, setCurrentSelection] = useState<DropDownItem>(extendedItems[startingIndex]);

  const handleSelection = (item: DropDownItem) => {
    onSelect(item.inGameKey || item.name);
    setIsActive(false);
    setCurrentSelection(item);
  };

  const getImage = (item: DropDownItem) => {
    return includeImages ? item.imageUrl || (item.inGameKey && getImageUrl(item.inGameKey, "traits")) : undefined;
  };

  return (
    <div className="app__dropdownselection">
      <button
        type="button"
        onClick={() => setIsActive((prev) => !prev)}
        className={`app__dropdownselection-button ${isActive ? "active" : ""}`}
      >
        <div className="app__dropdownselection-button-container">
          {currentSelection && getImage(currentSelection) && (
            <img src={getImage(currentSelection)} alt={currentSelection.name} />
          )}
          <p className="p__opensans">{currentSelection?.name}</p>
        </div>
        <FaChevronDown className={`fa-chevron-down ${isActive ? "rotate" : ""}`} />
      </button>
      {isActive && (
        <div className="app__dropdownselection-menulist">
          <ul>
            {extendedItems
              .filter((item) => item.name !== currentSelection.name)
              .map((item) => {
                const imageUrl = getImage(item);
                return (
                  <li
                    key={item.inGameKey || item.name}
                    className="app__dropdownselection-menulist-item"
                    onClick={() => handleSelection(item)}
                  >
                    {imageUrl && <img src={imageUrl} alt={item.name} />}
                    <p className="p__opensans">{item.name}</p>
                  </li>
                );
              })}
          </ul>
        </div>
      )}
    </div>
  );
};

export default memo(DropDownSelection);
