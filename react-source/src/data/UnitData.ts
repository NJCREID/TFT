import { Trait } from "./TraitData";

export interface Unit {
  inGameKey: string;
  name: string;
  tier: number;
  cost: number[];
  health?: number[];
  attackDamage?: number[];
  damagePerSecond?: number[];
  attackRange?: number;
  attackSpeed?: number;
  armor?: number;
  magicalResistance?: number;
  recommendedItems?: string[];
  skill?: Skill;
  traits: Trait[];
  uniqueItemKey?: string;
  IsItemIncompatible?: boolean;
  compatabilityType?: string;
  isTriggerUnit?: boolean;
}

export interface Skill {
  id: number;
  key: string;
  name: string;
  imageUrl: string;
  desc: string;
  startingMana: number;
  skillMana: number;
  stats: string[];
}

interface BorderColors {
  [key: number]: string;
}

export const UNIT_COLORS: BorderColors = {
  1: "var(--color-tier1)",
  2: "var(--color-tier2)",
  3: "var(--color-tier3)",
  4: "var(--color-tier4)",
  5: "var(--color-tier5)",
  9: "var(--color-kayle)",
};

export const unitTiers = [
  { key: "all", name: "all", imageUrl: "/images/general/all-option.svg" },
  { key: "1", name: "1", type: "text" },
  { key: "2", name: "2", type: "text" },
  { key: "3", name: "3", type: "text" },
  { key: "4", name: "4", type: "text" },
  { key: "5", name: "5", type: "text" },
];
export const AZGold = [
  { key: "cost", name: "cost", imageUrl: "/images/general/coin.svg" },
  { key: "A-Z", name: "A-Z", type: "text" },
];
