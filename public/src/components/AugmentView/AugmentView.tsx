import { Augment } from "../../common/api";
import "./augmentview.css";
import { clearModalContent, setModalContent } from "../../store/modalSlice";
import { getElementPosition } from "../../utils/getElementPosition";
import { useDispatch } from "react-redux";
import { useEffect } from "react";

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

  useEffect(() => {
    return () => {
      dispatch(clearModalContent());
    };
  }, [dispatch]);

  return (
    <img
      src={augment.imageUrl}
      alt={augment.name}
      className={`app__augmentview tier${augment.tier}`}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
      style={{ width, height }}
    />
  );
};

export default AugmentView;
