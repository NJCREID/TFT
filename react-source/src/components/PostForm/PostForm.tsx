import { useEffect, useState } from "react";
import { ENDPOINT } from "../../common/endpoints";
import fetchRequest from "../../common/api";
import Button from "../../components/Button/Button";
import { MdDeleteOutline } from "react-icons/md";
import { TbShare2 } from "react-icons/tb";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../../store";
import { showError } from "../../store/errorModalSlice";
import AugmentFilters from "../AugmentFIlters/AugmentFilters";
import { Augment, UserGuide, UserGuideTrait } from "../../data";
import { HexItem } from "../../pages/TeamBuilder/TeamBuilder";
import { useAuthContext } from "../../context";
import { isItem, isUnit } from "../../utils";
import DropDownSelection from "../DropDownSelection/DropDownSelection";
import SelectorItem from "../SelectorItem/SelectorItem";
import Selector from "../Selector/Selector";
import "./postform.css";

const difficultyLevels = [
  { key: "Easy", name: "Easy" },
  { key: "Medium", name: "Medium" },
  { key: "Hard", name: "Hard" },
];

const playStyles = [
  { key: "Defualt", name: "Defualt" },
  { key: "Level 7 Slow Roll", name: "Level 7 Slow Roll" },
  { key: "Level 5 Slow Roll", name: "Level 5 Slow Roll" },
  { key: "Level 6 Slow Roll", name: "Level 6 Slow Roll" },
  { key: "Fast 9", name: "Fast 9" },
  { key: "Fast 8", name: "Fast 8" },
];

const PostForm = ({ traits, hexes }: { traits: UserGuideTrait[]; hexes: HexItem[] }) => {
  const [augments, setAugments] = useState<Augment[] | null>(null);
  const [selectedAugments, setSelectedAugments] = useState<Augment[]>([]);
  const [playStyle, setPlayStyle] = useState(playStyles[0].key);
  const [difficultyLevel, setDifficultyLevel] = useState(difficultyLevels[0].key);
  const draggedItem = useSelector((state: RootState) => state.drag.draggedItem);
  const { user } = useAuthContext();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchAugments = async () => {
      try {
        const fetchedAugments = await fetchRequest<Augment[]>({
          endpoint: ENDPOINT.AUGMENT_FETCH,
        });
        setAugments(fetchedAugments);
      } catch (error) {
        setAugments([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching augments: ${error.message}`]));
        } else {
          dispatch(showError(["An unknown error occurred"]));
        }
      }
    };
    fetchAugments();
  }, []);

  const handleAugmentDrop = (e: React.DragEvent, isFromSelectedAugments?: boolean) => {
    const dropTarget = document.elementFromPoint(e.clientX, e.clientY);
    const isOverSelection = dropTarget && dropTarget.closest(".app__postform-form-augments-selection");
    if (draggedItem) {
      if (!isOverSelection && isFromSelectedAugments) {
        setSelectedAugments((prev) => prev.filter((augment) => augment.inGameKey !== draggedItem.inGameKey));
      }
    }
  };

  const handleDrop = () => {
    if (
      draggedItem &&
      !selectedAugments.some((augment) => augment.inGameKey === draggedItem.inGameKey) &&
      !isUnit(draggedItem) &&
      !isItem(draggedItem)
    ) {
      setSelectedAugments((prev) => [...prev, draggedItem as Augment]);
    }
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
  };

  const postBuild = async (event: React.SyntheticEvent) => {
    if (!user) {
      navigate("/sign-in");
      return;
    }
    event.preventDefault();
    const { buildName, description, stage2, stage3, stage4 } = event.target as typeof event.target & {
      buildName: HTMLInputElement;
      description: HTMLTextAreaElement;
      stage2: HTMLTextAreaElement;
      stage3: HTMLTextAreaElement;
      stage4: HTMLTextAreaElement;
    };
    if (selectedAugments.length < 2) {
      dispatch(showError(["You Must Select at least 2 Augments"]));
      return;
    }
    let userGuide = {
      patch: "14.10.1",
      name: buildName.value,
      generalDesc: description.value,
      stage2Desc: stage2.value,
      stage3Desc: stage3.value,
      stage4Desc: stage4.value,
      augments: selectedAugments,
      difficultyLevel: difficultyLevel,
      playStyle: playStyle,
      hexes: hexes.filter((a) => a.unit),
      traits: traits,
    };
    try {
      let currentUser = user.user;
      const build = await fetchRequest<UserGuide>({
        endpoint: ENDPOINT.POST_GUIDE,
        method: "POST",
        body: userGuide,
        identifier: currentUser.id.toString(),
        authToken: user?.token,
      });
      navigate(`/guide/${build.id}`);
    } catch (error) {
      if (error instanceof Error) {
        dispatch(showError([`Failed to update user guide: ${error.message}`]));
      } else {
        dispatch(showError(["An unknown error occurred while creating guide."]));
      }
    }
  };

  const handleAugmentsChange = (augments: Augment[] | null) => {
    setAugments(augments);
  };

  return (
    <div className="app__postform">
      <form onSubmit={postBuild} className="app__postform-form">
        <div className="app__postform-form-container">
          <article className="app__postform-form-article">
            <h1 className="p__opensans-title-large">Build Details</h1>
            <div className="app__postform-form-details">
              <div className="app__postform-form-details-container">
                <input
                  className="app__post-form-input"
                  type="text"
                  placeholder="Build Name..."
                  name="buildName"
                  id="buildName"
                  required
                  autoComplete="off"
                />
                <div className="app__postform-form-selectors">
                  <div className="app__postform-form-playstyle">
                    <DropDownSelection items={playStyles} onSelect={setPlayStyle} />
                  </div>
                  <div className="app__postform-form-difficulty">
                    <DropDownSelection items={difficultyLevels} onSelect={setDifficultyLevel} />
                  </div>
                </div>
              </div>
              <textarea
                className="app__post-form-input"
                placeholder="Description..."
                name="description"
                id="description"
                required
              />
              <textarea className="app__post-form-input" placeholder="Stage 2..." name="stage2" id="stage2" required />
              <textarea className="app__post-form-input" placeholder="Stage 3..." name="stage3" id="stage3" required />
              <textarea className="app__post-form-input" placeholder="Stage 4..." name="stage4" id="stage4" required />
            </div>
          </article>
          <div className="app__postform-form-augments">
            <div className="app__postform-form-augments-selector">
              <div className="app__postform-form-augments-header">
                <h4 className="p__opensans">Drag to Add Augments</h4>
                <Button onClick={() => setSelectedAugments([])} aria-label="Clear Augment Selections">
                  <MdDeleteOutline size={15} />
                  <p>Clear Augments</p>
                </Button>
              </div>
              <div
                onDragOver={handleDragOver}
                onDrop={handleDrop}
                className="app__postform-form-augments-selection-container"
              >
                <div className="app__postform-form-augments-selection">
                  {selectedAugments.map((augment) => (
                    <SelectorItem
                      onDragEnd={(e) => handleAugmentDrop(e, true)}
                      key={augment.inGameKey}
                      object={augment}
                    />
                  ))}
                </div>
              </div>
              <div className="app__postform-form-augments-filters">
                <AugmentFilters onAugmentsChange={handleAugmentsChange} />
              </div>
            </div>
            <Selector data={augments} />
          </div>
        </div>
        <Button type="submit" aria-label="Post guide">
          <TbShare2 size={15} />
          {user ? <p>Post Board</p> : <p>Log in to post</p>}
        </Button>
      </form>
    </div>
  );
};

export default PostForm;
