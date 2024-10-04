import { CoOccurrenceStats } from "../../components";
import "./traitdetails.css";

const TraitDetails = () => {
  return (
    <div className="app__traitdetails page_padding">
      <CoOccurrenceStats type="trait" />
    </div>
  );
};

export default TraitDetails;
