import React, { useEffect, useRef, useState } from "react";
import "./viewguide.css";
import { useParams } from "react-router-dom";
import fetchRequest, { Comment, Hex, Item, UserGuide } from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import { useAuthContext } from "../../context/authContext";
import StaticBoard from "../../components/StaticBoard/StaticBoard";
import AugmentTextView from "../../components/AugmentTextView/AugmentTextView";
import { BASE_IMAGE_URL, IMAGE_ENDPOINTS } from "../../data/endpoints";
import UnitView from "../../components/UnitView/UnitView";
import { MdKeyboardArrowRight } from "react-icons/md";
import ItemView from "../../components/ItemView/ItemView";
import { IoSend } from "react-icons/io5";
import { Link } from "react-router-dom";
import SortHexes from "../../utils/SortHexes";

const ViewGuide = () => {
  const { guideId } = useParams();
  const [guide, setGuide] = useState<UserGuide | null>(null);
  const [sortedHexes, setSortedHexes] = useState<Hex[]>([]);
  const [commentInput, setCommentInput] = useState("");
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const { user } = useAuthContext();

  useEffect(() => {
    if (!user) return;
    const fetchGuide = async () => {
      const fetchedGuide: UserGuide = await fetchRequest(ENDPOINT.GUIDE_FETCH_ID, "GET", null, guideId, user.token);
      setGuide(fetchedGuide);
    };
    fetchGuide();
  }, [guideId, user]);

  useEffect(() => {
    if (!guide) return;
    const hexes = SortHexes(guide.hexes);
    setSortedHexes(hexes);
  }, [guide]);

  const handleInputChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    setCommentInput(e.target.value);
    const textarea = textareaRef.current;
    if (textarea) {
      textarea.style.height = "auto";
      textarea.style.height = `${textarea.scrollHeight}px`;
    }
  };

  const handleComment = async (e: React.MouseEvent) => {
    e.preventDefault();
    if (!user || !guide) return;
    const addedComment: Comment = await fetchRequest(
      ENDPOINT.ADD_COMMENT,
      "POST",
      {
        UserId: user.user.id,
        UserGuideId: guide.id,
        Content: commentInput,
      },
      null,
      user.token
    );
    setGuide((prevGuide) => {
      if (!prevGuide) return prevGuide;
      return {
        ...prevGuide,
        comments: [...prevGuide.comments, addedComment],
      };
    });
    setCommentInput("");
  };

  const getTop5Components = (guide: UserGuide) => {
    if (!guide || !guide.hexes) return [];
    const componentCount: { [key: string]: number } = {};
    guide.hexes.forEach((hex: Hex) => {
      hex.currentItems.forEach((item: Item) => {
        if (item.isComponent) {
          if (componentCount[item.key]) {
            componentCount[item.key]++;
          } else {
            componentCount[item.key] = 1;
          }
        }
        item.recipe.forEach((component: string) => {
          if (componentCount[component]) {
            componentCount[component]++;
          } else {
            componentCount[component] = 1;
          }
        });
      });
    });
    const sortedRecipes = Object.entries(componentCount).sort((a, b) => b[1] - a[1]);
    return sortedRecipes.slice(0, 5).map(([recipe]) => recipe);
  };

  const top5Components = guide ? getTop5Components(guide) : [];

  const itemHolders = sortedHexes.filter((hex: Hex) => hex.currentItems.length > 0) || [];

  return (
    guide && (
      <div className="app__viewguide page_padding">
        <div className="app__viewguide-banner">
          <img src={guide.hexes[0]?.unit?.centeredImageUrl} alt={guide.hexes[0]?.unit?.centeredImageUrl} />
        </div>
        <div className="app__viewguide-guide">
          <h1 className="p__opensans-title-large">{guide.name}</h1>
          {guide.usersUsername && (
            <Link to={`/user/${guide.usersUsername}`} className="p__opensans p__bold">
              @{guide.usersUsername}
            </Link>
          )}
          <div className="app__viewguide-guide-content">
            <div className="app__viewguide-guide-section1">
              <div className="app__viewguide-guide-container">
                <div>
                  <h2 className="p__opensans">Overview</h2>
                  <p className="p__desc-text">{guide.generalDesc}</p>
                </div>
                <div className="app__viewguide-guide-board">
                  <StaticBoard units={guide.hexes} />
                </div>
                <div>
                  <h3 className="p__opensans">Stage 2</h3>
                  <p className="p__desc-text">{guide.stage2Desc}</p>
                </div>
                <div>
                  <h3 className="p__opensans">Stage 3</h3>
                  <p className="p__desc-text">{guide.stage3Desc}</p>
                </div>
                <div>
                  <h3 className="p__opensans">Stage 4</h3>
                  <p className="p__desc-text">{guide.stage4Desc}</p>
                </div>
                <div className="app__viewguide-guide-units">
                  {sortedHexes.map((hex, index) => (
                    <UnitView key={index} unit={hex.unit} items={hex.currentItems} isStarred={hex.isStarred} />
                  ))}
                </div>
              </div>
              <div className="app__viewguide-guide-comments">
                <div className="app__viewguide-guide-comment-input">
                  <textarea
                    ref={textareaRef}
                    value={commentInput}
                    onChange={handleInputChange}
                    placeholder="Add a comment..."
                  />
                  <button onClick={handleComment}>
                    <IoSend size={18} />
                  </button>
                </div>
                <div className="app__viewguide-guide-comment-list">
                  {guide.comments.map((comment, index) => (
                    <React.Fragment key={index}>
                      <div className="app__viewguide-guide-comment">
                        <p className="p__opensans-small">{comment.author}</p>
                        <p className="p__desc-text">{comment.content}</p>
                      </div>
                      {index < guide.comments.length - 1 && <hr />}
                    </React.Fragment>
                  ))}
                </div>
              </div>
            </div>
            <div className="app__viewguide-guide-section2">
              <div className="app__viewguide-guide-augments">
                <h3 className="p__opensans">Suggested Augments</h3>
                {guide.augments.map((augment, index) => (
                  <AugmentTextView key={index} augment={augment} />
                ))}
              </div>
              {top5Components.length && (
                <div className="app__viewguide-guide-componentpriority">
                  <h3 className="p__opensans">Component Priority</h3>
                  <div className="app__viewguide-guide-components">
                    {top5Components.map((component, index) => (
                      <React.Fragment key={index}>
                        <img src={`${BASE_IMAGE_URL}items/TFT_Item_${component}`} alt={component} />
                        {index < top5Components.length - 1 && <MdKeyboardArrowRight size={24} color="white" />}
                      </React.Fragment>
                    ))}
                  </div>
                </div>
              )}
              {itemHolders.length && (
                <div className="app__viewguide-guide-carries">
                  <h3 className="p__opensans">Item Holders</h3>
                  {itemHolders.map((hex, index) => (
                    <div key={index} className="app__viewguide-guide-carry">
                      <UnitView unit={hex.unit} isStarred={hex.isStarred} />
                      <div className="app__viewguide-guide-carry-items">
                        {hex.currentItems.map((item, index) => (
                          <div key={index} className="app__viewguide-guide-carry-item">
                            <div className="app__viewguide-guide-carry-item-details">
                              <ItemView item={item} width={32} />
                              <div className="app__viewguide-guide-carry-item-details-container">
                                <p className="p__desc-text">{item.name}</p>
                                <div></div>
                              </div>
                            </div>
                            {item.recipe && (
                              <div className="app__viewguide-guide-item-recipe">
                                {item.recipe.map((component, index) => (
                                  <img
                                    key={index}
                                    src={`${IMAGE_ENDPOINTS.ITEMS}TFT_Item_${component}`}
                                    alt={`${component}`}
                                  />
                                ))}
                              </div>
                            )}
                          </div>
                        ))}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    )
  );
};

export default ViewGuide;
