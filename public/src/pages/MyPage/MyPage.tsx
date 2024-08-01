import { useEffect, useState } from "react";
import Feed from "../../components/Feed/Feed";
import { useAuthContext } from "../../context/authContext";
import fetchRequest, { UserGuide } from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import { useNavigate } from "react-router-dom";
import "./mypage.css";
import { FaRegUserCircle } from "react-icons/fa";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";

const groups = [
  { name: "My Guides", endpoint: ENDPOINT.USERGUIDE_USERID },
  { name: "Commented", endpoint: ENDPOINT.USERGUIDE_COMMENTED },
  { name: "Upvoted", endpoint: ENDPOINT.USERGUIDE_UPVOTED },
  { name: "Downvoted", endpoint: ENDPOINT.USERGUIDE_DOWNVOTED },
];

const MyPage = () => {
  const { user } = useAuthContext();
  const [userGuides, setUserGuides] = useState<UserGuide[] | null>(null);
  const [activeGroup, setActiveGroup] = useState<{ name: string; endpoint: string }>(groups[0]);
  const navigate = useNavigate();
  const dispatch = useDispatch();
  useEffect(() => {
    const fetchGuides = async () => {
      if (user) {
        try {
          const guides = await fetchRequest<UserGuide[]>(
            activeGroup.endpoint,
            "GET",
            null,
            `${user?.user.id}`,
            user.token
          );
          setUserGuides(guides);
        } catch (error) {
          if (error instanceof Error) {
            dispatch(showError([`Error fetching ${activeGroup.name}: ${error.message}`]));
          } else {
            dispatch(showError(["An unknown error occurred"]));
          }
        }
      }
    };
    fetchGuides();
  }, [activeGroup, user]);

  if (!user) {
    navigate("sign-in");
    return;
  }

  return (
    <div className="app__mypage">
      <div className="app__mypage-content">
        <div className="app__mypage-content-profile">
          <div>
            <h1 className="p__opensans-title-large">{user.user.name}</h1>
            <h3 className="p__opensans">@{user.user.username}</h3>
          </div>
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
        {userGuides && <Feed posts={userGuides} />}
      </div>
      <div className="app__mypage-profilesidebar">
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
        <hr />
        <p className="p__opensans">Settings</p>
        <div className="app__mypage-profilesidebar-settings">
          <div className="app__mypage-profilesidebar-section">
            <FaRegUserCircle color="white" size={28} />
            <div>
              <p className="p__opensans">Profile</p>
              <p className="p__desc-text">Customize your profile</p>
            </div>
            <button className="p__opensans">Edit Profile</button>
          </div>
        </div>
        <hr />
        <div className="app__mypage-profilesidebar-"></div>
      </div>
    </div>
  );
};

export default MyPage;
