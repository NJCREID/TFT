import SearchBar from "../SearchBar/SearchBar";
import SwitchSlider from "../SwitchSlider/SwitchSlider";
import DropDownSelection from "../DropDownSelection/DropDownSelection";
import FilterSelector from "../FilterSelector/FilterSelector";
import { useCallback, useEffect, useState } from "react";
import useFilterUnits from "../../hooks/useFilterUnits";
import useFilterItems from "../../hooks/useFilterItems";
import allOption from "../../assets/all-option.svg";
import coin from "../../assets/coin.svg";
import "./compselection.css";
import { ITEM_TYPES } from "../../data/ItemData";
import Selector from "../Selector/Selector";
import { Item, Trait, fetchComponents, fetchTraits } from "../../common/api";
import { useWindowDimensions } from "../../context/windowDimensionContext";

const unitTiers = [
  { key: "all", name: "all", imageUrl: allOption },
  { key: "1", name: "1", type: "text" },
  { key: "2", name: "2", type: "text" },
  { key: "3", name: "3", type: "text" },
  { key: "4", name: "4", type: "text" },
  { key: "5", name: "5", type: "text" },
];
const AZGoldItems = [
  { key: "cost", name: "cost", imageUrl: coin },
  { key: "A-Z", name: "A-Z", type: "text" },
];

const CompSelection = () => {
  const [components, setComponents] = useState<Item[]>([]);
  const [traits, setTraits] = useState<Trait[]>([]);
  const [unitsSelected, setUnitsSelected] = useState(true);
  const [loading, setLoading] = useState(true);
  const { items, itemQuery, setItemQuery, setComponentFilter, setItemTypeFilter } = useFilterItems();
  const { units, unitQuery, setUnitQuery, setAzGoldFilter, setTierFilter, setTraitFilter } = useFilterUnits();
  const { width } = useWindowDimensions();

  useEffect(() => {
    const fetchItems = async () => {
      try {
        setComponents(await fetchComponents());
        setTraits(await fetchTraits());
        setLoading(false);
      } catch (error) {
        console.error("Error fetching items:", error);
      }
    };
    fetchItems();
  }, []);

  const handleUnitQueryChange = useCallback(
    (value: string) => {
      setUnitQuery(value);
    },
    [setUnitQuery]
  );

  const handleAzGoldSelect = useCallback(
    (AZGold: string | null) => {
      setAzGoldFilter(AZGold);
    },
    [setAzGoldFilter]
  );

  const handleTierSelect = useCallback(
    (tier: string | null) => {
      setTierFilter(tier === "all" ? null : tier);
    },
    [setTierFilter]
  );

  const handleTraitSelect = useCallback(
    (trait: string) => {
      setTraitFilter(trait === "all" ? null : trait);
    },
    [setTraitFilter]
  );

  const handleItemQueryChange = useCallback(
    (value: string) => {
      setItemQuery(value);
    },
    [setItemQuery]
  );

  const handleComponentSelect = useCallback(
    (component: string | null) => {
      setComponentFilter(component === "all" ? null : component);
    },
    [setComponentFilter]
  );

  const handleItemTypeSelect = useCallback(
    (itemtype: string) => {
      setItemTypeFilter(itemtype === "all" ? null : itemtype);
    },
    [setItemTypeFilter]
  );

  const handleToggleFilters = () => {
    if (unitsSelected) {
      setUnitQuery("");
      setAzGoldFilter("cost");
      setTierFilter(null);
      setTraitFilter(null);
    } else {
      setItemQuery("");
      setComponentFilter(null);
      setItemTypeFilter(null);
    }
    setUnitsSelected((prev) => !prev);
  };

  return (
    <div className="app__compselection">
      {!loading && (
        <>
          <div className="app__compselection-filters">
            {unitsSelected ? (
              <>
                <SearchBar placeHolder="Search by unit name..." value={unitQuery} onChange={handleUnitQueryChange} />
                <FilterSelector items={AZGoldItems} onSelect={handleAzGoldSelect} />
                <FilterSelector items={unitTiers} onSelect={handleTierSelect} />
                <DropDownSelection items={traits} onSelect={handleTraitSelect} />
              </>
            ) : (
              <>
                <SearchBar placeHolder="Search by item name..." value={itemQuery} onChange={handleItemQueryChange} />
                {width > 370 && (
                  <FilterSelector items={components} onSelect={handleComponentSelect} selectFirst={true} />
                )}
                <DropDownSelection items={ITEM_TYPES} onSelect={handleItemTypeSelect} />
              </>
            )}
            <SwitchSlider type1="Champions" type2="Items" setAction={handleToggleFilters} />
          </div>
          <div className="app__compselection-container">
            {unitsSelected && units ? <Selector data={units} /> : items && <Selector data={items} />}
          </div>
        </>
      )}
    </div>
  );
};
export default CompSelection;
