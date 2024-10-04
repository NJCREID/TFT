import "./unitdetailsview.css";
import StatCard from "../StatCard/StatCard";
import DropDownSelection from "../DropDownSelection/DropDownSelection";
import { CoOccurrenceTYPES } from "../SelectedView/SelectedView";
import Button from "../Button/Button";
import { MdDeleteOutline } from "react-icons/md";
import { Unit } from "../../data";
import { getImageUrl } from "../../utils";

interface UnitDetailsViewProps {
  unit: Unit;
  onTypeChange: (key: string) => void;
  onClick: () => void;
}

const UnitDetailsView = ({ unit, onTypeChange, onClick }: UnitDetailsViewProps) => {
  return (
    <div className="app__unitdetailsview">
      <div className="app__unitdetailsview-header">
        <div className="app__unitdetailsview-unit">
          <img src={getImageUrl(unit.inGameKey, "champions")} alt={unit.name} />
          <p className="p__opensans-title p__bold">{unit.name}</p>
        </div>
        <div className="app__unitdetailsview-content">
          <div className="app__unitdetailsview-content-container">
            <img src={unit.skill?.imageUrl} alt={unit.skill?.name} />
            <p className="p__opensans">{unit.skill?.name}</p>
          </div>
          <p className="p__desc-text-small" dangerouslySetInnerHTML={{ __html: unit.skill!.desc }}></p>
        </div>
      </div>
      <div className="app__unitdetailsview-body">
        <div>
          <StatCard stat={unit.health!} title="Health" />
          <StatCard stat={unit.attackDamage!} title="Damage" />
          <StatCard stat={unit.damagePerSecond!} title="DPS" />
          <StatCard stat={unit.magicalResistance!} title="MR" />
        </div>
        <div>
          <StatCard stat={unit.armor!} title="Armor" />
          <StatCard stat={unit.attackSpeed!} title="Speed" />
          <StatCard stat={[unit.skill!.startingMana, unit.skill!.skillMana]} title="Mana" />
          <StatCard stat={unit.attackRange!} title="Range" />
        </div>
      </div>
      <div className="app__unitdetailsview-footer">
        <div className="app__unitdetailsview-buttons">
          <DropDownSelection items={CoOccurrenceTYPES} onSelect={onTypeChange} />
          <Button onClick={onClick} aria-label={`Remove ${unit.name} Selection`}>
            <MdDeleteOutline size={15} />
            Remove Selection
          </Button>
        </div>
      </div>
    </div>
  );
};

export default UnitDetailsView;
