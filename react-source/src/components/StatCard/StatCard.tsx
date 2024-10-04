import StatsSVG from "../StatsSVG/StatsSVG";
import "./statcard.css";

interface StatCardProps {
  title: string;
  stat: string | number | number[];
}

const StatCard = ({ stat, title }: StatCardProps) => {
  const isRange = title === "Range";
  return (
    <div className="app__statcard">
      <p className="p__desc-text">{title}</p>
      <p className="app__statcard-stat">
        {isRange ? (
          <StatsSVG type={title} value={stat as number} />
        ) : (
          <>
            <StatsSVG type={title} />
            <span className="p__opensans-small">{Array.isArray(stat) ? stat.join(" / ") : stat}</span>
          </>
        )}
      </p>
    </div>
  );
};

export default StatCard;
