import { useEffect, useState } from "react";
import StatsTable, { HeaderMapping, RowData } from "../../components/StatsTable/StatsTable";
import fetchRequest, { BaseItemStat } from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import "./itemsstats.css";

const headers: HeaderMapping[] = [
  { displayName: "Item", dataKey: "name" },
  { displayName: "Games", dataKey: "games" },
  { displayName: "Play Rate", dataKey: "playRate", type: "/8" },
  { displayName: "Place", dataKey: "place" },
  { displayName: "Delta", dataKey: "delta" },
  { displayName: "Top 4", dataKey: "top4", type: "%" },
  { displayName: "Win", dataKey: "win", type: "%" },
];

export default function ItemsStats() {
  const [itemStats, setItemStats] = useState<RowData[] | null>(null);
  useEffect(() => {
    const fetchItemStats = async () => {
      const items: BaseItemStat = await fetchRequest(ENDPOINT.STATS_FETCH_ITEM, "GET");
      const itemStats = items.itemStats.map((item) => {
        const stat = item.stat;
        const playRate = ((stat.games / items.games) * 100) / 8;
        const place = stat.place / stat.games;
        const top4 = (stat.top4 / stat.games) * 100;
        const win = (stat.win / stat.games) * 100;
        return {
          name: item.name,
          games: stat.games,
          playRate,
          place,
          delta: stat.delta,
          top4,
          win,
        };
      });
      setItemStats(itemStats);
    };
    fetchItemStats();
  }, []);

  return (
    <div className="app__itemsstats page_padding">{itemStats && <StatsTable headers={headers} rows={itemStats} />}</div>
  );
}
