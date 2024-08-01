import { NavLink } from "react-router-dom";
import logoText from "../../assets/logo_text.svg";
import logo from "../../assets/logo_lightning.svg";
import { useEffect, useState } from "react";
import { FaRegUserCircle } from "react-icons/fa";
import "./header.css";
import { useWindowDimensions } from "../../context/windowDimensionContext";
import { useAuthContext } from "../../context/authContext";
import { Link } from "react-router-dom";
import { GenerateSlug } from "../../utils/GenerateSlug";
import { GetUserMenuIcon } from "../../utils/GetIcons";

const menuItems = ["Sign In", "Register"];
const userMenuItems = ["My Page", "Settings", "Log out"];

const Header = () => {
  const [isOpen, setIsOpen] = useState(false);
  const { width } = useWindowDimensions();
  const { user, logout } = useAuthContext();
  const displayItems = user ? userMenuItems : menuItems;

  const toggleUserMenu = () => {
    setIsOpen((prev) => !prev);
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
    <nav className={`app__header ${width < 800 ? "gradient-purple-alt" : "gradient-purple"}`}>
      <div className="app__header-homelink">
        <NavLink to="/">
          <img src={width < 800 ? logo : logoText} alt="TeamTactixs Logo" className="app__header-homelink-img" />
        </NavLink>
      </div>
      <div className="app__header-menu">
        <FaRegUserCircle className="app__header-menu-usericon" onClick={toggleUserMenu} />
        <ul className={`app__header-menu-items ${isOpen ? "open" : ""}`}>
          {displayItems.map((item) => (
            <li key={item} className="app__header-menu-items-item p__opensans">
              {item === "Log out" ? (
                <a onClick={logout}>
                  {GetUserMenuIcon(item)}
                  <p>{item}</p>
                </a>
              ) : (
                <Link to={GenerateSlug(item)}>
                  {GetUserMenuIcon(item)}
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
