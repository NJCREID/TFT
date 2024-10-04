import { useState } from "react";
import { RootState } from "../../store";
import { useSelector } from "react-redux";
import { isItem, isTrait, isUnit } from "../../utils/isObject";
import TraitDetailsView from "../TraitDetailsView/TraitDetailsView";
import UnitDetailsView from "../UnitDetailsView/UnitDetailsView";
import ItemDetailsView from "../ItemDetailsView/ItemDetailsView";
import AugmentDetailsView from "../AugmentDetailsView/AugmentDetailsView";
import { Augment, Item, Trait, Unit } from "../../data";
import "./selectedview.css";

export const CoOccurrenceTYPES = [
  { inGameKey: "unit", name: "Units" },
  { inGameKey: "item", name: "Items" },
  { inGameKey: "trait", name: "Traits" },
  { inGameKey: "augment", name: "Augments" },
];

interface SelectedViewProps {
  onTypeChange: (key: string) => void;
  onObjectChange: (key: string | null) => void;
  type: "unit" | "item" | "trait" | "augment";
}

const SelectedView = ({ onTypeChange, onObjectChange, type }: SelectedViewProps) => {
  const [selectedObject, setSelectedObject] = useState<Unit | Item | Trait | Augment | null>(null);
  const draggedItem = useSelector((state: RootState) => state.drag.draggedItem);

  const handleTypeChange = (key: string) => {
    onTypeChange(key);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    if (draggedItem) {
      if (isTrait(draggedItem)) {
        const defaultTier = draggedItem.tiers![0].level;
        setSelectedObject(draggedItem);
        onObjectChange(`${draggedItem.inGameKey}-${defaultTier}`);
      } else {
        setSelectedObject(draggedItem);
        onObjectChange(draggedItem.inGameKey);
      }
    }
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
  };

  const handleClick = () => {
    setSelectedObject(null);
    onObjectChange(null);
  };

  const handleTierChange = (newTier: string) => {
    if (selectedObject && isTrait(selectedObject)) {
      onObjectChange(`${selectedObject.inGameKey}-${newTier}`);
    }
  };

  return (
    <div className="app__selectedview" onDrop={handleDrop} onDragOver={handleDragOver}>
      {selectedObject ? (
        isUnit(selectedObject) ? (
          <UnitDetailsView unit={selectedObject} onTypeChange={handleTypeChange} onClick={handleClick} />
        ) : isItem(selectedObject) ? (
          <ItemDetailsView item={selectedObject} onTypeChange={handleTypeChange} onClick={handleClick} />
        ) : isTrait(selectedObject) ? (
          <TraitDetailsView
            trait={selectedObject}
            onTierChange={handleTierChange}
            onTypeChange={handleTypeChange}
            onClick={handleClick}
          />
        ) : (
          <AugmentDetailsView augment={selectedObject} onTypeChange={handleTypeChange} onClick={handleClick} />
        )
      ) : (
        <p className="app__selectedview-message p__opensans-title">Drag a {type} to see CoOccurrence Stats</p>
      )}
    </div>
  );
};

export default SelectedView;
