import "./unitdetails.css";
import { CoOccurrenceStats } from "../../components";

export default function UnitDetails() {
  return (
    <div className="app__unitdetails page_padding">
      <CoOccurrenceStats type="unit" />
    </div>
  );
}
