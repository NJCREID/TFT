import { useState } from "react";
import SearchBar from "../SearchBar/SearchBar";
import FilterSelector from "../FilterSelector/FilterSelector";
import Table from "../Table/Table";
import LoadingSpinner from "../LoadingSpinner/LoadingSpinner";
import { Unit } from "../../data/UnitData";
import "./statstable.css";

const leagueTiers = [
  { key: "", name: "All", imageUrl: "/images/general/all-option.svg" },
  { key: "Challenger", name: "Challenger", imageUrl: "/images/regalia/TFT_Regalia_Challenger.png" },
  { key: "GrandMaster", name: "Grand Master", imageUrl: "/images/regalia/TFT_Regalia_GrandMaster.png" },
  { key: "Master", name: "Master", imageUrl: "/images/regalia/TFT_Regalia_Master.png" },
];

export type RowObject = { name: string; inGameKey: string };

export type RowData = {
  [key: string]: string | number | Unit[] | RowObject;
};

export type HeaderMapping = {
  displayName: string;
  dataKey: string;
  type?: "%" | "/8" | "team";
};

type DataTableProps = {
  headers: HeaderMapping[];
  rows: RowData[] | null;
  onLeagueChange?: (league: string | null) => void;
};

const StatsTable = ({ headers, rows, onLeagueChange }: DataTableProps) => {
  const [searchQuery, setSearchQuery] = useState<string>("");

  const handleSearchChange = (query: string) => {
    setSearchQuery(query);
  };

  const handleLeagueChange = (league: string | null) => {
    if (!onLeagueChange) return;
    onLeagueChange(league == "All" ? "" : league);
  };

  return (
    <div className="app__statstable">
      <div className="app__statstable-header">
        <SearchBar placeHolder="Search..." value={searchQuery} onChange={handleSearchChange} />
        {!!onLeagueChange && <FilterSelector items={leagueTiers} onSelect={handleLeagueChange} />}
      </div>
      {!rows ? <LoadingSpinner /> : !!rows.length && <Table headers={headers} rows={rows} searchQuery={searchQuery} />}
    </div>
  );
};

export default StatsTable;
