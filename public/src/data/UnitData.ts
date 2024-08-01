interface BorderColors {
  [key: number]: string;
}

export interface Unit {
  key: string;
  ingameKey: string;
  name: string;
  image_url: string;
  originalImageUrl?: string;
  splashUrl: string;
  traits: string[];
  cost: number[];
  health: number[];
  attackDamage: number[];
  damagePerSecond: number[];
  attackRange: number;
  attackSpeed: number;
  armor: number;
  magicalResistance: number;
  skill: Skill;
  recommendItems: string[];
  isHiddenGuide?: boolean;
  isHiddenLanding?: boolean;
}

export interface Skill {
  name: string;
  image_url: string;
  desc: string;
  startingMana: number;
  skillMana: number;
  stats: string[];
}

export const UNIT_COLORS: BorderColors = {
  1: "var(--color-tier1)",
  2: "var(--color-tier2)",
  3: "var(--color-tier3)",
  4: "var(--color-tier4)",
  5: "var(--color-tier5)",
  9: "var(--color-kayle)",
};
