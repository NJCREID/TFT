import DropDownSelection from "../DropDownSelection/DropDownSelection";
import SearchBar from "../SearchBar/SearchBar";
import { useEffect, useState } from "react";
import { Trait, fetchTraits } from "../../common/api";
import "./teamfilter.css";

interface TeamFilterProps {
  onTraitChange: (trait: string) => void;
  onQueryChange: (value: string) => void;
  searchQuery: string;
}

export default function TeamFilter({ onQueryChange, onTraitChange, searchQuery }: TeamFilterProps) {
  const [traits, setTraits] = useState<Trait[] | null>(null);

  useEffect(() => {
    const fetchItems = async () => {
      setTraits(await fetchTraits());
    };
    fetchItems();
  }, []);

  const handleQueryChange = (value: string) => {
    onQueryChange(value);
  };

  const handleTraitChange = (trait: string) => {
    onTraitChange(trait);
  };

  return (
    traits && (
      <div className="app__teamfilter">
        <SearchBar placeHolder="Search" value={searchQuery} onChange={handleQueryChange} />
        <DropDownSelection items={traits} onSelect={handleTraitChange} />
      </div>
    )
  );
}
