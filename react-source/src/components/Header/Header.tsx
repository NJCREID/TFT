import { useEffect, useState } from "react";
import { FaRegUserCircle } from "react-icons/fa";
import { useWindowDimensions } from "../../context/windowDimensionContext";
import { useAuthContext } from "../../context/authContext";
import { Link } from "react-router-dom";
import { generateSlug, getUserMenuIcon } from "../../utils";
import { MdOutlineMenu } from "react-icons/md";
import "./header.css";

const menuItems = ["Sign In", "Register"];
const userMenuItems = ["My Page", "Settings", "Log out"];

interface HeaderProps {
  onMenuClick: React.Dispatch<React.SetStateAction<boolean>>;
}

const Header = ({ onMenuClick }: HeaderProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const { width } = useWindowDimensions();
  const { user, logout } = useAuthContext();
  const displayItems = user ? userMenuItems : menuItems;

  const toggleUserMenu = () => {
    setIsOpen((prev) => !prev);
  };

  const toggleSidebar = () => {
    onMenuClick((prev) => !prev);
  };

  const handleOutsideClick = (event: globalThis.MouseEvent) => {
    const userMenuElement = document.querySelector(".app__header-menu");
    if (userMenuElement && !userMenuElement.contains(event?.target as Node)) {
      setIsOpen(false);
    }
  };

  useEffect(() => {
    if (isOpen) {
      window.addEventListener("click", handleOutsideClick);
    }
    return () => {
      window.removeEventListener("click", handleOutsideClick);
    };
  }, [isOpen]);

  return (
    <nav className={`app__header ${width < 750 ? "gradient-purple-alt" : "gradient-purple"}`}>
      <div className="app__header-sidebarmenu">
        <MdOutlineMenu className="app__header-sidebarmenu-icon" onClick={toggleSidebar} />
      </div>
      <div className="app__header-homelink">
        <Link to="/">
          <img
            src={width < 750 ? "/images/general/logo_lightning.svg" : "/images/general/logo_text.svg"}
            alt="TeamTactixs Logo"
            className="app__header-homelink-img"
          />
        </Link>
      </div>
      <div className="app__header-menu">
        {user && user.user.profileImageUrl ? (
          <img
            src={user.user.profileImageUrl}
            alt="User"
            className="app__header-menu-usericon"
            onClick={toggleUserMenu}
            style={{
              border: "3px solid white",
              borderRadius: "50%",
              objectFit: "cover",
            }}
          />
        ) : (
          <FaRegUserCircle className="app__header-menu-usericon" onClick={toggleUserMenu} />
        )}
        <ul className={`app__header-menu-items ${isOpen ? "open" : ""}`}>
          {displayItems.map((item) => (
            <li key={item} className="app__header-menu-items-item p__opensans">
              {item === "Log out" ? (
                <a onClick={logout}>
                  {getUserMenuIcon(item)}
                  <p>{item}</p>
                </a>
              ) : (
                <Link to={generateSlug(item)}>
                  {getUserMenuIcon(item)}
                  <p>{item}</p>
                </Link>
              )}
            </li>
          ))}
        </ul>
      </div>
    </nav>
  );
};
export default Header;
