import { NavLink } from "react-router-dom";
import "./underlinelink.css";
import { generateSlug } from "../../utils";
export default function UnderlineLink({ title }: { title: string }) {
  return (
    <NavLink to={`/${generateSlug(title)}`} className="app__underlinelink-container underline_animation-hover">
      <p className="p__opensans-small p__bold underline_animation">{title}</p>
    </NavLink>
  );
}
