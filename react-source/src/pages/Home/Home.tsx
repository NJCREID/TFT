import { useEffect, useState } from "react";
import fetchRequest from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import "./home.css";
import { useAuthContext } from "../../context";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import { LoadingSpinner, FeedFilter, Feed } from "../../components";
import { UserGuide } from "../../data";

export default function Home() {
  const [sortBy, setSortBy] = useState("best");
  const [userGuides, setUserGuides] = useState<UserGuide[] | null>(null);
  const { user } = useAuthContext();
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchGuides = async () => {
      try {
        setUserGuides(null);
        let fetchedGuides = await fetchRequest<UserGuide[]>({
          endpoint: ENDPOINT.GUIDE_FETCH,
          identifier: user ? `${sortBy}/${user.user.id}` : sortBy,
        });
        setUserGuides(fetchedGuides);
      } catch (error) {
        setUserGuides([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching guides: ${error.message}`]));
        } else {
          dispatch(showError(["An Unkown Error occured while fetching guides."]));
        }
      }
    };
    fetchGuides();
  }, [sortBy]);

  return (
    <div className="app__home">
      <FeedFilter setSortBy={setSortBy} />
      <div className="app__home-feed page_padding">
        {!userGuides ? <LoadingSpinner /> : !!userGuides.length && <Feed posts={userGuides} />}
      </div>
    </div>
  );
}
