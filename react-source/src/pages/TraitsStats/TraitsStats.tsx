import { useEffect, useState } from "react";
import StatsTable, { HeaderMapping, RowData } from "../../components/StatsTable/StatsTable";
import fetchRequest from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import "./traitsstats.css";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import { BaseTraitStat } from "../../data";

const headers: HeaderMapping[] = [
  { displayName: "Trait", dataKey: "object" },
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
  const [league, setLeague] = useState("");
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchTraitStats = async () => {
      try {
        const traits = await fetchRequest<BaseTraitStat>({
          endpoint: ENDPOINT.STATS_FETCH_TRAIT,
          identifier: league,
        });
        const traitStats = traits.traitStats.map((trait) => {
          const stat = trait.stat;

          const playRate = ((stat.games / traits.games) * 100) / 8;
          const place = stat.place / stat.games;
          const top4 = (stat.top4 / stat.games) * 100;
          const win = (stat.win / stat.games) * 100;

          return {
            object: { name: trait.name, inGameKey: trait.inGameKey },
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
      } catch (error) {
        setTraitStats([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching trait stats: ${error.message}`]));
        } else {
          dispatch(showError(["An Unkown Error occured while fetching trait stats."]));
        }
      }
    };
    fetchTraitStats();
  }, [league]);

  const handleLeagueChange = (league: string | null) => {
    setLeague(league == null ? "" : league);
  };

  return (
    <div className="app__traitsstats page_padding">
      <StatsTable headers={headers} rows={traitStats} onLeagueChange={handleLeagueChange} />
    </div>
  );
}
