export interface Trait {
  name: string;
  inGameKey: string;
  desc?: string;
  tiers?: Tier[];
  tierString?: string;
  stats?: {
    [key: string]: string;
  };
}

export interface Tier {
  id?: number;
  level: number;
  rarity: number;
}
