import { useCallback } from "react";
import DropDownSelection from "../DropDownSelection/DropDownSelection";
import "./feedfilter.css";

const sortByItems = [
  { key: "best", name: "Best" },
  { key: "top", name: "Top" },
  { key: "new", name: "New" },
];

interface FeedFiltersProps {
  setSortBy: (type: string) => void;
}

const FeedFilter = ({ setSortBy }: FeedFiltersProps) => {
  const handleSortChange = useCallback(
    (type: string) => {
      setSortBy(type);
    },
    [setSortBy]
  );

  return (
    <div className="app__feedfilters">
      <div className="app__feedfilters-sortby">
        <DropDownSelection items={sortByItems} onSelect={handleSortChange} />
      </div>
    </div>
  );
};
export default FeedFilter;
