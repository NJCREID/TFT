import { IoIosSearch } from "react-icons/io";
import { memo, useRef } from "react";
import "./searchbar.css";

interface SearchBarProps {
  placeHolder: string;
  onChange: (value: string) => void;
  value: string;
}

const SearchBar = ({ placeHolder, onChange, value }: SearchBarProps) => {
  const inputRef = useRef<HTMLInputElement>(null);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    onChange(event.target.value);
  };

  const handleSearchIconClick = () => {
    if (inputRef.current) {
      inputRef.current.focus();
    }
  };

  return (
    <div className="app__searchbar">
      <div className="app__searchbar-input">
        <input value={value} ref={inputRef} type="text" placeholder={placeHolder} onChange={handleChange} />
      </div>
      <div className="app__searchbar-icon" onClick={handleSearchIconClick}>
        <IoIosSearch size={24} />
      </div>
    </div>
  );
};
export default memo(SearchBar);
