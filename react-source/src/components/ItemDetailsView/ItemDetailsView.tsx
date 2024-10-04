import "./itemdetailsview.css";
import StatCard from "../StatCard/StatCard";
import DropDownSelection from "../DropDownSelection/DropDownSelection";
import Button from "../Button/Button";
import { CoOccurrenceTYPES } from "../SelectedView/SelectedView";
import { MdDeleteOutline } from "react-icons/md";
import { Item } from "../../data";
import { getStatTypes } from "../../utils";
import { getImageUrl } from "../../utils/getImageUrl";

interface ItemDetailsViewProps {
  item: Item;
  onTypeChange: (key: string) => void;
  onClick: () => void;
}

const ItemDetailsView = ({ item, onTypeChange, onClick }: ItemDetailsViewProps) => {
  return (
    <div className="app__itemdetailsview">
      <div className="app__itemdetailsview-header">
        <div className="app__itemdetailsview-item">
          <img src={getImageUrl(item.inGameKey, "items")} alt={item.name} />
        </div>
        <div>
          <p className="p__opensans-title p__bold">{item.name}</p>
          <p className="p__desc-text" dangerouslySetInnerHTML={{ __html: item.desc }}></p>
        </div>
      </div>
      <div className="app__itemdetailsview-body">
        <div className="app__stats">
          {getStatTypes(item).map((statType, index) => {
            return <StatCard key={index} stat={statType.desc} title={statType.stat} />;
          })}
        </div>
        <div>
          {!!item.recipe.length && (
            <div className="app__itemdetailsview-recipe">
              <p className="p__opensans p__bold">Recipe:</p>
              {item.recipe.map((component, index) => (
                <img key={index} src={getImageUrl(`${component}`, "items")} alt={`${component}`} />
              ))}
            </div>
          )}
          {!!item.affectedTraitKey && (
            <p className="p__opensans">
              <span className="p__bold">Affected Trait:</span> {item.affectedTraitKey}
            </p>
          )}
        </div>
      </div>
      <div className="app__itemdetailsview-footer">
        <div className="app__itemdetailsview-buttons">
          <DropDownSelection items={CoOccurrenceTYPES} onSelect={onTypeChange} />
          <Button onClick={onClick} aria-label={`Remove ${item.name} Selection`}>
            <MdDeleteOutline size={15} />
            Remove Selection
          </Button>
        </div>
      </div>
    </div>
  );
};
export default ItemDetailsView;
