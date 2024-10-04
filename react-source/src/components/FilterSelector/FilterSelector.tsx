import { memo, useState } from "react";
import "./filterselector.css";
import { getImageUrl } from "../../utils";

interface SelectionItem {
  key?: string;
  inGameKey?: string;
  name: string;
  imageUrl?: string;
}

interface FilterSelectorProps {
  items: SelectionItem[];
  onSelect: (item: string) => void;
}

const FilterSelector = ({ items, onSelect }: FilterSelectorProps) => {
  const [isActive, setIsActive] = useState<number | null>(0);

  const handleClick = (index: number) => {
    setIsActive(index);
    const selectedItem =
      items[index].key == "all"
        ? items[index].key
        : items[index].inGameKey
        ? items[index].inGameKey
        : items[index].key || items[index].name;
    onSelect(selectedItem ?? "");
  };

  return (
    <div className="app__filterselector">
      {items.map((item, index) => {
        const imageUrl = item.imageUrl
          ? item.imageUrl
          : item.key === "all"
          ? "/images/general/all-option.svg"
          : item.inGameKey
          ? getImageUrl(item.inGameKey, "items")
          : "";
        const altText = item.inGameKey || item.name;

        return (
          <div
            key={item.name}
            onClick={() => handleClick(index)}
            className={`app__filterselector-container ${isActive === index ? "active" : ""}`}
          >
            {imageUrl ? <img src={imageUrl} alt={altText} /> : <span className="p__opensans p__bold">{item.name}</span>}
          </div>
        );
      })}
    </div>
  );
};
export default memo(FilterSelector);
