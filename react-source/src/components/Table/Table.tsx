import { useMemo, useState } from "react";
import { HeaderMapping, RowData, RowObject } from "../StatsTable/StatsTable";
import { getImageUrl, getImageUrlType } from "../../utils";
import { FaArrowUpLong } from "react-icons/fa6";
import { FaArrowDownLong } from "react-icons/fa6";
import "./table.css";
import { Unit } from "../../data";

interface TableProps {
  headers: HeaderMapping[];
  rows: RowData[];
  searchQuery?: string;
}

const Table = ({ headers, rows, searchQuery }: TableProps) => {
  const [sortBy, setSortBy] = useState<string | null>(headers[0].dataKey);
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");

  const formatValue = (value: any, type?: string) => {
    if (typeof value === "number") {
      const formattedValue = Number.isInteger(value) ? value : value.toFixed(2);
      return type ? `${formattedValue}${type}` : formattedValue;
    }
    return value;
  };

  const handleHeaderClick = (header: string) => {
    if (header === "team") return;
    if (sortBy === header) {
      setSortOrder(sortOrder === "asc" ? "desc" : "asc");
    } else {
      setSortBy(header);
      setSortOrder("asc");
    }
  };

  const filteredRows = useMemo(() => {
    if (!searchQuery) return rows;
    return rows.filter((row) =>
      String((row.object as RowObject).name)
        .toLowerCase()
        .includes(searchQuery.toLowerCase())
    );
  }, [searchQuery, rows, headers]);

  const sortedRows = useMemo(() => {
    if (!sortBy) return filteredRows;
    const comparison = sortOrder === "asc" ? 1 : -1;
    return filteredRows.slice().sort((a, b) => {
      let aValue, bValue;
      if (sortBy === "object") {
        aValue = (a[sortBy] as RowObject).name;
        bValue = (b[sortBy] as RowObject).name;
      } else {
        aValue = a[sortBy];
        bValue = b[sortBy];
      }
      if (typeof aValue === "string" && typeof bValue === "string") {
        return aValue.localeCompare(bValue) * comparison;
      } else if (typeof aValue === "number" && typeof bValue === "number") {
        return (aValue - bValue) * comparison;
      }
      return 0;
    });
  }, [sortBy, sortOrder, filteredRows]);

  return (
    <div className="app__table-table-wrapper">
      <table className="app__table-table">
        <thead>
          <tr>
            {headers.map((header, index) => (
              <th key={index} className="p__opensans" onClick={() => handleHeaderClick(header.dataKey)}>
                <div className="app__table-table-header">
                  <span>{header.displayName}</span>
                  {header.type != "team" && (
                    <span className="app__table-arrow">
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
            const imageUrl = getImageUrl(
              (row.object as RowObject).inGameKey as string,
              getImageUrlType(headers[0].displayName as "Unit" | "Trait" | "Item" | "Comp" | "Augment")
            );
            return (
              <tr key={rowIndex}>
                <td className="app__table-object p__opensans">
                  <img src={imageUrl} alt={row.name as string} className="app__table-object-img" />
                  {(row.object as RowObject).name as string}
                </td>
                {headers.slice(1).map((header, colIndex) => (
                  <td key={colIndex} className="p__opensans">
                    {header.type === "team" ? (
                      <div key={colIndex} className="app__table-team">
                        {(row[header.dataKey] as Unit[]).map((unit, unitIndex) => (
                          <img key={unitIndex} src={getImageUrl(unit.inGameKey, "champions")} alt={unit.name} />
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
  );
};

export default Table;
