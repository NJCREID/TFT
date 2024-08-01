import { memo, useState } from "react";
import "./filterselector.css";

interface Item {
  key: string;
  name?: string;
  imageUrl?: string;
}

interface FilterSelectorProps {
  items: Item[];
  onSelect: (item: string | null) => void;
  selectFirst?: boolean;
  allowToggle?: boolean;
}

const FilterSelector = ({ items, onSelect, allowToggle = false }: FilterSelectorProps) => {
  const [isActive, setIsActive] = useState<number | null>(0);

  const handleClick = (index: number) => {
    if (allowToggle) {
      setIsActive(isActive == index ? null : index);
      onSelect(isActive == index ? null : items[index].key);
    } else if (!allowToggle) {
      setIsActive(index);
      onSelect(items[index].key);
    }
  };

  return (
    <div className="app__filterselector">
      {items.map((item, index) => (
        <div
          key={item.key}
          onClick={() => handleClick(index)}
          className={`app__filterselector-container ${isActive === index ? "active" : ""}`}
        >
          {item.imageUrl ? (
            <img src={item.imageUrl} alt={item.name} />
          ) : (
            <span className="p__opensans p__bold">{item.name}</span>
          )}
        </div>
      ))}
    </div>
  );
};
export default memo(FilterSelector);
