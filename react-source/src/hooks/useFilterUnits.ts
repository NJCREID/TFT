import { useEffect, useState } from "react";
import fetchRequest from "../common/api";
import { ENDPOINT } from "../common/endpoints";
import { useDispatch } from "react-redux";
import { showError } from "../store/errorModalSlice";
import { Unit } from "../data";

export default function useFilterUnits(endpoint?: string) {
  const [allUnits, setAllUnits] = useState<Unit[] | null>(null);
  const [filteredUnits, setFilteredUnits] = useState<Unit[] | null>(null);
  const [unitQuery, setUnitQuery] = useState("");
  const [azGoldFilter, setAzGoldFilter] = useState<string | null>("cost");
  const [tierFilter, setTierFilter] = useState<string | null>(null);
  const [traitFilter, setTraitFilter] = useState<string | null>(null);
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchUnits = async () => {
      try {
        setAllUnits(null);
        let units = await fetchRequest<Unit[]>({
          endpoint: endpoint || ENDPOINT.UNIT_PARTIAL_FETCH,
        });
        setAllUnits(units);
      } catch (error) {
        setAllUnits([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching units: ${error.message}`]));
        } else {
          dispatch(showError(["An Unkown Error occured while fetching units."]));
        }
      }
    };
    fetchUnits();
  }, []);

  useEffect(() => {
    if (!allUnits) return;
    let tempFilteredUnits = allUnits.filter((unit) => unit.name.toLowerCase().includes(unitQuery.toLowerCase()));

    if (azGoldFilter === "A-Z") {
      tempFilteredUnits = tempFilteredUnits.sort((a, b) => a.name.localeCompare(b.name));
    } else if (azGoldFilter === "cost") {
      tempFilteredUnits = tempFilteredUnits.sort((a, b) => a.cost[0] - b.cost[0]);
    }

    if (tierFilter) {
      tempFilteredUnits = tempFilteredUnits.filter((unit) => unit.tier.toString() === tierFilter);
    }

    if (traitFilter) {
      tempFilteredUnits = tempFilteredUnits.filter((unit) =>
        unit.traits.some((trait) => trait.inGameKey === traitFilter)
      );
    }

    setFilteredUnits(tempFilteredUnits);
  }, [unitQuery, azGoldFilter, tierFilter, traitFilter, allUnits]);

  return {
    units: filteredUnits,
    unitQuery,
    setUnitQuery,
    setAzGoldFilter,
    setTierFilter,
    setTraitFilter,
  };
}
