import { useEffect, useState } from "react";
import fetchRequest from "../common/api";
import { ENDPOINT } from "../common/endpoints";
import { useDispatch } from "react-redux";
import { showError } from "../store/errorModalSlice";
import { Augment } from "../data";

export default function useFilterAugments() {
  const [allAugments, setAllAugments] = useState<Augment[] | null>(null);
  const [filteredAugments, setFilteredAugments] = useState<Augment[] | null>(null);
  const [augmentQuery, setAugmentQuery] = useState("");
  const [tierFilter, setTierFilter] = useState<string | null>(null);
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchAugments = async () => {
      try {
        setAllAugments(null);
        let augments = await fetchRequest<Augment[]>({
          endpoint: ENDPOINT.AUGMENT_FETCH,
        });
        setAllAugments(augments);
      } catch (error) {
        setAllAugments([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching augments: ${error.message}`]));
        } else {
          dispatch(showError(["An unknown error occurred while fetching augments."]));
        }
      }
    };
    fetchAugments();
  }, []);

  useEffect(() => {
    if (!allAugments) return;
    let tempFilteredAugments = allAugments.filter((augment) =>
      augment.name.toLowerCase().includes(augmentQuery.toLowerCase())
    );
    if (tierFilter && tierFilter != "all") {
      tempFilteredAugments = tempFilteredAugments.filter((augment) => tierFilter.includes(augment.tier.toString()));
    }
    setFilteredAugments(tempFilteredAugments);
  }, [augmentQuery, tierFilter, allAugments]);

  return {
    augments: filteredAugments,
    augmentQuery,
    setAugmentQuery,
    setTierFilter,
  };
}
