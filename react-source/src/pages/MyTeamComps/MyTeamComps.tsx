import { useNavigate } from "react-router-dom";
import { ENDPOINT } from "../../common/endpoints";
import { useAuthContext } from "../../context";
import { useFilterUserGuides } from "../../hooks";
import { LoadingSpinner, TeamFilter, Feed } from "../../components";
import "./myteamcomps.css";
import { useEffect } from "react";

export default function MyTeamComps() {
  const { guides, searchQuery, setSearchQuery, setTraitFilter } = useFilterUserGuides(ENDPOINT.USERGUIDE_USERID);
  const navigate = useNavigate();
  const { user } = useAuthContext();
  const handleSetSearchQuery = (value: string) => {
    setSearchQuery(value);
  };

  const handleSetTraitFilter = (trait: string) => {
    setTraitFilter(trait === "all" ? null : trait);
  };

  useEffect(() => {
    if (!user) {
      navigate("/sign-in");
      return;
    }
  }, [user]);

  return (
    <div className="app__myteamcomps page_padding">
      <TeamFilter searchQuery={searchQuery} onQueryChange={handleSetSearchQuery} onTraitChange={handleSetTraitFilter} />
      {!guides ? <LoadingSpinner /> : !!guides.length && <Feed posts={guides} />}
    </div>
  );
}
