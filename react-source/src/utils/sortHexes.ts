import { Hex } from "../data";

export default function sortHexes(hexes: Hex[]) {
  return hexes.sort((a, b) => {
    if (a.unit?.tier === 0 && b.unit?.tier !== 0) return 1;
    if (b.unit?.tier === 0 && a.unit?.tier !== 0) return -1;
    if (a.currentItems.length === 0 && b.currentItems.length > 0) return 1;
    if (b.currentItems.length === 0 && a.currentItems.length > 0) return -1;
    if (a.currentItems.length !== b.currentItems.length) return b.currentItems.length - a.currentItems.length;
    if (a.unit?.tier !== b.unit?.tier) return b.unit.tier - a.unit.tier;
    if (a.isStarred && !b.isStarred) return -1;
    if (!a.isStarred && b.isStarred) return 1;
    return 0;
  });
}
