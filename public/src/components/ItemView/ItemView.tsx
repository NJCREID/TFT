import { Augment, Item } from "../../common/api";
import "./ItemView.css";
import { clearModalContent, setModalContent } from "../../store/modalSlice";
import { getElementPosition } from "../../utils/getElementPosition";
import { useDispatch } from "react-redux";
import { useEffect } from "react";

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

  useEffect(() => {
    return () => {
      dispatch(clearModalContent());
    };
  }, [dispatch]);

  return (
    <img
      className="app__itemview"
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
      src={item.imageUrl}
      alt={item.key}
      draggable="false"
      style={{ width, height }}
    />
  );
};

export default ItemView;
