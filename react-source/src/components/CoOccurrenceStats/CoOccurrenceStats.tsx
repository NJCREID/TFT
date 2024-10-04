import { useEffect, useState } from "react";
import StatsTable, { HeaderMapping, RowData } from "../StatsTable/StatsTable";
import fetchRequest from "../../common/api";
import { ENDPOINT } from "../../common/endpoints";
import { useDispatch } from "react-redux";
import { showError } from "../../store/errorModalSlice";
import Selector from "../Selector/Selector";
import SelectedView from "../SelectedView/SelectedView";
import AugmentFilters from "../AugmentFIlters/AugmentFilters";
import UnitFilters from "../UnitFilters/UnitFilters";
import ItemFilters from "../ItemFilters/ItemFilters";
import TraitFilters from "../TraitFilters/TraitFilters";
import { Augment, BaseCoOcurrence, CoOccurrence, Item, Trait, Unit } from "../../data";
import "./cooccurrencestats.css";

const getDisplayName = (type: string): string => {
  if (type.includes(":trait")) return "Trait";
  if (type.includes(":augment")) return "Augment";
  if (type.includes(":item")) return "Item";
  return "Unit";
};

interface CoOccurrenceStatsProps {
  type: "unit" | "augment" | "item" | "trait";
}

export default function CoOccurrenceStats({ type }: CoOccurrenceStatsProps) {
  const [coOccurrence, setCoOccurrence] = useState<RowData[] | null>([]);
  const [headers, setHeaders] = useState<HeaderMapping[]>([]);
  const [objects, setObjects] = useState<(Unit | Item | Trait | Augment)[] | null>(null);
  const [coOccurrenceType, setCoOccurrenceType] = useState(`${type}:unit`);
  const [object, setObject] = useState<string | null>(null);
  const dispatch = useDispatch();

  useEffect(() => {
    if (!object) {
      setCoOccurrence([]);
      return;
    }
    const fetchChampionStats = async () => {
      try {
        const championDetails = await fetchRequest<BaseCoOcurrence>({
          endpoint: ENDPOINT.COOCCURRENCE_FETCH,
          identifier: `?type=${coOccurrenceType}&key=${object}`,
        });
        const coOcurrence = championDetails.coOccurrences.map((coOccurrence: CoOccurrence) => {
          const stat = coOccurrence.stat;

          const playRate = ((stat.games / championDetails.games) * 100) / 8;
          const place = stat.place / stat.games;
          const top4 = (stat.top4 / stat.games) * 100;
          const win = (stat.win / stat.games) * 100;

          const [inGameKey, tier] =
            coOccurrenceType === `${type}:trait` ? coOccurrence.inGameKey.split("-") : [coOccurrence.inGameKey, ""];
          return {
            object: { name: coOccurrence.name, inGameKey },
            tier: parseInt(tier),
            games: stat.games,
            playRate,
            place,
            delta: stat.delta,
            top4,
            win,
          };
        });
        setCoOccurrence(coOcurrence);

        const newHeaders: HeaderMapping[] = [
          { displayName: getDisplayName(coOccurrenceType), dataKey: "object" },
          ...(coOccurrenceType.includes(":trait") ? [{ displayName: "Tier", dataKey: "tier" }] : []),
          { displayName: "Games", dataKey: "games" },
          { displayName: "Play Rate", dataKey: "playRate", type: "/8" },
          { displayName: "Place", dataKey: "place" },
          { displayName: "Delta", dataKey: "delta" },
          { displayName: "Top 4", dataKey: "top4", type: "%" },
          { displayName: "Win", dataKey: "win", type: "%" },
        ];

        setHeaders(newHeaders);
      } catch (error) {
        setCoOccurrence([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching CoOccurrence: ${error.message}`]));
        } else {
          dispatch(showError(["An Unkown Error occured while fetching CoOccurrence"]));
        }
      }
    };
    fetchChampionStats();
  }, [coOccurrenceType, object]);

  const handleObjectsChange = (objects: (Unit | Item | Augment | Trait)[] | null) => {
    setObjects(objects);
  };

  const handleTypeChange = (key: string) => {
    setCoOccurrenceType(`${type}:` + key);
  };

  const handleObjectChange = (key: string | null) => {
    setObject(key);
  };

  const getFilters = () => {
    switch (type) {
      case "unit":
        return <UnitFilters onUnitsChange={handleObjectsChange} endpoint={ENDPOINT.UNIT_FULL_FETCH} />;
      case "item":
        return <ItemFilters onItemsChange={handleObjectsChange} endpoint={ENDPOINT.ITEM_PARTIAL_FETCH} />;
      case "trait":
        return <TraitFilters onTraitsChange={handleObjectsChange} />;
      case "augment":
        return <AugmentFilters onAugmentsChange={handleObjectsChange} />;
    }
  };

  return (
    <div className="app__cooccurrencestats">
      <div className="app__cooccurrencestats-filters">{getFilters()}</div>
      <div className="app__cooccurrencestats-container">
        <Selector data={objects} height="350px" />
        <SelectedView onTypeChange={handleTypeChange} onObjectChange={handleObjectChange} type={type} />
      </div>
      <StatsTable headers={headers} rows={coOccurrence} />
    </div>
  );
}
