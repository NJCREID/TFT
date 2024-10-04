import { useCallback, useEffect, useState } from "react";
import { useWindowDimensions } from "../../context";
import DropDownSelection from "../DropDownSelection/DropDownSelection";
import FilterSelector from "../FilterSelector/FilterSelector";
import SearchBar from "../SearchBar/SearchBar";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import { Item, ITEM_TYPES } from "../../data/ItemData";
import { useFilterItems } from "../../hooks";
import { ENDPOINT } from "../../common/endpoints";
import fetchRequest from "../../common/api";
import "./itemfilters.css";

interface ItemFiltersProps {
  onItemsChange: (items: Item[] | null) => void;
  endpoint: string;
}

const ItemFilters = ({ onItemsChange, endpoint }: ItemFiltersProps) => {
  const { items, itemQuery, setItemQuery, setComponentFilter, setItemTypeFilter } = useFilterItems(endpoint);
  const [components, setComponents] = useState<Item[]>([]);
  const { width } = useWindowDimensions();
  const dispatch = useDispatch();

  useEffect(() => {
    onItemsChange(items);
  }, [items, onItemsChange]);

  useEffect(() => {
    const fetchItemComponents = async () => {
      try {
        let fetchedComponents = await fetchRequest<Item[]>({
          endpoint: ENDPOINT.COMPONENT_FETCH,
        });

        setComponents([
          { inGameKey: "", key: "all", name: "all", recipe: [], desc: "", tags: [] },
          ...fetchedComponents,
        ]);
      } catch (error) {
        if (error instanceof Error) {
          dispatch(showError([`Error fetching components: ${error.message}`]));
        } else {
          dispatch(showError([`An unkown error occured while fetching components.`]));
        }
      }
    };
    fetchItemComponents();
  }, []);

  const handleItemQueryChange = useCallback((value: string) => {
    setItemQuery(value);
  }, []);

  const handleComponentSelect = useCallback((component: string | null) => {
    setComponentFilter(component === "all" ? null : component);
  }, []);

  const handleItemTypeSelect = useCallback((itemtype: string | null) => {
    setItemTypeFilter(itemtype === "all" ? null : itemtype);
  }, []);

  return (
    <>
      <SearchBar placeHolder="Search by item name..." value={itemQuery} onChange={handleItemQueryChange} />
      {width > 370 && <FilterSelector items={components} onSelect={handleComponentSelect} />}
      <DropDownSelection items={ITEM_TYPES} onSelect={handleItemTypeSelect} allDisplayName="All types" includeAll />
    </>
  );
};

export default ItemFilters;
