import FilterSelector from "../FilterSelector/FilterSelector";
import DropDownSelection from "../DropDownSelection/DropDownSelection";
import Button from "../Button/Button";
import { MdDeleteOutline } from "react-icons/md";
import { CoOccurrenceTYPES } from "../SelectedView/SelectedView";
import "./traitdetailsview.css";
import { Trait } from "../../data";
import { getImageUrl } from "../../utils";

interface TraitDetailsViewProps {
  trait: Trait;
  onTierChange: (tier: string) => void;
  onTypeChange: (key: string) => void;
  onClick: () => void;
}

const TraitDetailsView = ({ trait, onTierChange, onTypeChange, onClick }: TraitDetailsViewProps) => {
  let selectorItems = Array.from(trait.tiers!).map((tier) => ({
    key: tier.level.toString(),
    name: tier.level.toString(),
  }));

  const statsHTML = Object.entries(trait.stats || {})
    .map(
      ([key, value]) =>
        `<div class="app__traitdetailsview-stats-item"><dt class="p__opensans-small p__bold">${key}:</dt><dd class="p__desc-text">${value}</dd></div>`
    )
    .join("");

  return (
    <div className="app__traitdetailsview">
      <div className="app__traitdetailsview-header">
        <div className="app__traitdetailsview-trait">
          <img src={getImageUrl(trait.inGameKey, "traits")} alt={trait.name} />
        </div>
        <div className="app__traitdetailsview-content">
          <p className="p__opensans-title p__bold">{trait.name}</p>
          <p className="p__desc-text" dangerouslySetInnerHTML={{ __html: trait.desc! }}></p>
        </div>
      </div>
      <div className="app__traitdetailsview-body">
        <dl className="app__traitdetailsview-stats-list" dangerouslySetInnerHTML={{ __html: statsHTML }}></dl>
        <div className="app__traitdetailsview-tier">
          <p className="p__opensans p__bold">Tier:</p>
          <FilterSelector items={selectorItems} onSelect={onTierChange} />
        </div>
      </div>
      <div className="app__traitdetailsview-footer">
        <div className="app__traitdetailsview-buttons">
          <DropDownSelection items={CoOccurrenceTYPES} onSelect={onTypeChange} />
          <Button onClick={onClick} aria-label={`Remove ${trait.name} Selection`}>
            <MdDeleteOutline size={15} />
            Remove Selection
          </Button>
        </div>
      </div>
    </div>
  );
};

export default TraitDetailsView;
