import { useEffect, useState } from "react";
import StatsTable, { HeaderMapping, RowData } from "../../components/StatsTable/StatsTable";
import "./compsstats.css";
import { ENDPOINT } from "../../common/endpoints";
import fetchRequest from "../../common/api";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import { BaseCompStat } from "../../data";

export const headers: HeaderMapping[] = [
  { displayName: "Comp", dataKey: "object" },
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
  const [league, setLeague] = useState("");
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchItemStats = async () => {
      try {
        const comps = await fetchRequest<BaseCompStat>({
          endpoint: ENDPOINT.STATS_FETCH_COMPS,
          identifier: league,
        });
        const compStats = comps.compStats.map((comp) => {
          const stat = comp.stat;
          const playRate = ((stat.games / comps.games) * 100) / 8;
          const place = stat.place / stat.games;
          const top4 = (stat.top4 / stat.games) * 100;
          const win = (stat.win / stat.games) * 100;
          return {
            object: { name: comp.name, inGameKey: comp.inGameKey },
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
      } catch (error) {
        setCompStats([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching comp stats: ${error.message}`]));
        } else {
          dispatch(showError(["An Unkown Error occured while fetching comp stats."]));
        }
      }
    };
    fetchItemStats();
  }, [league]);

  const handleLeagueChange = (league: string | null) => {
    setLeague(league == null ? "" : league);
  };

  return (
    <div className="app__compsstats page_padding">
      <StatsTable headers={headers} rows={compStats} onLeagueChange={handleLeagueChange} />
    </div>
  );
};

export default CompsStats;
