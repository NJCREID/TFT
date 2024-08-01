import { useCallback, useEffect, useRef, useState } from "react";
import { useWindowDimensions } from "../context/windowDimensionContext";

export const useModalPosition = (modalPosition: { x: number; y: number; width: number; height: number } | null) => {
  const { width } = useWindowDimensions();
  const [position, setPosition] = useState({ x: 0, y: 0 });
  const modalRef = useRef<HTMLDivElement>(null);

  const getAppMainBounds = useCallback(() => {
    const element = document.querySelector(".app__main");
    const { left = 0, right = 0 } = element?.getBoundingClientRect() || {};
    return { left, right };
  }, [width]);

  useEffect(() => {
    if (!modalPosition || !modalRef.current) return;
    const headerHeight = 55;
    const mainContainerBuffer = 20;
    const itemBuffer = 10;
    const { left: containerLeft, right: containerRight } = getAppMainBounds();
    const { x: positionX, y: positionY, width: parentWidth, height: parentHeight } = modalPosition;
    const { width: modalWidth, height: modalHeight } = modalRef.current.getBoundingClientRect();

    const leftOffset = modalWidth / 2 - parentWidth / 2;
    let left = positionX - leftOffset;
    let top = positionY;

    top =
      top - modalHeight - mainContainerBuffer < headerHeight
        ? top + parentHeight + itemBuffer
        : top - modalHeight - itemBuffer;
    left =
      left < containerLeft
        ? containerLeft + mainContainerBuffer
        : left > containerRight
        ? containerRight - mainContainerBuffer
        : left;
    setPosition({ x: left, y: top });
  }, [width, getAppMainBounds, modalPosition]);

  return { position, modalRef };
};
