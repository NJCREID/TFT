import { useEffect, useState } from "react";
import Text from "../Text/Text";
import { BiChevronDown } from "react-icons/bi";
import StaticBoard from "../StaticBoard/StaticBoard";
import fetchRequest, { Hex, UserGuide, VoteResponse } from "../../common/api";
import { BiDownvote } from "react-icons/bi";
import { BiUpvote } from "react-icons/bi";
import { ENDPOINT } from "../../common/endpoints";
import { useAuthContext } from "../../context/authContext";
import Button from "../Button/Button";
import { useNavigate } from "react-router-dom";
import AugmentTextView from "../AugmentTextView/AugmentTextView";
import UnitView from "../UnitView/UnitView";
import "./teamitem.css";
import { Link } from "react-router-dom";
import SortHexes from "../../utils/SortHexes";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";

interface TeamItemProps {
  team: UserGuide;
}

const TeamItem = ({ team }: TeamItemProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [sortedHexes, setSortedHexes] = useState<Hex[]>([]);
  const [voteStatus, setVoteStatus] = useState<Boolean | null>(null);
  const [upVotes, setUpVotes] = useState<number>(team.upVotes);
  const [downVotes, setDownVotes] = useState<number>(team.downVotes);
  const { user } = useAuthContext();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    const sorted = SortHexes(team.hexes);
    setSortedHexes(sorted);
    setUpVotes(team.upVotes);
    setDownVotes(team.downVotes);
    setVoteStatus(team.isUpvote);
  }, [team]);

  const handleVote = async (e: React.MouseEvent, isUpvote: boolean) => {
    e.stopPropagation();
    if (!user) return;
    try {
      const response: VoteResponse = await fetchRequest(
        ENDPOINT.ADD_VOTE,
        "POST",
        { userGuideId: team.id, userId: user.user.id, isUpvote },
        null,
        user.token
      );
      setUpVotes(response.upVotes);
      setDownVotes(response.downVotes);
      setVoteStatus(response.isUpvote);
    } catch (error) {
      if (error instanceof Error) {
        dispatch(showError([`Error voting: ${error.message}`]));
      } else {
        dispatch(showError(["An Unkown Error occured"]));
      }
    }
  };

  const handleViewGuide = () => {
    navigate(`/guide/${team.id}`);
  };

  return (
    <div className={`app__teamitem ${isOpen ? "open" : ""}`}>
      <div
        className="app__teamitem-summary"
        style={{
          backgroundImage: `url(${team.initialUnit?.centeredImageUrl || sortedHexes[0]?.unit?.centeredImageUrl}`,
        }}
        onClick={() => setIsOpen((prev) => !prev)}
      >
        <div className="overlay" />
        <div className="app__teamitem-summary-info-container">
          <div className="app__teamitem-summary-info">
            <div className="app__teamitem-summary-info-details-container">
              <h3 className="p__opensans-title">{team.name}</h3>
              <div className="app__teamitem-summary-info-text">
                <Text>{team.patch}</Text>
                <Text>{team.playStyle}</Text>
                <Text>{team.difficultyLevel}</Text>
              </div>
              {team.usersUsername && (
                <Link to={`/user/${team.usersUsername}`} className="p__opensans p__bold app__teamitem-user-link">
                  @{team.usersUsername}
                </Link>
              )}
            </div>
          </div>
          <div className="app__teamitem-summary-units">
            <div className="app__teamitem-summary-units-container">
              {sortedHexes.map((hex, index) => (
                <UnitView key={index} unit={hex.unit} items={hex.currentItems} isStarred={hex.isStarred} width={55} />
              ))}
            </div>
          </div>
        </div>
        <BiChevronDown className={`app__teamitem-chevron ${isOpen ? "rotate" : ""}`} size={48} />
        {user && (
          <div className="app__teamitem-summary-votes">
            <div className="app__teamitem-summary-votes-vote">
              <BiUpvote size={20} color={voteStatus === true ? "white" : ""} onClick={(e) => handleVote(e, true)} />
              <p className="p__opensans">{upVotes}</p>
            </div>
            <div className="app__teamitem-summary-votes-vote">
              <BiDownvote size={20} color={voteStatus === false ? "white" : ""} onClick={(e) => handleVote(e, false)} />
              <p className="p__opensans">{downVotes}</p>
            </div>
          </div>
        )}
      </div>
      {isOpen && (
        <div className="app__teamitem-board">
          <div className="app__teamitem-board-container">
            <Button onClick={() => handleViewGuide()}>View Guide</Button>
            <div className="app__teamitem-board-info-container">
              <div className="app__teamitem-board-info">
                <div className="app__teamitem-board-details">
                  <div>
                    <h1 className="p__opensans-title-md">Overview</h1>
                    <p className="p__desc-text">{team.generalDesc}</p>
                  </div>
                  <div>
                    <h3 className="p__opensans-small p__bold">Stage 2</h3>
                    <p className="p__desc-text">{team.stage2Desc}</p>
                  </div>
                  <div>
                    <h3 className="p__opensans-small p__bold">Stage 3</h3>
                    <p className="p__desc-text">{team.stage3Desc}</p>
                  </div>
                  <div>
                    <h3 className="p__opensans-small p__bold">Stage 4</h3>
                    <p className="p__desc-text">{team.stage4Desc}</p>
                  </div>
                </div>
                <div className="app__teamitem-board-augments">
                  {team.augments.map((augment, index) => (
                    <AugmentTextView key={index} augment={augment} />
                  ))}
                </div>
              </div>
              <div className="app__teamitem-board-units">
                <StaticBoard units={sortedHexes} />
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
export default TeamItem;
