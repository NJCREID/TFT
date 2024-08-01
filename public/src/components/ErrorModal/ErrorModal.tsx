import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../../store";
import "./errormodal.css";
import { clearError } from "../../store/errorModalSlice";

export const ErrorModal = () => {
  const dispatch = useDispatch();
  const messages = useSelector((state: RootState) => state.errorModal.messages);
  const [progress, setProgress] = useState(0);

  useEffect(() => {
    setProgress(0);
    if (messages) {
      const interval = setInterval(() => {
        setProgress((prev) => {
          const newProgress = Math.min(prev + 100 / (3000 / 50), 100);
          if (newProgress === 100) {
            clearInterval(interval);
            dispatch(clearError());
          }
          return newProgress;
        });
      }, 50);
      return () => clearInterval(interval);
    }
  }, [messages, dispatch]);

  if (!messages) return null;

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
        <div className="error-modal-progress" style={{ width: `${progress}%` }} />
      </div>
    </div>
  );
};
