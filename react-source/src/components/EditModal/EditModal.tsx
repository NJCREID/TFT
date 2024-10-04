import { ChangeEvent, useState } from "react";
import Button from "../Button/Button";
import "./editmodal.css";

interface EditModalProps {
  field: string;
  closeModal: () => void;
  handleSave: (newValue: string | File) => void;
}

const EditModal = ({ field, closeModal, handleSave }: EditModalProps) => {
  const [err, setErr] = useState<string | null>(null);
  const [fileName, setFileName] = useState("No file chosen");

  const onSave = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const form = e.currentTarget as HTMLFormElement;

    let newValue: string | File;
    if (field === "Profile Image") {
      const fileInput = form.elements.namedItem("newValue") as HTMLInputElement;
      newValue = fileInput.files?.[0] || "";
    } else {
      newValue = (form.elements.namedItem("newValue") as HTMLInputElement).value;
      if (field === "Password" || "Email") {
        const confirmValue = (form.elements.namedItem("confirmValue") as HTMLInputElement).value;
        if (newValue !== confirmValue) {
          setErr(`${field}s do not match.`);
        }
      }
    }
    handleSave(newValue);
    closeModal();
  };

  const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      const name = file.name;
      const type = file.type.split("/").pop();
      const maxLength = 25;

      let formattedName = name;
      if (name.length > maxLength) {
        const lastDotIndex = name.lastIndexOf(".");
        const baseName = name.slice(0, lastDotIndex);
        const lastTwo = baseName.slice(-3);
        const truncatedBaseName = baseName.slice(0, maxLength - 5 - lastTwo.length);
        formattedName = `${truncatedBaseName}...${lastTwo}.${type}`;
      } else {
        formattedName = name;
      }
      setFileName(formattedName);
    } else {
      setFileName("No file chosen");
    }
  };

  const getPattern = () => {
    switch (field) {
      case "Email":
        return "[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,}$";
      case "Password":
        return "^(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?\"':{}|<>]).{8,}$";
      case "Name":
      case "Username":
        return ".{6,}";
      default:
        return ".*";
    }
  };

  return (
    <div className="app__editmodal">
      <form className="app__editmodal-container" onSubmit={onSave}>
        <p className="p__opensans-title-large">Edit {field}</p>
        {field === "Profile Image" ? (
          <label className="app__editmodal-image">
            <input type="file" name="newValue" accept="image/*" onChange={handleFileChange} />
            <span className="p__opensans-black app__editmodal-image-file" id="fileName">
              {fileName}
            </span>
          </label>
        ) : (
          <>
            <input
              type={field === "Email" ? "email" : field === "Password" ? "password" : "text"}
              name="newValue"
              placeholder={`Enter new ${field}`}
              pattern={getPattern()}
              className="app__editmodal-input"
              autoComplete="off"
            />
            {(field === "Password" || field === "Email") && (
              <input
                type={field === "Password" ? "password" : "email"}
                name="confirmValue"
                placeholder={`Confirm new ${field}`}
                pattern={getPattern()}
                className="app__editmodal-input"
                autoComplete="off"
              />
            )}
          </>
        )}
        <div className="app__editmodal-buttons">
          <Button onClick={closeModal} aria-label="Cancel editing">
            Cancel
          </Button>
          <Button type="submit" aria-label={`Save new ${field}`}>
            Save
          </Button>
        </div>
      </form>
      {err && <p className="error-text">{err}</p>}
    </div>
  );
};

export default EditModal;
