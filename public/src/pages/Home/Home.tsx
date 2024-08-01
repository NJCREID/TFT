import { useEffect, useState } from "react";
import Feed from "../../components/Feed/Feed";
import fetchRequest, { UserGuide } from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import FeedFilter from "../../components/FeedFilter/FeedFilter";
import "./home.css";
import { useAuthContext } from "../../context/authContext";

export default function Home() {
  const [sortBy, setSortBy] = useState("best");
  const [userGuides, setUserGuides] = useState<UserGuide[]>([]);
  const { user } = useAuthContext();
  useEffect(() => {
    const fetchGuides = async () => {
      setUserGuides(
        await fetchRequest(ENDPOINT.GUIDE_FETCH, "GET", null, user ? `${sortBy}?userId=${user.user.id}` : sortBy)
      );
    };
    fetchGuides();
  }, [sortBy]);
  return (
    <div className="app__home">
      <FeedFilter setSortBy={setSortBy} />
      <div className="app__home-feed page_padding">{userGuides && <Feed posts={userGuides} />}</div>
    </div>
  );
}
