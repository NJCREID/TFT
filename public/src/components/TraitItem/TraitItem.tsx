import { UserGuideTrait } from "../../common/api.js";
import GenerateTierColor from "../../utils/GenerateTierColor.js";
import "./traititem.css";

export default function TraitItem({ userGuideTrait }: { userGuideTrait: UserGuideTrait }) {
  const { backgroundStyle, imageStyle } = GenerateTierColor({
    traitName: userGuideTrait.trait.name,
    amount: userGuideTrait.value,
  });
  return (
    <div className="app__trait" style={backgroundStyle}>
      <img style={imageStyle} src={userGuideTrait.trait.imageUrl} alt={userGuideTrait.trait.name} />
      <p className="p__opensans">{userGuideTrait.value}</p>
    </div>
  );
}
