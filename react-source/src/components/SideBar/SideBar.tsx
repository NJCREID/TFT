import UnderlineLink from "../UnderlineLink/UnderlineLink";
import { LuPanelLeftClose } from "react-icons/lu";
import { getNavIcon } from "../../utils";
import { useEffect, useState } from "react";
import { useWindowDimensions } from "../../context";
import { AiOutlineClose } from "react-icons/ai";
import "./sidebar.css";

const listItems = [
  {
    name: "Home",
    items: ["Home", "Odds Of Hitting"],
  },
  {
    name: "Comps",
    items: ["Auto Generated", "Comp Stats", "My Team Comps", "Team Builder"],
  },
  {
    name: "Stats",
    items: ["Comp Stats", "Unit Stats", "Augment Stats", "Item Stats", "Trait Stats"],
  },
  {
    name: "Details",
    items: ["Unit Details", "Augment Details", "Item Details", "Trait Details"],
  },
];

interface SideBarProps {
  isMenuOpen: boolean;
  onMenuClose: () => void;
}

export default function SideBar({ isMenuOpen, onMenuClose }: SideBarProps) {
  const [subsection, setSubsection] = useState<string | null>("Home");
  const { width } = useWindowDimensions();
  const isMediumScreen = width < 1100 && width > 750;
  const isSmallScreen = width < 750;

  useEffect(() => {
    setSubsection((prev) => (isMediumScreen ? null : prev ? prev : "Home"));
  }, [isMediumScreen]);

  const toggleOpen = (section: string | null) => {
    setSubsection((prev) => (prev !== section ? section : isMediumScreen ? null : prev));
  };

  const handleClose = () => {
    onMenuClose();
  };

  return (
    <div className={`app__sidebar ${isMenuOpen && "active"}`}>
      <ul className="app__sidebar-list">
        {listItems.map(({ name }) => (
          <li key={name} className="app__sidebar-section">
            <button
              onClick={() => toggleOpen(name)}
              className={`app__sidebar-section-button`}
              style={{
                backgroundColor: subsection === name ? "var(--color-secondary)" : undefined,
              }}
            >
              {getNavIcon(name, "white", 24)}
            </button>
          </li>
        ))}
      </ul>
      {subsection && (
        <div className="app__sidebar-section-links">
          {isMediumScreen && (
            <div onClick={() => toggleOpen(null)} className="app__sidebar-section-links-close">
              <LuPanelLeftClose color="white" size={20} />
            </div>
          )}
          {isSmallScreen && (
            <div className="app__sidebar-section-close" onClick={handleClose}>
              <AiOutlineClose color="white" size={20} />
            </div>
          )}
          <ul>
            {listItems
              .find((item) => item.name === subsection)
              ?.items.map((item) => (
                <li key={item} className="app__sidebarlink" onClick={handleClose}>
                  <UnderlineLink title={item} />
                </li>
              ))}
          </ul>
        </div>
      )}
    </div>
  );
}
