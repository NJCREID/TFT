import { useEffect, useState } from "react";
import StatsTable, { HeaderMapping, RowData } from "../../components/StatsTable/StatsTable";
import "./compsstats.css";
import { ENDPOINT } from "../../common/endpoints";
import fetchRequest, { BaseCompStat } from "../../common/api";
export const headers: HeaderMapping[] = [
  { displayName: "Comp", dataKey: "name" },
  { displayName: "Games", dataKey: "games" },
  { displayName: "Play Rate", dataKey: "playRate", type: "/8" },
  { displayName: "Place", dataKey: "place" },
  { displayName: "Avg Delta", dataKey: "delta" },
  { displayName: "Top 4", dataKey: "top4", type: "%" },
  { displayName: "Win", dataKey: "win", type: "%" },
  { displayName: "Team", dataKey: "units", type: "team" },
];

const CompsStats = () => {
  const [compStats, setCompStats] = useState<RowData[] | null>(null);
  useEffect(() => {
    const fetchItemStats = async () => {
      const comps: BaseCompStat = await fetchRequest(ENDPOINT.STATS_FETCH_COMPS, "GET");
      const compStats = comps.compStats.map((comp) => {
        const stat = comp.stat;
        const playRate = ((stat.games / comps.games) * 100) / 8;
        const place = stat.place / stat.games;
        const top4 = (stat.top4 / stat.games) * 100;
        const win = (stat.win / stat.games) * 100;
        return {
          name: comp.name,
          games: stat.games,
          playRate,
          place,
          delta: stat.delta,
          top4,
          win,
          units: comp.units,
        };
      });
      setCompStats(compStats);
    };
    fetchItemStats();
  }, []);
  return (
    <div className="app__compsstats page_padding">{compStats && <StatsTable headers={headers} rows={compStats} />}</div>
  );
};

export default CompsStats;
