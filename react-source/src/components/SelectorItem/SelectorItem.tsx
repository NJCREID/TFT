import { useCallback } from "react";
import { isItem, isTrait, isUnit } from "../../utils";
import UnitView from "../UnitView/UnitView";
import ItemView from "../ItemView/ItemView";
import AugmentView from "../AugmentView/AugmentView";
import { setModalContent } from "../../store/modalSlice";
import { useDispatch } from "react-redux";
import { setDraggedItem } from "../../store/dragSlice";
import "./selectoritem.css";
import TraitView from "../TraitView/TraitView";
import { Augment, Item, Trait, Unit } from "../../data";

interface SelectorItemProps {
  object: Unit | Item | Augment | Trait;
  onDragEnd?: (e: React.DragEvent) => void;
}

const SelectorItem = ({ object, onDragEnd }: SelectorItemProps) => {
  const dispatch = useDispatch();

  const handleDragStart = useCallback(() => {
    dispatch(setDraggedItem(object));
    dispatch(
      setModalContent({
        content: null,
        position: null,
      })
    );
  }, [dispatch, object]);

  const handleDragFinish = useCallback(
    (e: React.DragEvent) => {
      if (onDragEnd) onDragEnd(e);
      dispatch(setDraggedItem(null));
    },
    [dispatch, onDragEnd]
  );

  return (
    <div draggable className="app__selectoritem" onDragStart={handleDragStart} onDragEnd={handleDragFinish}>
      {isUnit(object) ? (
        <UnitView unit={object} />
      ) : isItem(object) ? (
        <ItemView item={object} />
      ) : isTrait(object) ? (
        <TraitView trait={object} />
      ) : (
        <AugmentView augment={object} />
      )}
    </div>
  );
};

export default SelectorItem;
