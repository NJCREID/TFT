import { UserGuideTrait } from "../data";

export function sortTraits(traits: UserGuideTrait[]) {
  const filteredTraits = traits.filter((trait) => trait.tier > 0);
  const sortedTraits = filteredTraits.sort((a, b) => {
    return b.tier - a.tier;
  });
  return sortedTraits;
}
