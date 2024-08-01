import { useMemo, useState } from "react";
import "./statstable.css";
import { FaArrowUpLong } from "react-icons/fa6";
import { FaArrowDownLong } from "react-icons/fa6";
import SearchBar from "../SearchBar/SearchBar";
import { lookupName } from "../../utils/lookupName";
import { Unit } from "../../common/api";

export type RowData = {
  [key: string]: string | number | Unit[];
};

export type HeaderMapping = {
  displayName: string;
  dataKey: string;
  type?: "%" | "/8" | "team";
};

type DataTableProps = {
  headers: HeaderMapping[];
  rows: RowData[];
};

const StatsTable = ({ headers, rows }: DataTableProps) => {
  const [sortBy, setSortBy] = useState<string | null>(null);
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
  const [searchQuery, setSearchQuery] = useState<string>("");

  const handleHeaderClick = (header: string) => {
    if (header === "team") return;
    if (sortBy === header) {
      setSortOrder(sortOrder === "asc" ? "desc" : "asc");
    } else {
      setSortBy(header);
      setSortOrder("asc");
    }
  };

  const handleSearchChange = (query: string) => {
    setSearchQuery(query);
  };

  const filteredRows = useMemo(() => {
    if (!searchQuery) return rows;
    return rows.filter((row) => String(row["name"]).toLowerCase().includes(searchQuery.toLowerCase()));
  }, [searchQuery, rows, headers]);

  const sortedRows = useMemo(() => {
    if (!sortBy) return filteredRows;
    const comparison = sortOrder === "asc" ? 1 : -1;
    return filteredRows.slice().sort((a, b) => {
      const aValue = a[sortBy];
      const bValue = b[sortBy];
      if (typeof aValue === "string" && typeof bValue === "string") {
        return aValue.localeCompare(bValue) * comparison;
      } else if (typeof aValue === "number" && typeof bValue === "number") {
        return (aValue - bValue) * comparison;
      }
      return 0;
    });
  }, [sortBy, sortOrder, filteredRows]);

  const formatValue = (value: any, type?: string) => {
    if (typeof value === "number") {
      const formattedValue = Number.isInteger(value) ? value : value.toFixed(2);
      return type ? `${formattedValue}${type}` : formattedValue;
    }
    return value;
  };

  return (
    <div className="app__statstable">
      <div className="app__statstable-search">
        <SearchBar placeHolder="Search..." value={searchQuery} onChange={handleSearchChange} />
      </div>
      <div className="app__statstable-table-wrapper">
        <table className="app__statstable-table">
          <thead>
            <tr>
              {headers.map((header, index) => (
                <th key={index} className="p__opensans" onClick={() => handleHeaderClick(header.dataKey)}>
                  <div className="app__statstable-table-header">
                    <span>{header.displayName}</span>
                    {header.type != "team" && (
                      <span className="app__statstable-arrow">
                        {sortBy === header.dataKey && (sortOrder === "asc" ? <FaArrowDownLong /> : <FaArrowUpLong />)}
                      </span>
                    )}
                  </div>
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {sortedRows.map((row, rowIndex) => {
              const { name, imageUrl } = lookupName(row.name as string);
              return (
                <tr key={rowIndex}>
                  <td className="app__statstable-object p__opensans">
                    <img src={imageUrl} alt={row.name as string} className="app__statstable-object-img" />
                    {name}
                  </td>
                  {headers.slice(1).map((header, colIndex) => (
                    <td key={colIndex} className="p__opensans">
                      {header.type === "team" ? (
                        <div key={colIndex} className="app__statstable-team">
                          {(row[header.dataKey] as Unit[]).map((unit, unitIndex) => (
                            <img key={unitIndex} src={unit.imageUrl} alt={unit.name} />
                          ))}
                        </div>
                      ) : (
                        formatValue(row[header.dataKey], header.type)
                      )}
                    </td>
                  ))}
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default StatsTable;
