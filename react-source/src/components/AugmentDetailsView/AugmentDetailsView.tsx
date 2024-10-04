import "./augmentdetailsview.css";

import Button from "../Button/Button";
import { MdDeleteOutline } from "react-icons/md";
import { CoOccurrenceTYPES } from "../SelectedView/SelectedView";
import { Augment } from "../../data";
import DropDownSelection from "../DropDownSelection/DropDownSelection";
import { getImageUrl } from "../../utils/getImageUrl";

interface AugmentDetailsViewProps {
  augment: Augment;
  onTypeChange: (key: string) => void;
  onClick: () => void;
}

const AugmentDetailsView = ({ augment, onTypeChange, onClick }: AugmentDetailsViewProps) => {
  return (
    <div className="app__augmentdetailsview">
      <div className="app__augmentdetailsview-header">
        <div className="app__augmentdetailsview-augment">
          <img
            src={getImageUrl(augment.inGameKey, "augments")}
            alt={augment.name}
            className={`app__augmentdetailsview-img tier${augment.tier}`}
          />
        </div>
        <div className="app__augmentdetailsview-content">
          <p className="p__opensans-title p__bold">{augment.name}</p>
          <p className="p__desc-text" dangerouslySetInnerHTML={{ __html: augment.desc }}></p>
        </div>
      </div>
      <div className="app__augmentdetailsview-footer">
        <div className="app__augmentdetailsview-buttons">
          <DropDownSelection items={CoOccurrenceTYPES} onSelect={onTypeChange} />
          <Button onClick={onClick} aria-label={`Remove ${augment.name} selection`}>
            <MdDeleteOutline size={15} />
            Remove Selection
          </Button>
        </div>
      </div>
    </div>
  );
};

export default AugmentDetailsView;
