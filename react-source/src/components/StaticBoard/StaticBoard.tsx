import { useEffect, useState } from "react";
import HexagonSVG from "../HexagonSVG/HexagonSVG";
import { Item, Unit } from "../../data";
import "./staticboard.css";
import { getImageUrl } from "../../utils";

interface BoardProps {
  units: HexItem[];
}

interface HexItem {
  unit: Unit | null;
  currentItems: Item[];
  isStarred: boolean;
  coordinates: number | null;
}

const StaticBoard = ({ units }: BoardProps) => {
  const [hexes, setHexes] = useState<HexItem[]>(
    Array(28).fill({
      unit: null,
      currentItems: [],
      starred: false,
      coordinates: null,
    })
  );

  useEffect(() => {
    if (units) {
      const updatedHexes: HexItem[] = hexes.map((_, index) => {
        const initialUnit = units.find((unit) => unit.coordinates === index);
        return {
          unit: initialUnit?.unit || null,
          currentItems: initialUnit?.currentItems || [],
          isStarred: initialUnit?.isStarred || false,
          coordinates: initialUnit?.coordinates || null,
        };
      });
      setHexes(updatedHexes);
    }
  }, []);

  return (
    <div className="app__staticboard">
      <div className="app__staticboard-container" style={{ "--width": "13.26530612244898%" } as React.CSSProperties}>
        {hexes.map((hex, index) => (
          <div key={index} className="app__staticboard-hex">
            {hex.unit && (
              <div className={`app__staticboard-hex-stars ${hex.isStarred ? "starred" : ""}`}>
                {[...Array(3)].map((_, index) => (
                  <div key={index} className="app__staticboard-hex-stars-star">
                    <img
                      src={hex.isStarred ? "/images/general/star-gold.svg" : "/images/general/star-gray.svg"}
                      alt={hex.isStarred ? "gold star" : "gray star"}
                    />
                  </div>
                ))}
              </div>
            )}
            <HexagonSVG
              imageUrl={hex.unit ? getImageUrl((hex.unit as Unit).inGameKey, "champions", "tiles") : undefined}
              cost={hex.unit ? (hex.unit as Unit).cost[0] : undefined}
            />
            {hex.currentItems.length > 0 && (
              <div className="app__staticboard-hex-items">
                {hex.currentItems.map((item, idx) => (
                  <div key={idx} className="app__staticboard-hex-items-item">
                    <img src={getImageUrl(item.inGameKey, "items")} alt={item.name} />
                  </div>
                ))}
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
};
export default StaticBoard;
