import { useEffect, useState } from "react";
import fetchRequest from "../common/api";
import { ENDPOINT } from "../common/endpoints";
import { useDispatch } from "react-redux";
import { showError } from "../store/errorModalSlice";
import { Item } from "../data";

export default function useFilterItems(endpoint?: string) {
  const [allItems, setAllItems] = useState<Item[] | null>(null);
  const [filteredItems, setFilteredItems] = useState<Item[] | null>(null);
  const [itemQuery, setItemQuery] = useState("");
  const [componentFilter, setComponentFilter] = useState<string | null>(null);
  const [itemTypeFilter, setItemTypeFilter] = useState<string | null>(null);
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchItems = async () => {
      try {
        setAllItems(null);
        let items = await fetchRequest<Item[]>({
          endpoint: endpoint || ENDPOINT.ITEM_PARTIAL_FETCH,
        });
        setAllItems(items);
      } catch (error) {
        setAllItems([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching items: ${error.message}`]));
        } else {
          dispatch(showError(["An Unkown Error occured while fetching."]));
        }
      }
    };
    fetchItems();
  }, []);

  useEffect(() => {
    if (!allItems) return;
    let tempFilteredItems = allItems.filter((item) => item.name.toLowerCase().includes(itemQuery.toLowerCase()));
    if (componentFilter) {
      tempFilteredItems = tempFilteredItems.filter((item) => item.recipe?.includes(componentFilter));
    }
    if (itemTypeFilter) {
      tempFilteredItems = tempFilteredItems.filter((item) =>
        item.tags?.includes(itemTypeFilter.toLowerCase().replace(" ", ""))
      );
    }
    setFilteredItems(tempFilteredItems);
  }, [itemQuery, componentFilter, itemTypeFilter, allItems]);

  return {
    items: filteredItems,
    itemQuery,
    setItemQuery,
    setComponentFilter,
    setItemTypeFilter,
  };
}
