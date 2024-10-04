import React, { useEffect, useRef, useState } from "react";
import "./viewguide.css";
import { useParams } from "react-router-dom";
import fetchRequest from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import { useAuthContext } from "../../context";
import { MdKeyboardArrowRight } from "react-icons/md";
import { IoSend } from "react-icons/io5";
import { Link } from "react-router-dom";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import { AugmentTextView, ItemView, LoadingSpinner, StaticBoard, UnitView } from "../../components";
import { getImageUrl, sortHexes } from "../../utils";
import { Comment, Hex, Item, UserGuide } from "../../data";

const ViewGuide = () => {
  const { guideId } = useParams();
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const [guide, setGuide] = useState<UserGuide | null>(null);
  const [sortedHexes, setSortedHexes] = useState<Hex[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const { user } = useAuthContext();
  const dispatch = useDispatch();

  useEffect(() => {
    if (!user) return;
    setIsLoading(true);
    const fetchGuide = async () => {
      try {
        const fetchedGuide: UserGuide = await fetchRequest({
          endpoint: ENDPOINT.GUIDE_FETCH_ID,
          identifier: guideId,
          authToken: user.token,
        });
        setGuide(fetchedGuide);
      } catch (error) {
        if (error instanceof Error) {
          dispatch(showError([`Error fetching guide: ${error.message}`]));
        } else {
          dispatch(showError(["An Unkown Error occured while fetching guide."]));
        }
      } finally {
        setIsLoading(false);
      }
    };
    fetchGuide();
  }, [guideId, user]);

  useEffect(() => {
    if (!guide) return;
    const hexes = sortHexes(guide.hexes);
    setSortedHexes(hexes);
  }, [guide]);

  const handleComment = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!user || !guide) return;
    try {
      const { comment } = e.target as typeof e.target & { comment: HTMLTextAreaElement };
      const addedComment: Comment = await fetchRequest({
        endpoint: ENDPOINT.ADD_COMMENT,
        method: "POST",
        body: {
          UserId: user.user.id,
          UserGuideId: guide.id,
          Content: comment.value,
        },
        authToken: user.token,
      });
      comment.value = "";
      setGuide((prevGuide) => {
        if (!prevGuide) return prevGuide;
        return {
          ...prevGuide,
          comments: [...prevGuide.comments, addedComment],
        };
      });
    } catch (error) {
      if (error instanceof Error) {
        dispatch(showError([`Error creating comment: ${error.message}`]));
      } else {
        dispatch(showError(["An Unkown Error occured while creating comment."]));
      }
    }
  };

  const handleInput = () => {
    if (textareaRef.current) {
      textareaRef.current.style.height = "auto";
      textareaRef.current.style.height = `${textareaRef.current.scrollHeight}px`;
    }
  };

  const getTop5Components = (guide: UserGuide) => {
    if (!guide || !guide.hexes) return [];
    const componentCount: { [key: string]: number } = {};
    guide.hexes.forEach((hex: Hex) => {
      hex.currentItems.forEach((item: Item) => {
        if (item.isComponent) {
          if (componentCount[item.inGameKey]) {
            componentCount[item.inGameKey]++;
          } else {
            componentCount[item.inGameKey] = 1;
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

  if (isLoading) {
    return <LoadingSpinner />;
  }

  return (
    !!guide && (
      <div className="app__viewguide page_padding">
        <div className="app__viewguide-banner">
          <img
            src={getImageUrl(guide.hexes[0]?.unit?.inGameKey, "champions", "splash")}
            alt={guide.hexes[0]?.unit?.name}
          />
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
                <form className="app__viewguide-guide-comment-input" onSubmit={handleComment}>
                  <textarea
                    id="comment"
                    ref={textareaRef}
                    onChange={handleInput}
                    placeholder="Add a comment..."
                    required
                  />
                  <button>
                    <IoSend size={18} />
                  </button>
                </form>
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
                        <img src={getImageUrl(`${component}`, "items")} alt={component} />
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
                              </div>
                            </div>
                            {item.recipe && (
                              <div className="app__viewguide-guide-item-recipe">
                                {item.recipe.map((component, index) => (
                                  <img key={index} src={getImageUrl(`${component}`, "items")} alt={`${component}`} />
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
