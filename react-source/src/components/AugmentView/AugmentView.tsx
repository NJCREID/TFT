import "./augmentview.css";
import { clearModalContent, setModalContent } from "../../store/modalSlice";
import { getElementPosition } from "../../utils";
import { useDispatch } from "react-redux";
import { Augment } from "../../data";
import { getImageUrl } from "../../utils/getImageUrl";

interface AugmentViewProps {
  augment: Augment;
  width?: number | string;
  height?: number | string;
}

const AugmentView = ({ augment, width = 48, height = width }: AugmentViewProps) => {
  const dispatch = useDispatch();

  const handleMouseEnter = (e: React.MouseEvent) => {
    const position = getElementPosition(e.currentTarget as HTMLElement);
    dispatch(
      setModalContent({
        content: augment,
        position,
      })
    );
  };

  const handleMouseLeave = () => {
    dispatch(clearModalContent());
  };

  return (
    <img
      src={getImageUrl(augment.inGameKey, "augments")}
      alt={augment.name}
      className={`app__augmentview tier${augment.tier}`}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
      style={{ width, height }}
    />
  );
};

export default AugmentView;
