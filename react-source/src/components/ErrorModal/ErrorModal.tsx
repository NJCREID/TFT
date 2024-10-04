import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../../store";
import "./errormodal.css";
import { clearError } from "../../store/errorModalSlice";

export const ErrorModal = () => {
  const dispatch = useDispatch();
  const messages = useSelector((state: RootState) => state.errorModal.messages);
  const [progressKey, setProgressKey] = useState(0);

  useEffect(() => {
    if (messages.length > 0) {
      setProgressKey((prevKey) => prevKey + 1);
      const timer = setTimeout(() => {
        dispatch(clearError());
      }, 3000);
      return () => clearTimeout(timer);
    }
  }, [messages, dispatch]);

  if (!messages.length) return null;

  return (
    <div className="error-modal">
      <div className="error-modal-messages">
        {messages.map((message, index) => (
          <p key={index} className="p__opensans">
            {message}
          </p>
        ))}
      </div>
      <div className="error-modal-progress-bar">
        <div className="error-modal-progress" key={progressKey} />
      </div>
    </div>
  );
};
