import { useEffect, useState } from "react";
import fetchRequest, { Item } from "../common/api";
import { ENDPOINT } from "../common/endpoints";

export default function useFilterItems() {
  const [allItems, setAllItems] = useState<Item[] | null>(null);
  const [filteredItems, setFilteredItems] = useState<Item[] | null>(null);
  const [itemQuery, setItemQuery] = useState("");
  const [componentFilter, setComponentFilter] = useState<string | null>(null);
  const [itemTypeFilter, setItemTypeFilter] = useState<string | null>(null);

  useEffect(() => {
    const fetchItems = async () => {
      try {
        let items = await fetchRequest<Item[]>(ENDPOINT.ITEM_FETCH);
        setAllItems(items);
      } catch (error) {
        console.error("Error fetching items:", error);
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
