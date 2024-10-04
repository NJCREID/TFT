import { FaChevronRight } from "react-icons/fa";
import "./profileeditcontrol.css";

interface ProfileEditControlProps {
  openModal: (type: string) => void;
  type: string;
  currentValue: string | null;
}

const ProfileEditControl = ({ openModal, type, currentValue }: ProfileEditControlProps) => {
  return (
    <div className="app__profileeditcontrol underline_animation-hover">
      <button onClick={() => openModal(type)} className="app__profileeditcontrol-editbtn">
        <div className="app__profileeditcontrol-container">
          <span className="p__opensans-title underline_animation">{type}</span>
          <span className="p__desc-text">{currentValue}</span>
        </div>
        <span className="app__profileeditcontrol-arrow">
          <FaChevronRight size={20} color="white" />
        </span>
      </button>
    </div>
  );
};

export default ProfileEditControl;
