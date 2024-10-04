import DropDownSelection from "../DropDownSelection/DropDownSelection";
import SearchBar from "../SearchBar/SearchBar";
import { useEffect, useState } from "react";
import "./teamfilter.css";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import { Trait } from "../../data";
import fetchRequest from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";

interface TeamFilterProps {
  onTraitChange: (trait: string) => void;
  onQueryChange: (value: string) => void;
  searchQuery: string;
}

export default function TeamFilter({ onQueryChange, onTraitChange, searchQuery }: TeamFilterProps) {
  const [traits, setTraits] = useState<Trait[]>([]);
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchItems = async () => {
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
    fetchItems();
  }, []);

  const handleQueryChange = (value: string) => {
    onQueryChange(value);
  };

  const handleTraitChange = (trait: string) => {
    onTraitChange(trait);
  };

  return (
    <div className="app__teamfilter">
      <SearchBar placeHolder="Search" value={searchQuery} onChange={handleQueryChange} />
      <DropDownSelection
        items={traits}
        onSelect={handleTraitChange}
        includeAll
        includeImages
        allDisplayName="All Synergies"
      />
    </div>
  );
}
