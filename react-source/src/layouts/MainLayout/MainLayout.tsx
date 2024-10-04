import { Outlet } from "react-router-dom";
import "./mainlayout.css";
import { ErrorModal, Modal, SideBar, Header } from "../../components";
import { useState } from "react";

export default function MainLayout() {
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);

  const handleCloseMenu = () => {
    setIsSidebarOpen(false);
  };

  return (
    <div className="app__mainlayout">
      <Header onMenuClick={setIsSidebarOpen} />
      <SideBar isMenuOpen={isSidebarOpen} onMenuClose={handleCloseMenu} />
      <main className="app__main" style={{ backgroundImage: `url(/images/general/appbackground.avif)` }}>
        <ErrorModal />
        <Modal />
        <Outlet />
      </main>
    </div>
  );
}
