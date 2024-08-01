import { useEffect, useState } from "react";
import fetchRequest, { Unit } from "../common/api";
import { ENDPOINT } from "../common/endpoints";

export default function useFilterUnits() {
  const [allUnits, setAllUnits] = useState<Unit[] | null>(null);
  const [filteredUnits, setFilteredUnits] = useState<Unit[] | null>(null);
  const [unitQuery, setUnitQuery] = useState("");
  const [azGoldFilter, setAzGoldFilter] = useState<string | null>("cost");
  const [tierFilter, setTierFilter] = useState<string | null>(null);
  const [traitFilter, setTraitFilter] = useState<string | null>(null);

  useEffect(() => {
    const fetchUnits = async () => {
      try {
        let units = await fetchRequest<Unit[]>(ENDPOINT.UNIT_FETCH);
        setAllUnits(units);
      } catch (error) {
        console.error("Error fetching units:", error);
      }
    };
    fetchUnits();
  }, []);

  useEffect(() => {
    if (!allUnits) return;
    let tempFilteredUnits = allUnits.filter((unit) => unit.name.toLowerCase().includes(unitQuery.toLowerCase()));

    if (azGoldFilter === "A-Z") {
      tempFilteredUnits = tempFilteredUnits.sort((a, b) => a.key.localeCompare(b.key));
    } else if (azGoldFilter === "cost") {
      tempFilteredUnits = tempFilteredUnits.sort((a, b) => a.cost[0] - b.cost[0]);
    }

    if (tierFilter) {
      tempFilteredUnits = tempFilteredUnits.filter((unit) => unit.tier.toString() === tierFilter);
    }

    if (traitFilter) {
      tempFilteredUnits = tempFilteredUnits.filter((unit) => unit.traits.some((trait) => trait.key === traitFilter));
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
