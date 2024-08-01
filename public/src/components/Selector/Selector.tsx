import { memo } from "react";
import { Augment, Item, Unit } from "../../common/api";
import SelectorItem from "../SelectorItem/SelectorItem";
import "./selector.css";

interface ItemsSelectionProps {
  data: (Unit | Item | Augment)[];
}

const Selector = ({ data }: ItemsSelectionProps) => {
  return (
    <div className="app__selector">
      <div className="app__selector-container">
        {data.map((item, index) => (
          <SelectorItem key={index} item={item} />
        ))}
      </div>
    </div>
  );
};
export default memo(Selector);
