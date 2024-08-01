export const getElementPosition = (
  element: HTMLElement | null
): { x: number; y: number; width: number; height: number } => {
  if (!element) return { x: 0, y: 0, width: 0, height: 0 };

  const rect = element.getBoundingClientRect();
  return {
    x: rect.left,
    y: rect.top,
    width: rect.width,
    height: rect.height,
  };
};
