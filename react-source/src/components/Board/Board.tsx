import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../../store";
import { setDraggedItem } from "../../store/dragSlice";
import { showError } from "../../store/errorModalSlice";
import "./board.css";
import { Item, Tier, Trait, Unit, UserGuideTrait } from "../../data";
import { HexItem } from "../../pages/TeamBuilder/TeamBuilder";
import { generateTier, isItem, isUnit } from "../../utils";
import HexagonSVG from "../HexagonSVG/HexagonSVG";
import { getImageUrl } from "../../utils/getImageUrl";

interface BoardProps {
  boardTraits: UserGuideTrait[];
  setBoardTraits: React.Dispatch<React.SetStateAction<UserGuideTrait[]>>;
  hexes: HexItem[];
  setHexes: React.Dispatch<React.SetStateAction<HexItem[]>>;
}

const Board = ({ boardTraits, setBoardTraits, hexes, setHexes }: BoardProps) => {
  const [draggedIndex, setDraggedIndex] = useState<number | null>(null);
  const draggedItem = useSelector((state: RootState) => state.drag.draggedItem);
  const dispatch = useDispatch();

  const updateBoardTraits = (unit: Unit, add: boolean) => {
    const updatedTraits = [...boardTraits];
    const countItemInstance = hexes.filter((hex) => hex.unit?.name === unit.name).length;

    unit.traits.forEach((trait) => {
      const existingTraitIndex = updatedTraits.findIndex((traitItem) => traitItem.inGameKey === trait.inGameKey);
      if (add && countItemInstance < 1) {
        if (existingTraitIndex !== -1) {
          updatedTraits[existingTraitIndex] = {
            ...updatedTraits[existingTraitIndex],
            value: updatedTraits[existingTraitIndex].value! + 1,
            tier: generateTier(updatedTraits[existingTraitIndex].tiers, updatedTraits[existingTraitIndex].value),
          };
        } else {
          updatedTraits.push({
            value: 1,
            name: trait.name,
            inGameKey: trait.inGameKey,
            tierString: trait.tierString as string,
            tier: 0,
            tiers: trait.tiers as Tier[],
          });
        }
      } else if (!add && countItemInstance === 1) {
        if (existingTraitIndex !== -1) {
          updatedTraits[existingTraitIndex].value! -= 1;
          if (updatedTraits[existingTraitIndex].value === 0) {
            updatedTraits.splice(existingTraitIndex, 1);
          }
        }
      }
    });
    setBoardTraits(updatedTraits);
  };

  const canAddItemToUnit = (item: Item, index: number) => {
    if (!isUnit(hexes[index].unit)) return false;
    let unit = hexes[index].unit as Unit;
    if (hexes[index].currentItems.length == 3) {
      dispatch(showError(["Each unit can only hold 3 items"]));
      return false;
    }
    if (
      unit.IsItemIncompatible &&
      unit.compatabilityType === "trait" &&
      !unit.traits.some((trait) => trait.name === item.traitCompatiblityKey)
    ) {
      dispatch(showError([`Only trait-compatible items can be added to ${unit.name}`]));
      return false;
    }
    if (unit.IsItemIncompatible && unit.compatabilityType === "unit" && item.unitCompatiblityKey !== unit.name) {
      dispatch(showError([`Only unit-compatible items can be added to ${unit.name}`]));
      return false;
    }
    if (item.traitCompatiblityKey && unit.traits.some((trait) => trait.name === item.traitCompatiblityKey)) {
      dispatch(showError([`Can't add a ${item.traitCompatiblityKey} item to ${unit.name}`]));
      return false;
    }
    if (item.unitCompatiblityKey && unit.name !== item.unitCompatiblityKey) {
      dispatch(showError([`Can't add a ${item.unitCompatiblityKey} item to ${unit.name}`]));
      return false;
    }
    if (hexes[index].currentItems.includes(item) && item.tags.includes("unique")) {
      dispatch(showError([`Only one ${item.name} can be added to ${unit.name}`]));
      return false;
    }
    const itemCount = hexes.reduce((count, hex) => {
      const validItems = hex.currentItems.filter((item) => !item.traitCompatiblityKey && !item.unitCompatiblityKey);
      return count + validItems.length;
    }, 0);
    if (itemCount >= 10 && !unit.IsItemIncompatible) {
      dispatch(showError(["A maximum of 10 items can be used in a build excluding Unit and Trait specific items"]));
      return false;
    }
    return true;
  };

  const canAddUnitToBoard = (unit: Unit, updatedHexes: HexItem[]) => {
    const triggerUnits = updatedHexes.filter((hex) => hex.unit?.isTriggerUnit);
    const maxUnits = triggerUnits.length ? 10 + triggerUnits.length : 10;
    if (updatedHexes.filter((hex) => hex.unit).length >= maxUnits) {
      dispatch(showError(["The maximum amount of units in a build is 10"]));
      return false;
    }
    const unitCount = updatedHexes.filter((hex) => hex.unit?.name === unit.name).length;
    if (unitCount >= 2) {
      dispatch(showError([`${unit.name} can't appear more than twice`]));
      return false;
    }
    if (unit.isTriggerUnit && triggerUnits.some((hexItem) => hexItem.unit?.inGameKey == unit.inGameKey)) {
      dispatch(showError([`${unit.name} is already present in the build.`]));
      return false;
    }
    return true;
  };

  const updateTraitForItem = (draggedItem: Item, index: number) => {
    if (draggedItem.affectedTraitKey) {
      if (hexes[index].unit?.traits.some((trait: Trait) => trait.name === draggedItem.affectedTraitKey)) {
        dispatch(showError([`${hexes[index].unit?.name} is already ${draggedItem.affectedTraitKey}`]));
        return;
      }
      const updatedTraits = [...boardTraits];
      const existingTraitIndex = updatedTraits.findIndex(
        (traitItem) => traitItem.name === draggedItem.affectedTraitKey
      );
      if (existingTraitIndex !== -1) {
        updatedTraits[existingTraitIndex] = {
          ...updatedTraits[existingTraitIndex],
          value: updatedTraits[existingTraitIndex].value! + 1,
          tier: generateTier(updatedTraits[existingTraitIndex].tiers, updatedTraits[existingTraitIndex].value),
        };
        setBoardTraits(updatedTraits);
      } else {
        dispatch(showError([`Please add at least one ${draggedItem.affectedTraitKey} before adding an emblem`]));
        return;
      }
    }
  };

  const handleDrop = (index: number) => {
    if (!draggedItem) return;
    const updatedHexes = [...hexes];
    if (isUnit(draggedItem)) {
      if (draggedIndex !== null && draggedIndex !== index) {
        [updatedHexes[index], updatedHexes[draggedIndex]] = [updatedHexes[draggedIndex], updatedHexes[index]];
        updatedHexes[index].coordinates = index;
        updatedHexes[draggedIndex].coordinates = updatedHexes[draggedIndex].unit ? draggedIndex : null;
      } else if (draggedIndex !== index && canAddUnitToBoard(draggedItem, updatedHexes)) {
        updatedHexes[index] = {
          unit: draggedItem,
          currentItems: [],
          isStarred: false,
          coordinates: index,
        };
        updateBoardTraits(draggedItem, true);
      }
    } else if (updatedHexes[index].unit !== null && isItem(draggedItem)) {
      if (canAddItemToUnit(draggedItem, index)) {
        updateTraitForItem(draggedItem, index);
        hexes[index].currentItems.push(draggedItem);
      }
    }
    setHexes(updatedHexes);
    resetDrag();
  };

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
  };

  const handleDragStart = (item: Unit | null, index: number) => {
    dispatch(setDraggedItem(item));
    setDraggedIndex(index);
  };

  const handleToggleStars = (index: number) => {
    const updatedHexes = [...hexes];
    updatedHexes[index].isStarred = !updatedHexes[index].isStarred;
    setHexes(updatedHexes);
  };

  const handleDragEnd = () => {
    if (draggedItem && draggedIndex !== null) {
      const updatedHexes = [...hexes];
      updatedHexes[draggedIndex] = {
        unit: null,
        currentItems: [],
        isStarred: false,
        coordinates: null,
      };
      setHexes(updatedHexes);
      updateBoardTraits(draggedItem as Unit, false);
      resetDrag();
    }
  };

  const resetDrag = () => {
    setDraggedItem(null);
    setDraggedIndex(null);
  };

  return (
    <div className="app__board">
      <div className="app__board-container">
        {hexes.map((hex, index) => (
          <Hex
            key={index}
            hex={hex}
            index={index}
            onDrop={handleDrop}
            onDragOver={handleDragOver}
            onDragStart={handleDragStart}
            onDragEnd={handleDragEnd}
            onToggleStars={handleToggleStars}
          />
        ))}
      </div>
    </div>
  );
};

interface HexProps {
  hex: HexItem;
  index: number;
  onDrop: (index: number) => void;
  onDragOver: (e: React.DragEvent<HTMLDivElement>) => void;
  onDragStart: (unit: Unit | null, index: number) => void;
  onDragEnd: () => void;
  onToggleStars: (index: number) => void;
}

const Hex = ({ hex, index, onDrop, onDragOver, onDragStart, onDragEnd, onToggleStars }: HexProps) => (
  <div
    className="app__board-hex"
    onDrop={() => onDrop(index)}
    onDragOver={onDragOver}
    onDragEnd={onDragEnd}
    onDragStart={() => onDragStart(hex.unit, index)}
    draggable={!!hex.unit}
  >
    {hex.unit && !hex.unit.isTriggerUnit && (
      <div className={`app__board-hex-stars ${hex.isStarred ? "starred" : ""}`} onClick={() => onToggleStars(index)}>
        {[...Array(3)].map((_, idx) => (
          <div key={idx} className="app__board-hex-stars-star">
            <img
              src={hex.isStarred ? "/images/general/star-gold.svg" : "/images/general/star-gray.svg"}
              alt={hex.isStarred ? "gold star" : "gray star"}
            />
          </div>
        ))}
      </div>
    )}
    <HexagonSVG
      imageUrl={hex.unit ? getImageUrl(hex.unit.inGameKey, "champions", "tiles") : undefined}
      cost={hex.unit ? hex.unit.cost[0] : undefined}
    />
    {hex.currentItems.length > 0 && (
      <div className="app__board-hex-items">
        {hex.currentItems.map((item, idx) => (
          <div key={idx} className="app__board-hex-items-item">
            <img src={getImageUrl(item.inGameKey, "items")} alt={item.name} />
          </div>
        ))}
      </div>
    )}
  </div>
);

export default Board;
