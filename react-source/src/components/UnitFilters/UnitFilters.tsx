import DropDownSelection from "../DropDownSelection/DropDownSelection";
import FilterSelector from "../FilterSelector/FilterSelector";
import SearchBar from "../SearchBar/SearchBar";
import { useCallback, useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import { AZGold, Unit, unitTiers } from "../../data/UnitData";
import "./unitfilters.css";
import { useFilterUnits } from "../../hooks";
import { Trait } from "../../data";
import fetchRequest from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";

interface UnitFiltersProps {
  onUnitsChange: (units: Unit[] | null) => void;
  endpoint: string;
}

const UnitFilters = ({ onUnitsChange, endpoint }: UnitFiltersProps) => {
  const { units, unitQuery, setUnitQuery, setAzGoldFilter, setTierFilter, setTraitFilter } = useFilterUnits(endpoint);
  const [traits, setTraits] = useState<Trait[]>([]);
  const dispatch = useDispatch();

  useEffect(() => {
    onUnitsChange(units);
  }, [units, onUnitsChange]);

  useEffect(() => {
    const fetchTraitsAsync = async () => {
      try {
        let fetchedTraits = await fetchRequest<Trait[]>({
          endpoint: ENDPOINT.TRAIT_FETCH,
        });
        setTraits(fetchedTraits);
      } catch (error) {
        if (error instanceof Error) {
          dispatch(showError([`Error fetching traits: ${error.message}`]));
        } else {
          dispatch(showError([`An unkown error occured while fetching traits.`]));
        }
      }
    };
    fetchTraitsAsync();
  }, []);

  const handleUnitQueryChange = useCallback((value: string) => {
    setUnitQuery(value);
  }, []);

  const handleAzGoldSelect = useCallback((AZGold: string | null) => {
    setAzGoldFilter(AZGold);
  }, []);

  const handleTierSelect = useCallback((tier: string | null) => {
    setTierFilter(tier === "all" ? null : tier);
  }, []);

  const handleTraitSelect = useCallback((trait: string | null) => {
    setTraitFilter(trait === "all" ? null : trait);
  }, []);

  return (
    <>
      <SearchBar placeHolder="Search by unit name..." value={unitQuery} onChange={handleUnitQueryChange} />
      <FilterSelector items={AZGold} onSelect={handleAzGoldSelect} />
      <FilterSelector items={unitTiers} onSelect={handleTierSelect} />
      <DropDownSelection
        items={traits}
        onSelect={handleTraitSelect}
        allDisplayName="All Synergies"
        includeImages
        includeAll
      />
    </>
  );
};

export default UnitFilters;
