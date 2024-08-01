import Header from "../../components/Header/Header";
import { Outlet } from "react-router-dom";
import SideBar from "../../components/SideBar/SideBar";
import "./mainlayout.css";
import Modal from "../../components/Modal/Modal";
import appBackground from "../../assets/download.png";
import { ErrorModal } from "../../components/ErrorModal/ErrorModal";
export default function MainLayout() {
  return (
    <div className="app__mainlayout">
      <Header />
      <SideBar />
      <main className="app__main" style={{ backgroundImage: `url(${appBackground})` }}>
        <ErrorModal />
        <Modal />
        <Outlet />
      </main>
    </div>
  );
}
