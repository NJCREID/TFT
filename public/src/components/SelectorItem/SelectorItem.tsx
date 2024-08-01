import { useCallback } from "react";
import { Augment, Item, Unit } from "../../common/api";
import { isItem, isUnit } from "../../utils/isObject";
import UnitView from "../UnitView/UnitView";
import ItemView from "../ItemView/ItemView";
import AugmentView from "../AugmentView/AugmentView";
import "./selectoritem.css";
import { setModalContent } from "../../store/modalSlice";
import { useDispatch } from "react-redux";
import { setDraggedItem } from "../../store/dragSlice";

interface SelectorItemProps {
  item: Unit | Item | Augment;
  handleDragEnd?: (e: React.DragEvent) => void;
}

const SelectorItem = ({ item, handleDragEnd }: SelectorItemProps) => {
  const dispatch = useDispatch();

  const handleDragStart = useCallback(
    (e: React.DragEvent) => {
      if (handleDragEnd) handleDragEnd(e);
      dispatch(setDraggedItem(item));
      dispatch(
        setModalContent({
          content: null,
          position: null,
        })
      );
    },
    [dispatch, item]
  );

  const handleDragFinish = useCallback(() => {
    dispatch(setDraggedItem(null));
  }, [dispatch]);

  return (
    <div draggable className="app__selectoritem" onDragStart={(e) => handleDragStart(e)} onDragEnd={handleDragFinish}>
      {isUnit(item) ? (
        <UnitView unit={item} />
      ) : isItem(item) ? (
        <ItemView item={item} />
      ) : (
        <AugmentView augment={item} />
      )}
    </div>
  );
};

export default SelectorItem;
