import { useEffect, useState } from "react";
import StatsTable, { HeaderMapping, RowData } from "../../components/StatsTable/StatsTable";
import fetchRequest, { BaseUnitStat, UnitStat } from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import "./championsstats.css";

const headers: HeaderMapping[] = [
  { displayName: "Champion", dataKey: "name" },
  { displayName: "Games", dataKey: "games" },
  { displayName: "Play Rate", dataKey: "playRate", type: "/8" },
  { displayName: "Place", dataKey: "place" },
  { displayName: "Delta", dataKey: "delta" },
  { displayName: "Top 4", dataKey: "top4", type: "%" },
  { displayName: "Win", dataKey: "win", type: "%" },
  { displayName: "3 Star Rate", dataKey: "star3Rate", type: "%" },
  { displayName: "3 Star Place", dataKey: "star3Place" },
];
export default function ChampionsStats() {
  const [championStats, setChampionStats] = useState<RowData[] | null>(null);
  useEffect(() => {
    const fetchChampionStats = async () => {
      const units: BaseUnitStat = await fetchRequest(ENDPOINT.STATS_FETCH_UNIT, "GET");
      const championStats = units.unitStats.map((unitStat: UnitStat) => {
        const starredUnitStat = units.starredUnitStats.find((starredUnit) => starredUnit.name === unitStat.name);
        const stat = unitStat.stat;
        const starredStat = starredUnitStat?.stat;

        const playRate = ((stat.games / units.games) * 100) / 8;
        const place = stat.place / stat.games;
        const top4 = (stat.top4 / stat.games) * 100;
        const win = (stat.win / stat.games) * 100;

        return {
          name: unitStat.name,
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
      setChampionStats(championStats);
    };
    fetchChampionStats();
  }, []);
  return (
    <div className="app__championsstats page_padding">
      {championStats && <StatsTable headers={headers} rows={championStats} />}
    </div>
  );
}
