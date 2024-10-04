import { useCallback, useEffect } from "react";
import "./traitfilters.css";
import { useFilterTraits } from "../../hooks";
import SearchBar from "../SearchBar/SearchBar";
import { Trait } from "../../data";

interface TraitFiltersProps {
  onTraitsChange: (augments: Trait[] | null) => void;
}

const TraitFilters = ({ onTraitsChange }: TraitFiltersProps) => {
  const { traits, traitQuery, setTraitQuery } = useFilterTraits();

  useEffect(() => {
    onTraitsChange(traits);
  }, [traits, onTraitsChange]);

  const handleTraitQueryChange = useCallback((query: string) => {
    setTraitQuery(query);
  }, []);

  return (
    <>
      <SearchBar value={traitQuery} placeHolder="Search..." onChange={handleTraitQueryChange} />
    </>
  );
};

export default TraitFilters;
