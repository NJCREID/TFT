import { memo } from "react";
import SelectorItem from "../SelectorItem/SelectorItem";
import "./selector.css";
import LoadingSpinner from "../LoadingSpinner/LoadingSpinner";
import { Augment, Item, Trait, Unit } from "../../data";

interface ItemsSelectionProps {
  data: (Unit | Item | Augment | Trait)[] | null;
  height?: string;
}

const Selector = ({ data, height = "300px" }: ItemsSelectionProps) => {
  return (
    <div className="app__selector" style={{ height }}>
      {" "}
      {!data ? (
        <LoadingSpinner />
      ) : (
        !!data.length && (
          <div className="app__selector-container">
            {data.map((item, index) => (
              <SelectorItem key={index} object={item} />
            ))}
          </div>
        )
      )}
    </div>
  );
};
export default memo(Selector);
