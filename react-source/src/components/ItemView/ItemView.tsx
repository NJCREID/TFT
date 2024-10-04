import { clearModalContent, setModalContent } from "../../store/modalSlice";
import { getElementPosition, getImageUrl } from "../../utils";
import { useDispatch } from "react-redux";
import "./ItemView.css";
import { Augment, Item } from "../../data";

interface ItemViewProps {
  item: Item | Augment;
  width?: number | string;
  height?: number | string;
}

const ItemView = ({ item, width = 48, height = width }: ItemViewProps) => {
  const dispatch = useDispatch();

  const handleMouseEnter = (e: React.MouseEvent) => {
    const position = getElementPosition(e.currentTarget as HTMLElement);
    dispatch(
      setModalContent({
        content: item,
        position,
      })
    );
  };

  const handleMouseLeave = () => {
    dispatch(clearModalContent());
  };

  return (
    <img
      className="app__itemview"
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
      src={getImageUrl(item.inGameKey, "items")}
      alt={item.inGameKey}
      draggable="false"
      style={{ width, height }}
    />
  );
};

export default ItemView;
