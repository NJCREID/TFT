import { useEffect, useLayoutEffect, useRef } from "react";
import { IMAGE_ENDPOINTS } from "../../data/endpoints";
import { useSelector } from "react-redux";
import { RootState } from "../../store";
import { Augment, Item, Unit } from "../../common/api";
import { isItem, isUnit } from "../../utils/isObject";
import AugmentView from "../AugmentView/AugmentView";
import ItemView from "../ItemView/ItemView";
import coin from "../../assets/coin.svg";
import { useModalPosition } from "../../hooks/useModalPosition";
import "./modal.css";

const Modal = () => {
  const { modalContent, modalPosition } = useSelector((state: RootState) => state.modal);
  const { position, modalRef } = useModalPosition(modalPosition);

  useEffect(() => {
    document.querySelector(".app__modal")?.classList.add("visible");
  }, [position]);

  if (!modalContent || !modalPosition) return;

  const renderModalContent = () => {
    if (isUnit(modalContent)) return <UnitModal unit={modalContent as Unit} />;
    if (isItem(modalContent)) return <ItemModal item={modalContent as Item} />;
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
  const pRef = useRef<HTMLDivElement>(null);

  useLayoutEffect(() => {
    if (pRef.current) {
      pRef.current.innerHTML = item.desc;
    }
  }, []);

  return (
    <div className="app__modal-item-container">
      <div className="app__modal-item-header">
        <div className="app__modal-item-header-container">
          <ItemView item={item} />
          <div className="app__modal-item-header-name p__opensans">{item.name}</div>
        </div>
        <p className="p__desc-text" ref={pRef}></p>
      </div>
      {item.recipe?.length ? (
        <div className="app__modal-item-body">
          <div className="app__modal-item-body-recipe">
            <p className="p__opensans">Recipe:</p>
            <div className="app__modal-item-body-recipe-components">
              {item.recipe.map((component, index) => (
                <img key={index} src={`${IMAGE_ENDPOINTS.ITEMS}TFT_Item_${component}`} alt={component} />
              ))}
            </div>
          </div>
        </div>
      ) : null}
    </div>
  );
}

function AugmentModal({ augment }: { augment: Augment }) {
  const pRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (pRef.current) {
      pRef.current.innerHTML = augment.desc;
    }
  }, []);

  return (
    <div className="app__modal-item-container">
      <div className="app__modal-item-header">
        <div className="app__modal-item-header-container">
          <AugmentView augment={augment} />
          <div className="app__modal-item-header-name p__opensans">{augment.name}</div>
        </div>
        <p className="p__desc-text" ref={pRef}></p>
      </div>
    </div>
  );
}

function UnitModal({ unit }: { unit: Unit }) {
  return (
    <div className="app__modal-unit-container">
      <div className="app__modal-unit-header">
        <img src={unit.splashImageUrl} alt={unit.name} />
        <div className="app__modal-unit-header-traits">
          {unit.traits.map((trait) => (
            <div key={trait.key} className="app__modal-unit-header-traits-trait">
              <img src={trait.imageUrl} alt={trait.key} />
              <p className="p__opensans-xs p__bold">{trait.key}</p>
            </div>
          ))}
        </div>
        <div className="app__modal-unit-header-items">
          {unit.recommendedItems.slice(0, 3).map((item) => (
            <div key={item} className="app__modal-unit-header-items-item">
              <img src={`${IMAGE_ENDPOINTS.ITEMS}${item}.png`} alt={item} />
            </div>
          ))}
        </div>
      </div>
      <div className="app__modal-unit-body">
        <div className="app__modal-unit-body-container">
          <p className="p__opensans p__bold">{unit.name}</p>
          <div className="app__modal-unit-body-cost">
            <img src={coin} alt="coin" />
            <p className="p__opensans p__bold">{unit.cost[0]}</p>
          </div>
        </div>
      </div>
    </div>
  );
}
