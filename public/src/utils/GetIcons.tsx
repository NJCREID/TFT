import { BiArchive } from "react-icons/bi";
import { GoHome } from "react-icons/go";
import { GiElfHelmet } from "react-icons/gi";
import { FaRegUserCircle } from "react-icons/fa";
import { IoSettingsOutline } from "react-icons/io5";
import { IoIosLogOut } from "react-icons/io";
import { IoLogInOutline } from "react-icons/io5";
import { CiEdit } from "react-icons/ci";

export default function GetNavIcon(type: string, color: string, size: number) {
  switch (type) {
    case "Home":
      return <GoHome color={color} size={size} />;
    case "Stats":
      return <BiArchive color={color} size={size} />;
    case "Comps":
      return <GiElfHelmet color={color} size={size} />;
  }
}

export function GetUserMenuIcon(type: string) {
  switch (type) {
    case "My Page":
      return <FaRegUserCircle />;
    case "Settings":
      return <IoSettingsOutline />;
    case "Log out":
      return <IoIosLogOut />;
    case "Register":
      return <CiEdit />;
    case "Sign In":
      return <IoLogInOutline />;
  }
}
