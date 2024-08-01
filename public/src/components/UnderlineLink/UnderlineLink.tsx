import { NavLink } from "react-router-dom";
import "./underlinelink.css";
import { GenerateSlug } from "../../utils/GenerateSlug";
export default function UnderlineLink({ title, name }: { title: string; name: string }) {
  return (
    <li className="app__sidebarlink">
      <NavLink to={`/${GenerateSlug(title, name)}`} className="app__sidebarlink-container">
        <p className="p__opensans-small p__bold">{title}</p>
      </NavLink>
    </li>
  );
}
