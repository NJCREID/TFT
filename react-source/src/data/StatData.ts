import { Unit } from "./UnitData";

export interface BaseUnitStat {
  games: number;
  unitStats: UnitStat[];
  starredUnitStats: UnitStat[];
}

export interface UnitStat {
  name: string;
  inGameKey: string;
  stat: Stat;
}

export interface BaseItemStat {
  games: number;
  itemStats: ItemStat[];
}

export interface ItemStat {
  name: string;
  inGameKey: string;
  stat: Stat;
}

export interface BaseAugmentStat {
  games: number;
  augmentStats: AugmentStat[];
}

export interface AugmentStat {
  name: string;
  inGameKey: string;
  stats: Stat[];
}

export interface BaseTraitStat {
  games: number;
  traitStats: TraitStat[];
}

export interface TraitStat {
  name: string;
  inGameKey: string;
  numUnits: number;
  stat: Stat;
}

export interface BaseCompStat {
  games: number;
  compStats: CompStat[];
}

export interface CompStat {
  name: string;
  inGameKey: string;
  units: Unit[];
  stat: Stat;
}

export interface BaseCoOcurrence {
  games: number;
  coOccurrences: CoOccurrence[];
}

export interface CoOccurrence {
  name: string;
  inGameKey: string;
  stat: Stat;
}

export interface Stat {
  games: number;
  place: number;
  top4: number;
  win: number;
  delta: number;
}
