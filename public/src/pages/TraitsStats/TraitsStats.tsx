import { useEffect, useState } from "react";
import StatsTable, { HeaderMapping, RowData } from "../../components/StatsTable/StatsTable";
import fetchRequest, { BaseTraitStat } from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import "./traitsstats.css";

const headers: HeaderMapping[] = [
  { displayName: "Trait", dataKey: "name" },
  { displayName: "Tier", dataKey: "numUnits" },
  { displayName: "Games", dataKey: "games" },
  { displayName: "Play Rate", dataKey: "playRate", type: "/8" },
  { displayName: "Place", dataKey: "place" },
  { displayName: "Delta", dataKey: "delta" },
  { displayName: "Top 4", dataKey: "top4", type: "%" },
  { displayName: "Win", dataKey: "win", type: "%" },
];

export default function TraitsStats() {
  const [traitStats, setTraitStats] = useState<RowData[] | null>(null);
  useEffect(() => {
    const fetchTraitStats = async () => {
      const traits: BaseTraitStat = await fetchRequest(ENDPOINT.STATS_FETCH_TRAIT, "GET");
      const traitStats = traits.traitStats.map((trait) => {
        const stat = trait.stat;

        const playRate = ((stat.games / traits.games) * 100) / 8;
        const place = stat.place / stat.games;
        const top4 = (stat.top4 / stat.games) * 100;
        const win = (stat.win / stat.games) * 100;

        return {
          name: trait.name,
          numUnits: trait.numUnits,
          games: stat.games,
          playRate,
          place,
          delta: stat.delta,
          top4,
          win,
        };
      });
      setTraitStats(traitStats);
    };
    fetchTraitStats();
  }, []);

  return (
    <div className="app__traitsstats page_padding">
      {traitStats && <StatsTable headers={headers} rows={traitStats} />}
    </div>
  );
}
