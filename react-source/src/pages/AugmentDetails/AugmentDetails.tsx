import { CoOccurrenceStats } from "../../components";
import "./augmentdetails.css";

const AugmentDetails = () => {
  return (
    <div className="app__augmentdetails page_padding">
      <CoOccurrenceStats type="augment" />
    </div>
  );
};

export default AugmentDetails;
