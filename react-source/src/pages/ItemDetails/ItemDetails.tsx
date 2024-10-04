import { CoOccurrenceStats } from "../../components";
import "./itemdetails.css";

const ItemDetails = () => {
  return (
    <div className="app__itemdetails page_padding">
      <CoOccurrenceStats type="item" />
    </div>
  );
};

export default ItemDetails;
