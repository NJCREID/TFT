import { useCallback, useEffect, useState } from "react";
import { ENDPOINT } from "../../common/endpoints";
import fetchRequest, { Augment, UserGuide, UserGuideTrait } from "../../common/api";
import { useAuthContext } from "../../context/authContext";
import "./postform.css";
import DropDownSelection from "../../components/DropDownSelection/DropDownSelection";
import Selector from "../../components/Selector/Selector";
import SearchBar from "../../components/SearchBar/SearchBar";
import Button from "../../components/Button/Button";
import SelectorItem from "../../components/SelectorItem/SelectorItem";
import { MdDeleteOutline } from "react-icons/md";
import { HexItem } from "../../pages/TeamBuilder/TeamBuilder";
import { TbShare2 } from "react-icons/tb";
import { useNavigate } from "react-router-dom";
import { isItem, isUnit } from "../../utils/isObject";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../../store";
import { setDraggedItem } from "../../store/dragSlice";
import { showError } from "../../store/errorModalSlice";

const difficultyLevels = [
  { key: "Easy", name: "Easy" },
  { key: "Medium", name: "Medium" },
  { key: "Hard", name: "Hard" },
];

const augmentTiers = [
  { key: "alltiers", name: "All Tiers" },
  { key: "tier1", name: "Tier 1" },
  { key: "tier2", name: "Tier 2" },
  { key: "tier3", name: "Tier 3" },
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
  const [augments, setAugments] = useState<Augment[]>([]);
  const [filteredAugments, setFilteredAugments] = useState<Augment[]>([]);
  const [selectedAugments, setSelectedAugments] = useState<Augment[]>([]);
  const [augmentQuery, setAugmentQuery] = useState("");
  const [augmentTier, setAugmentTier] = useState<string | null>(null);
  const [playStyle, setPlayStyle] = useState(playStyles[0].key);
  const [difficultyLevel, setDifficultyLevel] = useState(difficultyLevels[0].key);
  const draggedItem = useSelector((state: RootState) => state.drag.draggedItem);
  const { user } = useAuthContext();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchAugments = async () => {
      try {
        const fetchedAugments = await fetchRequest<Augment[]>(ENDPOINT.AUGMENT_FETCH, "GET", null);
        setAugments(fetchedAugments);
      } catch (error) {
        if (error instanceof Error) {
          dispatch(showError([`Error fetching augments: ${error.message}`]));
        } else {
          dispatch(showError(["An unknown error occurred"]));
        }
      }
    };
    fetchAugments();
  }, []);

  useEffect(() => {
    if (!augments) return;
    let tempFilteredAugments = augments.filter((augment) =>
      augment.name.toLowerCase().includes(augmentQuery.toLowerCase())
    );
    if (augmentTier && augmentTier != "alltiers") {
      tempFilteredAugments = tempFilteredAugments.filter((augment) => augmentTier.includes(augment.tier.toString()));
    }
    setFilteredAugments(tempFilteredAugments);
  }, [augmentQuery, augmentTier, augments]);

  const handleDragEnd = useCallback(
    (e: React.DragEvent, isFromSelectedAugments?: boolean) => {
      const dropTarget = document.elementFromPoint(e.clientX, e.clientY);
      const isOverSelection = dropTarget && dropTarget.closest(".app__postform-form-augments-selection");
      if (draggedItem) {
        if (!isOverSelection && isFromSelectedAugments) {
          setSelectedAugments((prev) => prev.filter((augment) => augment.key !== draggedItem.key));
        }
      }
      dispatch(setDraggedItem(null));
    },
    [setDraggedItem, draggedItem]
  );

  const handleDrop = useCallback(() => {
    if (
      draggedItem &&
      !selectedAugments.some((augment) => augment.key === draggedItem.key) &&
      !isUnit(draggedItem) &&
      !isItem(draggedItem)
    ) {
      setSelectedAugments((prev) => [...prev, draggedItem as Augment]);
    }
    dispatch(setDraggedItem(null));
  }, [draggedItem, setDraggedItem, selectedAugments]);

  const handleDragOver = (event: React.DragEvent) => {
    event.preventDefault();
  };

  const handleQueryChange = (query: string) => {
    setAugmentQuery(query);
  };

  const handleTierChange = (tier: string) => {
    setAugmentTier(tier);
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
      const build = await fetchRequest<UserGuide>(
        ENDPOINT.POST_GUIDE,
        "POST",
        userGuide,
        currentUser.id.toString(),
        user?.token
      );
      navigate(`/guide/${build.id}`);
    } catch (error) {
      if (error instanceof Error) {
        dispatch(showError([`Failed to update user guide: ${error.message}`]));
      } else {
        dispatch(showError(["An unknown error occurred"]));
      }
    }
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
                <Button onClick={() => setSelectedAugments([])}>
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
                    <SelectorItem handleDragEnd={handleDragEnd} key={augment.key} item={augment} />
                  ))}
                </div>
              </div>
              <div className="app__postform-form-augments-filters">
                <SearchBar value={augmentQuery} placeHolder="Search..." onChange={handleQueryChange} />
                <DropDownSelection items={augmentTiers} onSelect={handleTierChange} />
              </div>
            </div>
            <Selector data={filteredAugments} />
          </div>
        </div>
        <Button type="submit">
          <TbShare2 size={15} />
          {user ? <p>Post Board</p> : <p>Log in to post</p>}
        </Button>
      </form>
    </div>
  );
};

export default PostForm;
