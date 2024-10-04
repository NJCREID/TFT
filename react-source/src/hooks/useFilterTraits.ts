import { useEffect, useState } from "react";
import fetchRequest from "../common/api";
import { useDispatch } from "react-redux";
import { showError } from "../store/errorModalSlice";
import { ENDPOINT } from "../common/endpoints";
import { Trait } from "../data";

export function useFilterTraits() {
  const [allTraits, setAllTraits] = useState<Trait[] | null>(null);
  const [filteredTraits, setFilteredTraits] = useState<Trait[] | null>(null);
  const [traitQuery, setTraitQuery] = useState("");
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchTraits = async () => {
      try {
        let traits = await fetchRequest<Trait[]>({
          endpoint: ENDPOINT.TRAIT_FETCH,
        });
        setAllTraits(traits);
      } catch (error) {
        setAllTraits([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching traits: ${error.message}`]));
        } else {
          dispatch(showError(["An unknown error occurred while fetching traits."]));
        }
      }
    };
    fetchTraits();
  }, []);

  useEffect(() => {
    if (!allTraits) return;
    let tempFilteredTraits = allTraits.filter((trait) => trait.name.toLowerCase().includes(traitQuery.toLowerCase()));
    setFilteredTraits(tempFilteredTraits);
  }, [traitQuery, allTraits]);

  return {
    traits: filteredTraits,
    traitQuery,
    setTraitQuery,
  };
}
