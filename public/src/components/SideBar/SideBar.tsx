import UnderlineLink from "../UnderlineLink/UnderlineLink";
import { LuPanelLeftClose } from "react-icons/lu";
import GetNavIcon from "../../utils/GetIcons";
import { useEffect, useState } from "react";
import { useWindowDimensions } from "../../context/windowDimensionContext";
import "./sidebar.css";

const listItems = [
  {
    name: "Home",
    items: ["Home"],
  },
  {
    name: "Comps",
    items: ["Auto Generated", "Comps Stats", "Community Comps", "My Team Comps", "Team Builder"],
  },
  {
    name: "Stats",
    items: ["Comps", "Champions", "Augments", "Items", "Traits"],
  },
];

export default function SideBar() {
  const [smallScreen, setSmallScreen] = useState(true);
  const [isOpen, setIsOpen] = useState<number | null>(0);
  const { width } = useWindowDimensions();

  useEffect(() => {
    setSmallScreen(window.innerWidth < 1100);
  }, [width]);

  useEffect(() => {
    setIsOpen((prev) => (smallScreen ? null : prev ? prev : 0));
  }, [smallScreen]);

  const toggleOpen = (index: number) => {
    setIsOpen((prev) => (prev !== index ? index : smallScreen ? null : prev));
  };

  return (
    <div className="app__sidebar">
      <ul className="app__sidebar-list">
        {listItems.map(({ name, items }, index) => (
          <li key={name} className="app__sidebar-section">
            <button
              onClick={() => toggleOpen(index)}
              className={`app__sidebar-section-button`}
              style={{
                backgroundColor: isOpen === index ? "var(--color-secondary)" : undefined,
              }}
            >
              {GetNavIcon(name, "white", 24)}
            </button>
            {isOpen === index && (
              <div className="app__sidebar-section-links">
                {smallScreen && (
                  <div onClick={() => toggleOpen(index)} className="app__sidebar-section-links-close">
                    <LuPanelLeftClose color="white" size={20} />
                  </div>
                )}

                <ul>
                  {items.map((item) => (
                    <UnderlineLink key={item} title={item} name={name} />
                  ))}
                </ul>
              </div>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
}
