import { Augment } from "../../common/api";
import AugmentView from "../AugmentView/AugmentView";
import "./augmenttextview.css";

const AugmentTextView = ({ augment }: { augment: Augment }) => {
  return (
    <div className="app__augmenttextview">
      <AugmentView augment={augment} />
      <div className="app__augmenttextview-details">
        <p className="p__opensans-small">{augment.name}</p>
        <p className="p__desc-text">{augment.desc}</p>
      </div>
    </div>
  );
};

export default AugmentTextView;
