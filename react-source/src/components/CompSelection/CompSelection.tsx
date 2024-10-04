import { memo, useState } from "react";
import "./compselection.css";
import { Item, Unit } from "../../data";
import UnitFilters from "../UnitFilters/UnitFilters";
import ItemFilters from "../ItemFilters/ItemFilters";
import SwitchSlider from "../SwitchSlider/SwitchSlider";
import Selector from "../Selector/Selector";
import { ENDPOINT } from "../../common/endpoints";

const CompSelection = () => {
  const [unitsSelected, setUnitsSelected] = useState(true);
  const [units, setUnits] = useState<Unit[] | null>(null);
  const [items, setItems] = useState<Item[] | null>(null);

  const handleUnitsChange = (units: Unit[] | null) => {
    setUnits(units);
  };

  const handleItemsChange = (items: Item[] | null) => {
    setItems(items);
  };

  const handleToggleFilters = () => {
    setUnitsSelected((prev) => !prev);
  };

  return (
    <div className="app__compselection">
      <div className="app__compselection-filters">
        {unitsSelected ? (
          <UnitFilters onUnitsChange={handleUnitsChange} endpoint={ENDPOINT.UNIT_PARTIAL_FETCH} />
        ) : (
          <ItemFilters onItemsChange={handleItemsChange} endpoint={ENDPOINT.ITEM_FULL_FETCH} />
        )}
        <SwitchSlider type1="Units" type2="Items" setAction={handleToggleFilters} />
      </div>
      <div className="app__compselection-container">
        {unitsSelected ? <Selector data={units} /> : items && <Selector data={items} />}
      </div>
    </div>
  );
};
export default memo(CompSelection);
