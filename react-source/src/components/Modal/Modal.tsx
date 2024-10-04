import { useEffect } from "react";
import { useSelector } from "react-redux";
import { RootState } from "../../store";
import AugmentView from "../AugmentView/AugmentView";
import ItemView from "../ItemView/ItemView";
import TraitView from "../TraitView/TraitView";
import { useModalPosition } from "../../hooks";
import { getImageUrl, isItem, isTrait, isUnit } from "../../utils";
import { Augment, Item, Trait, Unit } from "../../data";
import "./modal.css";

const Modal = () => {
  const { modalContent, modalPosition } = useSelector((state: RootState) => state.modal);
  const { position, modalRef } = useModalPosition(modalPosition);

  useEffect(() => {
    document.querySelector(".app__modal")?.classList.add("visible");
  }, [position]);

  if (!modalContent || !modalPosition) return;

  const renderModalContent = () => {
    if (isUnit(modalContent)) return <UnitModal unit={modalContent} />;
    if (isItem(modalContent)) return <ItemModal item={modalContent} />;
    if (isTrait(modalContent)) return <TraitModal trait={modalContent} />;
    return <AugmentModal augment={modalContent as Augment} />;
  };

  return (
    <div className="app__modal" style={{ top: position.y + "px", left: position.x + "px" }} ref={modalRef}>
      {renderModalContent()}
    </div>
  );
};

export default Modal;

function ItemModal({ item }: { item: Item }) {
  return (
    <div className="app__modal-item-container">
      <div className="app__modal-item-header">
        <div className="app__modal-item-header-container">
          <ItemView item={item} />
          <div className="app__modal-item-header-name p__opensans">{item.name}</div>
        </div>
        <p className="p__desc-text" dangerouslySetInnerHTML={{ __html: item.desc }}></p>
      </div>
      {item.recipe?.length ? (
        <div className="app__modal-item-body">
          <div className="app__modal-item-body-recipe">
            <p className="p__opensans">Recipe:</p>
            <div className="app__modal-item-body-recipe-components">
              {item.recipe.map((component, index) => (
                <img key={index} src={getImageUrl(`${component}`, "items")} alt={component} />
              ))}
            </div>
          </div>
        </div>
      ) : null}
    </div>
  );
}

function AugmentModal({ augment }: { augment: Augment }) {
  return (
    <div className="app__modal-item-container">
      <div className="app__modal-item-header">
        <div className="app__modal-item-header-container">
          <AugmentView augment={augment} />
          <div className="app__modal-item-header-name p__opensans">{augment.name}</div>
        </div>
        <p className="p__desc-text" dangerouslySetInnerHTML={{ __html: augment.desc }}></p>
      </div>
    </div>
  );
}

function TraitModal({ trait }: { trait: Trait }) {
  const descHTML =
    trait.desc && trait.desc.length > 0
      ? trait.desc
      : Object.entries(trait.stats || {}).reduce((acc, [key, value]) => {
          return acc + `${key}: ${value}<br>`;
        }, "");

  return (
    <div className="app__modal-item-container">
      <div className="app__modal-item-header">
        <div className="app__modal-item-header-container">
          <TraitView trait={trait} />
          <div className="app__modal-item-header-name p__opensans">{trait.name}</div>
        </div>
        <p className="p__desc-text" dangerouslySetInnerHTML={{ __html: descHTML }}></p>
      </div>
    </div>
  );
}

function UnitModal({ unit }: { unit: Unit }) {
  const imageUrl = getImageUrl(unit.inGameKey, "champions", "splash");
  return (
    <div className="app__modal-unit-container">
      <div className="app__modal-unit-header">
        <img src={imageUrl} alt={unit.name} />
        <div className="app__modal-unit-header-traits">
          {unit.traits.map((trait) => (
            <div key={trait.inGameKey} className="app__modal-unit-header-traits-trait">
              <img src={getImageUrl(trait.inGameKey, "traits")} alt={trait.inGameKey} />
              <p className="p__opensans-xs p__bold">{trait.name}</p>
            </div>
          ))}
        </div>
        <div className="app__modal-unit-header-items">
          {unit.recommendedItems?.slice(0, 3).map((item) => (
            <div key={item} className="app__modal-unit-header-items-item">
              <img src={getImageUrl(item, "items")} alt={item} />
            </div>
          ))}
        </div>
      </div>
      <div className="app__modal-unit-body">
        <div className="app__modal-unit-body-container">
          <p className="p__opensans p__bold">{unit.name}</p>
          <div className="app__modal-unit-body-cost">
            <img src={"/images/general/coin.svg"} alt="coin" />
            <p className="p__opensans p__bold">{unit.cost[0]}</p>
          </div>
        </div>
      </div>
    </div>
  );
}
