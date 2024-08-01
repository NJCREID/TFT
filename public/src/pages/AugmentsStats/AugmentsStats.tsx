import { useEffect, useState } from "react";
import StatsTable, { HeaderMapping, RowData } from "../../components/StatsTable/StatsTable";
import "./augmentsstats.css";
import fetchRequest, { BaseAugmentStat } from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";

export const headers: HeaderMapping[] = [
  { displayName: "Augment", dataKey: "name" },
  { displayName: "Games", dataKey: "games" },
  { displayName: "Place", dataKey: "place" },
  { displayName: "Avg Delta", dataKey: "delta" },
  { displayName: "Top 4", dataKey: "top4", type: "%" },
  { displayName: "Win", dataKey: "win", type: "%" },
  { displayName: "At 2-1", dataKey: "placeStage2" },
  { displayName: "At 3-2", dataKey: "placeStage3" },
  { displayName: "At 4-2", dataKey: "placeStage4" },
];
export default function AugmentsStats() {
  const [augmentStats, setAugmentStats] = useState<RowData[] | null>(null);
  useEffect(() => {
    const fetchAugmentStats = async () => {
      const augmentStats: BaseAugmentStat = await fetchRequest(ENDPOINT.STATS_FETCH_AUGMENT, "GET");
      const augmentRows: RowData[] = augmentStats.augmentStats.map((augment) => {
        const stage2 = augment.stats[0] || { games: 0, place: 0, top4: 0, win: 0, delta: 0 };
        const stage3 = augment.stats[1] || { games: 0, place: 0, top4: 0, win: 0, delta: 0 };
        const stage4 = augment.stats[2] || { games: 0, place: 0, top4: 0, win: 0, delta: 0 };

        const totalGames = stage2.games + stage3.games + stage4.games;
        const avgPlace = (stage2.place + stage3.place + stage4.place) / totalGames;
        const avgTop4 = ((stage2.top4 + stage3.top4 + stage4.top4) / totalGames) * 100;
        const avgWin = ((stage2.win + stage3.win + stage4.win) / totalGames) * 100;
        const avgDelta = (stage3.delta + stage3.delta + stage4.delta) / 3;
        return {
          name: augment.name,
          games: totalGames,
          place: avgPlace,
          delta: avgDelta,
          top4: avgTop4,
          win: avgWin,
          placeStage2: stage2.games ? (stage2.place / stage2.games).toFixed(2) : "--",
          placeStage3: stage3.games ? (stage3.place / stage3.games).toFixed(2) : "--",
          placeStage4: stage4.games ? (stage4.place / stage4.games).toFixed(2) : "--",
        };
      });
      setAugmentStats(augmentRows);
    };
    fetchAugmentStats();
  }, []);

  return (
    <div className="app__augmentsstats page_padding">
      {augmentStats && <StatsTable headers={headers} rows={augmentStats} />}
    </div>
  );
}
