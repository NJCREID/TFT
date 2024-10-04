import { Trait } from "../../data/TraitData";
import { clearModalContent, setModalContent } from "../../store/modalSlice";
import { getElementPosition, getImageUrl } from "../../utils";
import "./traitview.css";
import { useDispatch } from "react-redux";

interface TraitViewProps {
  trait: Trait;
  width?: string | number;
  height?: string | number;
}

const TraitView = ({ trait, width = 48, height = width }: TraitViewProps) => {
  const dispatch = useDispatch();

  const handleMouseEnter = (e: React.MouseEvent) => {
    const position = getElementPosition(e.currentTarget as HTMLElement);
    dispatch(
      setModalContent({
        content: trait,
        position,
      })
    );
  };

  const handleMouseLeave = () => {
    dispatch(clearModalContent());
  };

  return (
    <img
      src={getImageUrl(trait.inGameKey, "traits")}
      alt={trait.name}
      className="app__traitview"
      style={{ width, height }}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
    />
  );
};

export default TraitView;
