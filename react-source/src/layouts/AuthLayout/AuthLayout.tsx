import { useEffect, useState } from "react";
import { useWindowDimensions } from "../../context";
import { NavLink, Outlet } from "react-router-dom";
import "./authlayout.css";

const AuthLayout = () => {
  const [smallScreen, setSmallScreen] = useState(false);
  const { width } = useWindowDimensions();
  useEffect(() => {
    setSmallScreen(window.innerWidth < 800);
  }, [width]);
  return (
    <div className="app__authlayout">
      <nav className={`app__authlayout-header ${smallScreen ? "gradient-purple-alt" : "gradient-purple"}`}>
        <div className="app__authlayout-header-homelink">
          <NavLink to="/">
            <img
              src={smallScreen ? "/images/general/logo_text.svg" : "/images/general/logo_lightning.svg"}
              alt="TeamTactixs Logo"
              className="app__authlayout-header-homelink-img"
            />
          </NavLink>
        </div>
      </nav>
      <main className="app__main">
        <Outlet />
      </main>
    </div>
  );
};
export default AuthLayout;
