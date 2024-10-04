import { UserGuideTrait } from "../../data/GuideData";
import { generateTierColor, getImageUrl } from "../../utils";
import "./traititem.css";

export default function TraitItem({ userGuideTrait }: { userGuideTrait: UserGuideTrait }) {
  const { backgroundStyle, imageStyle } = generateTierColor({
    tiers: userGuideTrait.tiers,
    amount: userGuideTrait.value,
  });
  return (
    <div className="app__trait" style={backgroundStyle}>
      <img style={imageStyle} src={getImageUrl(userGuideTrait.inGameKey, "traits")} alt={userGuideTrait.name} />
      <p className="p__opensans">{userGuideTrait.value}</p>
    </div>
  );
}
