import { useEffect, useState } from "react";
import Text from "../Text/Text";
import { BiChevronDown } from "react-icons/bi";
import StaticBoard from "../StaticBoard/StaticBoard";
import fetchRequest from "../../common/api";
import { BiDownvote } from "react-icons/bi";
import { BiUpvote } from "react-icons/bi";
import { ENDPOINT } from "../../common/endpoints";
import { useAuthContext } from "../../context";
import Button from "../Button/Button";
import { useNavigate } from "react-router-dom";
import AugmentTextView from "../AugmentTextView/AugmentTextView";
import UnitView from "../UnitView/UnitView";
import "./guideitem.css";
import { Link } from "react-router-dom";
import { getImageUrl, sortHexes, sortTraits } from "../../utils";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import { Hex, UserGuide, VoteResponse } from "../../data";
import TraitItem from "../TraitItem/TraitItem";

interface GuideItemProps {
  guide: UserGuide;
}

const GuideItem = ({ guide }: GuideItemProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [sortedHexes, setSortedHexes] = useState<Hex[] | null>(null);
  const [voteStatus, setVoteStatus] = useState<Boolean | null>(null);
  const [upVotes, setUpVotes] = useState<number>(guide.upVotes);
  const [downVotes, setDownVotes] = useState<number>(guide.downVotes);
  const { user } = useAuthContext();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    const sorted = sortHexes(guide.hexes);
    setSortedHexes(sorted);
    setUpVotes(guide.upVotes);
    setDownVotes(guide.downVotes);
    setVoteStatus(guide.isUpvote);
  }, [guide]);

  const handleVote = async (e: React.MouseEvent, isUpvote: boolean) => {
    e.stopPropagation();
    if (!user) {
      dispatch(showError([`You must be logged in to vote.`]));
      return;
    }
    try {
      const response: VoteResponse = await fetchRequest({
        endpoint: ENDPOINT.ADD_VOTE,
        method: "POST",
        body: { userGuideId: guide.id, userId: user.user.id, isUpvote },
        authToken: user.token,
      });
      setUpVotes(response.upVotes);
      setDownVotes(response.downVotes);
      setVoteStatus(response.isUpvote);
    } catch (error) {
      if (error instanceof Error) {
        dispatch(showError([`Error voting: ${error.message}`]));
      } else {
        dispatch(showError(["An Unkown Error occured while voting."]));
      }
    }
  };

  const handleViewGuide = () => {
    navigate(`/guide/${guide.id}`);
  };

  const sortedTraits = sortTraits(guide.traits);

  return (
    !!sortedHexes && (
      <div className={`app__guideitem ${isOpen ? "open" : ""}`} key={guide.id}>
        <div
          className="app__guideitem-summary"
          style={{
            backgroundImage: `url(${
              guide.initialUnit
                ? getImageUrl(guide.initialUnit.inGameKey, "champions", "splash")
                : getImageUrl(sortedHexes[0]?.unit?.inGameKey, "champions", "splash")
            }`,
          }}
          onClick={() => setIsOpen((prev) => !prev)}
        >
          <div className="overlay" />
          <div className="app__guideitem-summary-container">
            <div className="app__guideitem-summary-info-container">
              <div className="app__guideitem-summary-info">
                <div className="app__guideitem-summary-info-details-container">
                  <h3 className="p__opensans-title">{guide.name}</h3>
                  <div className="app__guideitem-summary-info-text">
                    <Text>{guide.patch}</Text>
                    <Text>{guide.playStyle}</Text>
                    <Text>{guide.difficultyLevel}</Text>
                  </div>
                  {guide.usersUsername && (
                    <Link to={`/user/${guide.usersUsername}`} className="p__opensans p__bold app__guideitem-user-link">
                      @{guide.usersUsername}
                    </Link>
                  )}
                </div>
              </div>
              <div className="app__guideitem-summary-units">
                <div className="app__guideitem-summary-units-container">
                  {sortedHexes.map((hex, index) => (
                    <UnitView
                      key={index}
                      unit={hex.unit}
                      items={hex.currentItems}
                      isStarred={hex.isStarred}
                      width={55}
                    />
                  ))}
                </div>
              </div>
            </div>
            <div className="app__guideitem-summary-traits">
              {sortedTraits.map((trait, index) => (
                <TraitItem userGuideTrait={trait} key={index} />
              ))}
            </div>
          </div>
          <BiChevronDown className={`app__guideitem-chevron ${isOpen ? "rotate" : ""}`} size={48} />
          <div className="app__guideitem-summary-votes">
            <div className={`app__guideitem-summary-votes-vote ${!user ? "disabled" : ""}`}>
              <BiUpvote size={20} color={voteStatus === true ? "white" : ""} onClick={(e) => handleVote(e, true)} />
              <p className="p__opensans">{upVotes}</p>
            </div>
            <div className={`app__guideitem-summary-votes-vote ${!user ? "disabled" : ""}`}>
              <BiDownvote size={20} color={voteStatus === false ? "white" : ""} onClick={(e) => handleVote(e, false)} />
              <p className="p__opensans">{downVotes}</p>
            </div>
          </div>
        </div>
        {isOpen && (
          <div className="app__guideitem-board">
            <div className="app__guideitem-board-container">
              <Button onClick={() => handleViewGuide()} aria-label="View guide">
                View Guide
              </Button>
              <div className="app__guideitem-board-info-container">
                <div className="app__guideitem-board-info">
                  <div className="app__guideitem-board-details">
                    <div>
                      <h1 className="p__opensans-title-md">Overview</h1>
                      <p className="p__desc-text">{guide.generalDesc}</p>
                    </div>
                    <div>
                      <h3 className="p__opensans-small p__bold">Stage 2</h3>
                      <p className="p__desc-text">{guide.stage2Desc}</p>
                    </div>
                    <div>
                      <h3 className="p__opensans-small p__bold">Stage 3</h3>
                      <p className="p__desc-text">{guide.stage3Desc}</p>
                    </div>
                    <div>
                      <h3 className="p__opensans-small p__bold">Stage 4</h3>
                      <p className="p__desc-text">{guide.stage4Desc}</p>
                    </div>
                  </div>
                  <div className="app__guideitem-board-augments">
                    {guide.augments.map((augment, index) => (
                      <AugmentTextView key={index} augment={augment} />
                    ))}
                  </div>
                </div>
                <div className="app__guideitem-board-units">
                  <StaticBoard units={sortedHexes} />
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    )
  );
};
export default GuideItem;
