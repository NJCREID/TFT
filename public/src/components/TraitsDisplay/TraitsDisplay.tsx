import { UserGuideTrait } from "../../common/api";
import GenerateTier from "../../utils/GenerateTier";
import "./traitsdisplay.css";

const TraitsDisplay = ({ userGuideTraits }: { userGuideTraits: UserGuideTrait[] }) => {
  let newTraits = [...userGuideTraits].map((userGuideTrait) => {
    return {
      ...userGuideTrait,
      tier: GenerateTier(userGuideTrait.trait.key, userGuideTrait.value),
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
              key={userGuideTrait.trait.key}
              className={`app__traitsdisplay-traits-trait ${userGuideTrait.tier > 0 ? "active" : ""}`}
            >
              <img src={userGuideTrait.trait.imageUrl} alt={userGuideTrait.trait.key} />
              <div className="app__traitsdisplay-traits-trait-value">
                <p className="p__opensans">{userGuideTrait.value}</p>
              </div>
              <div className="app__traitsdisplay-traits-trait-container">
                <p className="p__opensans">{userGuideTrait.trait.key}</p>
                <p className="p__opensans">{userGuideTrait.trait.tierString}</p>
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
