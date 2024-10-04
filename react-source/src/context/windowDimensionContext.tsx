import {
  ReactNode,
  createContext,
  useContext,
  useEffect,
  useState,
} from "react";

interface Dimensions {
  width: number;
  height: number;
}

const WindowDimensionsContext = createContext<Dimensions | null>(null);

export const useWindowDimensions = () => {
  const context = useContext(WindowDimensionsContext);
  if (!context) return { width: 0, height: 0 };
  return context;
};

export const WindowDimensionsProvider = ({
  children,
}: {
  children: ReactNode;
}) => {
  const [dimensions, setDimensions] = useState<Dimensions>({
    width: window.innerWidth,
    height: window.innerHeight,
  });

  useEffect(() => {
    const handleResize = () => {
      setDimensions({
        width: window.innerWidth,
        height: window.innerHeight,
      });
    };
    window.addEventListener("resize", handleResize);

    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, []);

  return (
    <WindowDimensionsContext.Provider value={dimensions!}>
      {children}
    </WindowDimensionsContext.Provider>
  );
};
