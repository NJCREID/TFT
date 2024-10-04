import { Tier } from "../data";

export default function GenerateTier(tiers: Tier[], amount: number) {
  const tier = tiers!.reduce((highest, current) => {
    if (current.level <= amount) {
      return highest && highest.level > current.level ? highest : current;
    }
    return highest;
  }, tiers![0]);
  return tier.rarity;
}
