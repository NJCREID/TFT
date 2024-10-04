import { useEffect, useState } from "react";
import StatsTable, { HeaderMapping, RowData } from "../../components/StatsTable/StatsTable";
import fetchRequest from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import "./unitsstats.css";
import { BaseUnitStat, UnitStat } from "../../data";

const headers: HeaderMapping[] = [
  { displayName: "Unit", dataKey: "object" },
  { displayName: "Games", dataKey: "games" },
  { displayName: "Play Rate", dataKey: "playRate", type: "/8" },
  { displayName: "Place", dataKey: "place" },
  { displayName: "Delta", dataKey: "delta" },
  { displayName: "Top 4", dataKey: "top4", type: "%" },
  { displayName: "Win", dataKey: "win", type: "%" },
  { displayName: "3 Star Rate", dataKey: "star3Rate", type: "%" },
  { displayName: "3 Star Place", dataKey: "star3Place" },
];

export default function UnitStats() {
  const [unitStats, setUnitStats] = useState<RowData[] | null>(null);
  const [league, setLeague] = useState("");
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchunitStats = async () => {
      try {
        const units = await fetchRequest<BaseUnitStat>({
          endpoint: ENDPOINT.STATS_FETCH_UNIT,
          identifier: league,
        });
        const unitStats = units.unitStats.map((unitStat: UnitStat) => {
          const starredUnitStat = units.starredUnitStats.find((starredUnit) => starredUnit.name === unitStat.name);
          const stat = unitStat.stat;
          const starredStat = starredUnitStat?.stat;

          const playRate = ((stat.games / units.games) * 100) / 8;
          const place = stat.place / stat.games;
          const top4 = (stat.top4 / stat.games) * 100;
          const win = (stat.win / stat.games) * 100;

          return {
            object: { name: unitStat.name, inGameKey: unitStat.inGameKey },
            games: stat.games,
            playRate,
            place,
            delta: stat.delta,
            top4,
            win,
            star3Rate: starredStat ? (starredStat.games / unitStat.stat.games) * 100 : 0,
            star3Place: starredStat ? starredStat.place / starredStat.games : 0,
          };
        });
        setUnitStats(unitStats);
      } catch (error) {
        setUnitStats([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching unit stats: ${error.message}`]));
        } else {
          dispatch(showError(["An Unkown Error occured while fetching unit stats"]));
        }
      }
    };
    fetchunitStats();
  }, [league]);

  const handleLeagueChange = (league: string | null) => {
    setLeague(league == null ? "" : league);
  };

  return (
    <div className="app__unitsstats page_padding">
      <StatsTable headers={headers} rows={unitStats} onLeagueChange={handleLeagueChange} />
    </div>
  );
}
