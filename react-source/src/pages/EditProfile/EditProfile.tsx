import { useState } from "react";
import EditModal from "../../components/EditModal/EditModal";
import { useAuthContext } from "../../context";
import { useNavigate } from "react-router-dom";
import ProfileEditControl from "../../components/ProfileEditControl/ProfileEditControl";
import fetchRequest from "../../common/api";
import { User } from "../../context/authContext";
import { ENDPOINT } from "../../common/endpoints";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import "./editprofile.css";

const updateEndpoints: { [key: string]: string } = {
  "Profile Image": ENDPOINT.UPDATE_USER_IMAGE,
  Name: ENDPOINT.UPDATE_USER_NAME,
  Username: ENDPOINT.UPDATE_USER_USERNAME,
  Email: ENDPOINT.UPDATE_USER_EMAIL,
  Password: ENDPOINT.UPDATE_USER_PASSWORD,
};

const EditProfile = () => {
  const [currentField, setCurrentField] = useState<string | null>(null);
  const { user, updateUser } = useAuthContext();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  if (!user) {
    navigate("/sign-in");
    return;
  }

  const openModal = (field: string) => {
    setCurrentField(field);
  };

  const closeModal = () => {
    setCurrentField(null);
  };

  const handleSave = async (newValue: string | File) => {
    if (!currentField) return;
    const isProfileImage = currentField === "Profile Image";
    const field = isProfileImage ? "profileImageUrl" : currentField.toLowerCase();

    try {
      const endpoint = updateEndpoints[currentField];
      const method = isProfileImage ? "POST" : "PATCH";

      let body;
      if (isProfileImage && newValue instanceof File) {
        body = new FormData();
        body.append("profileImage", newValue);
        body.append("userId", user.user.id.toString());
      } else {
        body = JSON.stringify({
          ...user.user,
          [field]: newValue,
        });
      }
      const returnedUser = await fetchRequest<User>({
        endpoint: endpoint,
        method,
        body,
        identifier: user.user.id.toString(),
        authToken: user?.token,
      });
      updateUser(returnedUser);
    } catch (error) {
      if (error instanceof Error) {
        dispatch(showError([`Failed to update user: ${error.message}`]));
      } else {
        dispatch(showError(["An unknown error occurred while updating user."]));
      }
    }
  };

  const editProfileItems = [
    {
      type: "Name",
      currentValue: user.user.name,
    },
    {
      type: "Username",
      currentValue: user.user.username,
    },
    {
      type: "Email",
      currentValue: user.user.email,
    },
    {
      type: "Password",
      currentValue: null,
    },
    {
      type: "Profile Image",
      currentValue: user.user.profileImageUrl,
    },
  ];

  return (
    <div className="app__editprofile page_padding">
      <div className="app__editprofile-wrapper ">
        <p className="p__opensans-title-large">Profile Settings</p>
        <div className="app__editprofile-settings">
          <p className="p__opensans-title p__bold">General</p>
          {editProfileItems.map((profileItem, index) => (
            <ProfileEditControl
              key={index}
              openModal={openModal}
              type={profileItem.type}
              currentValue={profileItem.currentValue}
            />
          ))}
        </div>
        {currentField && <EditModal field={currentField} closeModal={closeModal} handleSave={handleSave} />}
      </div>
    </div>
  );
};

export default EditProfile;
