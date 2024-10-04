export const getImageUrlType = (dataKey: "Unit" | "Item" | "Trait" | "Augment" | "Comp") => {
  const typeMap: { [key: string]: "champions" | "items" | "traits" | "augments" } = {
    Unit: "champions",
    Item: "items",
    Trait: "traits",
    Augment: "augments",
    Comp: "champions",
  };

  return typeMap[dataKey];
};
