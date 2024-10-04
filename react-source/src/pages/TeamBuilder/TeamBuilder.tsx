import { useState } from "react";
import "./teambuilder.css";
import { MdDeleteOutline } from "react-icons/md";
import { PostForm, TraitsDisplay, CompSelection, Board, Button } from "../../components";
import { UserGuideTrait, Unit, Item } from "../../data";

export interface HexItem {
  unit: Unit | null;
  currentItems: Item[];
  isStarred: boolean;
  coordinates: number | null;
}

export default function TeamBuilder() {
  const [boardTraits, setBoardTraits] = useState<UserGuideTrait[]>([]);
  const [hexes, setHexes] = useState<HexItem[]>(
    Array(28).fill({
      unit: null,
      currentItems: [],
      isStarred: false,
      coordinates: null,
    })
  );
  const handleClearBoard = () => {
    setHexes(
      Array(28).fill({
        unit: null,
        currentItems: [],
        isStarred: false,
        coordinates: null,
      })
    );
    setBoardTraits([]);
  };
  return (
    <div className="app__teambuilder page_padding">
      <div className="app__teambuilder-buttons">
        <Button onClick={handleClearBoard} aria-label="Clear all units from the board">
          <MdDeleteOutline size={15} />
          <p className="p__opensans">Clear Board</p>
        </Button>
      </div>
      <div className="app__teambuilder-container">
        <TraitsDisplay userGuideTraits={boardTraits} />
        <Board boardTraits={boardTraits} setBoardTraits={setBoardTraits} hexes={hexes} setHexes={setHexes} />
        <div />
      </div>
      <CompSelection />
      <div className="app__teambuilder-form">
        <PostForm traits={boardTraits} hexes={hexes} />
      </div>
    </div>
  );
}
