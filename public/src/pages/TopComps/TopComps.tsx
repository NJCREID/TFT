import Feed from "../../components/Feed/Feed";
import "./topcomps.css";
import TeamFilter from "../../components/TeamFilter/TeamFilter";
import useFilterUserGuides from "../../hooks/useFilterUserGuides";
export default function TopComps() {
  const { guides, searchQuery, setSearchQuery, setTraitFilter } = useFilterUserGuides();

  const handleSetSearchQuery = (value: string) => {
    setSearchQuery(value);
  };

  const handleSetTraitFilter = (trait: string) => {
    setTraitFilter(trait === "all" ? null : trait);
  };
  return (
    <div className="app__topcomps page_padding">
      <TeamFilter searchQuery={searchQuery} onQueryChange={handleSetSearchQuery} onTraitChange={handleSetTraitFilter} />
      {guides && <Feed posts={guides} />}
    </div>
  );
}
