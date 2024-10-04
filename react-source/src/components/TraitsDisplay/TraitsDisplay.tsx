import { UserGuideTrait } from "../../data";
import { generateTier, getImageUrl } from "../../utils";
import "./traitsdisplay.css";

const TraitsDisplay = ({ userGuideTraits }: { userGuideTraits: UserGuideTrait[] }) => {
  let newTraits = [...userGuideTraits].map((userGuideTrait) => {
    return {
      ...userGuideTrait,
      tier: generateTier(userGuideTrait.tiers, userGuideTrait.value),
    };
  });

  newTraits = newTraits.sort((a, b) => {
    if (a.tier !== b.tier) return b.tier - a.tier;
    return b.value! - a.value!;
  });

  return (
    <div className="app__traitsdisplay">
      {newTraits.length ? (
        <div className="app__traitsdisplay-traits">
          {newTraits.map((userGuideTrait) => (
            <div
              key={userGuideTrait.inGameKey}
              className={`app__traitsdisplay-traits-trait ${userGuideTrait.tier > 0 ? "active" : ""}`}
            >
              <img src={getImageUrl(userGuideTrait.inGameKey, "traits")} alt={userGuideTrait.name} />
              <div className="app__traitsdisplay-traits-trait-value">
                <p className="p__opensans">{userGuideTrait.value}</p>
              </div>
              <div className="app__traitsdisplay-traits-trait-container">
                <p className="p__opensans">{userGuideTrait.name}</p>
                <p className="p__opensans">{userGuideTrait.tierString}</p>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <div className="app__traitsdisplay-empty">
          <p className="p__opensans p__bold">Start building a team to see traits</p>
        </div>
      )}
    </div>
  );
};
export default TraitsDisplay;
