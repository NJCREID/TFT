import "./statssvg.css";
interface StatsSVGProps {
  type: string;
  value?: number;
}

interface StatSVG {
  [key: string]: string;
}

const StatSVGs: StatSVG = {
  Health: "/images/general/Health.avif",
  Damage: "/images/general/Damage.avif",
  DPS: "/images/general/DPS.avif",
  Crit: "/images/general/DPS.avif",
  MR: "/images/general/MR.avif",
  AP: "/images/general/AP.avif",
  Armor: "/images/general/Armor.avif",
  Speed: "/images/general/Speed.avif",
  Mana: "/images/general/Mana.avif",
};

const StatsSVG = ({ type, value }: StatsSVGProps) => {
  if (type == "Range" && value) return <RangeSVG value={value} />;
  const attribute = StatSVGs[type];
  if (!attribute) return null;
  return <img className="app__statssvg-img" src={StatSVGs[type]} alt={type} />;
};

export default StatsSVG;

const RangeSVG = ({ value }: { value: number }) => {
  const rects = [0, 5, 10, 15, 20];

  return (
    <svg className="app__statssvg-svg" width="25" height="14" viewBox="0 0 25 14">
      {rects.map((x, index) => (
        <rect key={index} fill={index < value ? "#fff" : "#6c6c94"} x={x} width="3px" height="14px" />
      ))}
    </svg>
  );
};
