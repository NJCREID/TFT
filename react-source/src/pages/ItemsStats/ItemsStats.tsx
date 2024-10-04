import { useEffect, useState } from "react";
import StatsTable, { HeaderMapping, RowData } from "../../components/StatsTable/StatsTable";
import fetchRequest from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import "./itemsstats.css";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import { BaseItemStat } from "../../data";

const headers: HeaderMapping[] = [
  { displayName: "Item", dataKey: "object" },
  { displayName: "Games", dataKey: "games" },
  { displayName: "Play Rate", dataKey: "playRate", type: "/8" },
  { displayName: "Place", dataKey: "place" },
  { displayName: "Delta", dataKey: "delta" },
  { displayName: "Top 4", dataKey: "top4", type: "%" },
  { displayName: "Win", dataKey: "win", type: "%" },
];

export default function ItemsStats() {
  const [itemStats, setItemStats] = useState<RowData[] | null>(null);
  const [league, setLeague] = useState("");
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchItemStats = async () => {
      try {
        const items = await fetchRequest<BaseItemStat>({
          endpoint: ENDPOINT.STATS_FETCH_ITEM,
          identifier: league,
        });
        const itemStats = items.itemStats.map((item) => {
          const stat = item.stat;
          const playRate = ((stat.games / items.games) * 100) / 8;
          const place = stat.place / stat.games;
          const top4 = (stat.top4 / stat.games) * 100;
          const win = (stat.win / stat.games) * 100;
          return {
            object: { name: item.name, inGameKey: item.inGameKey },
            games: stat.games,
            playRate,
            place,
            delta: stat.delta,
            top4,
            win,
          };
        });
        setItemStats(itemStats);
      } catch (error) {
        setItemStats([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching item stats: ${error.message}`]));
        } else {
          dispatch(showError(["An Unkown Error occured while fetching item stats."]));
        }
      }
    };
    fetchItemStats();
  }, [league]);

  const handleLeagueChange = (league: string | null) => {
    setLeague(league == null ? "" : league);
  };

  return (
    <div className="app__itemsstats page_padding">
      <StatsTable headers={headers} rows={itemStats} onLeagueChange={handleLeagueChange} />
    </div>
  );
}
