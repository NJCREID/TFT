import { useState } from "react";
import HexagonSVG from "../HexagonSVG/HexagonSVG";
import starGray from "../../assets/star-gray.svg";
import starGold from "../../assets/star-gold.svg";
import { Unit, UserGuideTrait } from "../../common/api";
import GenerateTier from "../../utils/GenerateTier";
import { HexItem } from "../../pages/TeamBuilder/TeamBuilder";
import { isItem, isUnit } from "../../utils/isObject";
import "./board.css";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../../store";
import { setDraggedItem } from "../../store/dragSlice";
import { showError } from "../../store/errorModalSlice";

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
      const existingTraitIndex = updatedTraits.findIndex((traitItem) => traitItem.trait.key === trait.key);
      if (add && countItemInstance < 1) {
        if (existingTraitIndex !== -1) {
          updatedTraits[existingTraitIndex] = {
            ...updatedTraits[existingTraitIndex],
            value: updatedTraits[existingTraitIndex].value! + 1,
            tier: GenerateTier(updatedTraits[existingTraitIndex].trait.name, updatedTraits[existingTraitIndex].value),
          };
        } else {
          updatedTraits.push({
            value: 1,
            trait: { ...trait },
            tier: 0,
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

  const handleDrop = (index: number) => {
    if (!draggedItem) return;
    const updatedHexes = [...hexes];
    const isKaylePresent = updatedHexes.some((hex) => hex.unit?.name.toLowerCase() === "kayle");
    if (isUnit(draggedItem)) {
      if (draggedIndex !== null && draggedIndex !== index) {
        [updatedHexes[index], updatedHexes[draggedIndex]] = [updatedHexes[draggedIndex], updatedHexes[index]];
        updatedHexes[index].coordinates = index;
        updatedHexes[draggedIndex].coordinates = updatedHexes[draggedIndex].unit ? draggedIndex : null;
      } else if (draggedIndex !== index) {
        const maxUnits = isKaylePresent ? 11 : 10;
        if (updatedHexes.filter((hex) => hex.unit).length >= maxUnits) {
          dispatch(showError(["The maximum amount of units in a build is 10"]));
          return;
        }
        const unitCount = updatedHexes.filter((hex) => hex.unit?.name === draggedItem.name).length;
        if (unitCount >= 2) {
          dispatch(showError([`${draggedItem.name} can't appear more than twice`]));
          return;
        }
        if (draggedItem.name.toLowerCase() === "kayle" && isKaylePresent) {
          dispatch(showError(["Kayle is already present in the build."]));
          return;
        }
        updatedHexes[index] = {
          unit: draggedItem,
          currentItems: [],
          isStarred: false,
          coordinates: index,
        };
        updateBoardTraits(draggedItem, true);
      }
    } else if (
      updatedHexes[index].unit !== null &&
      updatedHexes[index].currentItems.length < 3 &&
      isItem(draggedItem)
    ) {
      if (updatedHexes[index].unit?.name.toLowerCase() == "kayle" && !draggedItem.tags.includes("storyweaver")) {
        dispatch(showError(["Only storyweaver item's can be added to kayle"]));
        return;
      }
      if (draggedItem.tags.includes("storyweaver") && !(updatedHexes[index].unit?.name.toLowerCase() === "kayle")) {
        dispatch(showError(["Can only add storyweaver item's to kayle"]));
        return;
      }
      if (updatedHexes[index].currentItems.includes(draggedItem) && draggedItem.tags.includes("unique")) {
        dispatch(showError([`Only one ${draggedItem.name} can be added to ${updatedHexes[index].unit?.name}`]));
        return;
      }
      const itemCount = updatedHexes
        .filter((hex) => hex.unit?.name.toLowerCase() !== "kayle")
        .reduce((count, hex) => count + hex.currentItems.length, 0);
      if (itemCount >= 10 && updatedHexes[index].unit?.name.toLowerCase() !== "kayle") {
        dispatch(showError(["A maximum of 10 item's can be used in a build excluding kayle"]));
        return;
      }
      if (draggedItem.affectedTraitKey) {
        if (updatedHexes[index].unit?.traits.some((trait) => trait.name === draggedItem.affectedTraitKey)) {
          dispatch(showError([`${updatedHexes[index].unit?.name} is already ${draggedItem.affectedTraitKey}`]));
          return;
        }
        updatedHexes[index].unit?.traits.map((trait) => {
          console.log(trait.name);
          console.log(draggedItem.affectedTraitKey);
        });
        const updatedTraits = [...boardTraits];
        const existingTraitIndex = updatedTraits.findIndex(
          (traitItem) => traitItem.trait.name === draggedItem.affectedTraitKey
        );
        if (existingTraitIndex !== -1) {
          updatedTraits[existingTraitIndex] = {
            ...updatedTraits[existingTraitIndex],
            value: updatedTraits[existingTraitIndex].value! + 1,
            tier: GenerateTier(updatedTraits[existingTraitIndex].trait.name, updatedTraits[existingTraitIndex].value),
          };
          setBoardTraits(updatedTraits);
        } else {
          dispatch(showError([`Please add at least one ${draggedItem.affectedTraitKey} before adding an emblem`]));
          return;
        }
      }
      updatedHexes[index].currentItems?.push(draggedItem);
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
    {hex.unit && hex.unit.name.toLowerCase() !== "kayle" && (
      <div className={`app__board-hex-stars ${hex.isStarred ? "starred" : ""}`} onClick={() => onToggleStars(index)}>
        {[...Array(3)].map((_, idx) => (
          <div key={idx} className="app__board-hex-stars-star">
            <img src={hex.isStarred ? starGold : starGray} alt={hex.isStarred ? "gold star" : "gray star"} />
          </div>
        ))}
      </div>
    )}
    <HexagonSVG imageUrl={hex.unit ? hex.unit.imageUrl : undefined} cost={hex.unit ? hex.unit.cost[0] : undefined} />
    {hex.currentItems.length > 0 && (
      <div className="app__board-hex-items">
        {hex.currentItems.map((item, idx) => (
          <div key={idx} className="app__board-hex-items-item">
            <img src={item.imageUrl} alt={item.name} />
          </div>
        ))}
      </div>
    )}
  </div>
);

export default Board;
