import { useCallback, useEffect } from "react";
import "./augmentfilters.css";
import { Augment } from "../../data";
import { useFilterAugments } from "../../hooks";
import SearchBar from "../SearchBar/SearchBar";
import DropDownSelection from "../DropDownSelection/DropDownSelection";

interface AugmentFiltersProps {
  onAugmentsChange: (augments: Augment[] | null) => void;
}

const augmentTiers = [
  { key: "tier1", name: "Tier 1" },
  { key: "tier2", name: "Tier 2" },
  { key: "tier3", name: "Tier 3" },
];

const AugmentFilters = ({ onAugmentsChange }: AugmentFiltersProps) => {
  const { augments, augmentQuery, setAugmentQuery, setTierFilter } = useFilterAugments();

  useEffect(() => {
    onAugmentsChange(augments);
  }, [augments, onAugmentsChange]);

  const handleAugmentQueryChange = useCallback((query: string) => {
    setAugmentQuery(query);
  }, []);

  const handleTierSelect = useCallback((tier: string) => {
    setTierFilter(tier);
  }, []);

  return (
    <>
      <SearchBar value={augmentQuery} placeHolder="Search..." onChange={handleAugmentQueryChange} />
      <DropDownSelection items={augmentTiers} onSelect={handleTierSelect} allDisplayName="All Tiers" includeAll />
    </>
  );
};

export default AugmentFilters;
