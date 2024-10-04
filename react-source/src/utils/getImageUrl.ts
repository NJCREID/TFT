export const getImageUrl = (
  key: string,
  category: "augments" | "champions" | "traits" | "items",
  type: "splash" | "tiles" = "tiles"
) => {
  if (category == "champions") {
    return `/images/champions/${type}/${key}.avif`;
  }
  return `/images/${category}/${key}${category === "traits" ? ".svg" : ".avif"}`;
};
