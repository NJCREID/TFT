import { useEffect, useState } from "react";
import { useAuthContext } from "../../context";
import fetchRequest from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import { useNavigate } from "react-router-dom";
import "./mypage.css";
import { FaRegUserCircle } from "react-icons/fa";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import { LoadingSpinner, Feed } from "../../components";
import { UserGuide } from "../../data";
import { User } from "../../context/authContext";

const groups = [
  { name: "My Guides", endpoint: ENDPOINT.USERGUIDE_USERID },
  { name: "Commented", endpoint: ENDPOINT.USERGUIDE_COMMENTED },
  { name: "Upvoted", endpoint: ENDPOINT.USERGUIDE_UPVOTED },
  { name: "Downvoted", endpoint: ENDPOINT.USERGUIDE_DOWNVOTED },
];

const MyPage = () => {
  const { user, updateUser } = useAuthContext();
  const [userGuides, setUserGuides] = useState<UserGuide[] | null>(null);
  const [activeGroup, setActiveGroup] = useState<{ name: string; endpoint: string }>(groups[0]);
  const navigate = useNavigate();
  const dispatch = useDispatch();

  if (!user) {
    navigate("/sign-in");
    return;
  }

  useEffect(() => {
    const fetchGuides = async () => {
      try {
        setUserGuides(null);
        const guides = await fetchRequest<UserGuide[]>({
          endpoint: activeGroup.endpoint,
          identifier: `${user.user.id}`,
          authToken: user.token,
        });
        setUserGuides(guides);
      } catch (error) {
        setUserGuides([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching ${activeGroup.name}: ${error.message}`]));
        } else {
          dispatch(showError([`An unknown error occurred while fetching ${activeGroup.name}`]));
        }
      }
    };
    fetchGuides();
  }, [activeGroup, user]);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const fetchedUser = await fetchRequest<User>({
          endpoint: ENDPOINT.USER_FETCH_ID,
          identifier: user.user.id.toString(),
          authToken: user.token,
        });
        updateUser(fetchedUser);
      } catch (error) {
        if (error instanceof Error) {
          dispatch(showError([`Error fetching user information: ${error.message}`]));
        } else {
          dispatch(showError([`An unknown error occurred while fetching user information`]));
        }
      }
    };
    fetchUser();
  }, []);

  const handleEditProfile = () => {
    navigate("/edit-profile");
  };

  return user?.user ? (
    <div className="app__mypage">
      <div className="app__mypage-content">
        <div className="app__mypage-content-profile">
          <div>
            <h1 className="p__opensans-title-large">{user.user.name}</h1>
            <h3 className="p__opensans">@{user.user.username}</h3>
          </div>
          <div className="app__mypage-content-profile-wrapper">
            <div className="app__mypage-content-profile-groups">
              {groups.map((group) => (
                <div
                  key={group.name}
                  onClick={() => setActiveGroup(group)}
                  className={`app__mypage-content-profile-groups-group ${activeGroup.name === group.name && "active"}`}
                >
                  <p className="p__opensans">{group.name}</p>
                </div>
              ))}
            </div>
          </div>
        </div>
        {!userGuides ? <LoadingSpinner /> : !!userGuides.length && <Feed posts={userGuides} />}
      </div>
      <div className="app__mypage-profilesidebar">
        <div className="app__mypage-profilesidebar-section">
          <p className="p__opensans">{user.user.username}</p>
          <div className="app__mypage-profilesidebar-stats-container">
            <div className="app__mypage-profilesidebar-stat">
              <p className="p__opensans">{user.user.guidesCount}</p>
              <p className="p__desc-text">Guides</p>
            </div>
            <div className="app__mypage-profilesidebar-stat">
              <p className="p__opensans">{user.user.commentsCount}</p>
              <p className="p__desc-text">Comments</p>
            </div>
            <div className="app__mypage-profilesidebar-stat">
              <p className="p__opensans">{user.user.upVotesCount}</p>
              <p className="p__desc-text">Upvotes</p>
            </div>
            <div className="app__mypage-profilesidebar-stat">
              <p className="p__opensans">{user.user.downVotesCount}</p>
              <p className="p__desc-text">DownVotes</p>
            </div>
          </div>
        </div>
        <hr />
        <div className="app__mypage-profilesidebar-section">
          <p className="p__opensans">Settings</p>
          <div className="app__mypage-profilesidebar-settings">
            <div className="app__mypage-profilesidebar-settings-container">
              <FaRegUserCircle color="white" size={28} />
              <div>
                <p className="p__opensans">Profile</p>
                <p className="p__desc-text">Customize your profile</p>
              </div>
              <button className="p__opensans" onClick={handleEditProfile}>
                Edit Profile
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  ) : null;
};

export default MyPage;
