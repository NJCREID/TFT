import { ENDPOINT } from "./endpoints";
import allOption from "../assets/all-option.svg";
import { User } from "../context/authContext";

export interface Hex {
  unit: Unit;
  currentItems: Item[];
  isStarred: boolean;
  coordinates: number | null;
}

export interface UserGuide {
  id: number;
  initialUnit?: Unit;
  userId: number;
  usersUsername?: string;
  upVotes: number;
  downVotes: number;
  views: number;
  patch: string;
  name: string;
  stage2Desc: string;
  stage3Desc: string;
  stage4Desc: string;
  generalDesc: string;
  difficultyLevel: string;
  playStyle: string;
  tags: string[];
  hexes: Hex[];
  traits: UserGuideTrait[];
  augments: Augment[];
  comments: Comment[];
  isUpvote: boolean;
}

export interface Comment {
  author: string;
  userId: number;
  userGuideId: number;
  content: string;
}

export interface Augment {
  key: string;
  name: string;
  desc: string;
  imageUrl: string;
  tier: number;
}

export interface UserGuideTrait {
  value: number;
  trait: Trait;
  tier: number;
}

export interface Item {
  key: string;
  name: string;
  imageUrl: string;
  tags: string[];
  recipe: string[];
  desc: string;
  shortDesc?: string;
  itemStats?: string;
  isComponent?: boolean;
  affectedTraitKey?: string;
}

export interface Unit {
  key: string;
  name: string;
  imageUrl: string;
  splashImageUrl: string;
  centeredImageUrl: string;
  tier: number;
  cost: number[];
  health?: number[];
  attackDamage?: number[];
  damagePerSecond?: number[];
  attackRange?: number;
  attackSpeed?: number;
  armor?: number;
  magicalResistance?: number;
  recommendedItems: string[];
  heldItems: string[];
  skill?: Skill;
  traits: Trait[];
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

export interface Trait {
  desc?: string;
  tiers?: Tier[];
  id?: number;
  name: string;
  key: string;
  tierString?: string;
  imageUrl?: string;
}

export interface Tier {
  id: number;
  level: number;
  rarity: number;
  desc: string;
}

export interface TraitStat {
  name: string;
  numUnits: number;
  stat: Stat;
}

export interface BaseUnitStat {
  games: number;
  unitStats: UnitStat[];
  starredUnitStats: UnitStat[];
}

export interface BaseItemStat {
  games: number;
  itemStats: ItemStat[];
}

export interface BaseAugmentStat {
  games: number;
  augmentStats: AugmentStat[];
}

export interface BaseTraitStat {
  games: number;
  traitStats: TraitStat[];
}

export interface UnitStat {
  name: string;
  stat: Stat;
}

export interface ItemStat {
  name: string;
  stat: Stat;
}

export interface AugmentStat {
  name: string;
  stats: Stat[];
}

export interface Stat {
  games: number;
  place: number;
  top4: number;
  win: number;
  delta: number;
}

export interface Vote {
  id: number;
  postId: number;
  userId: number;
  isUpvote: boolean | null;
}

export interface VoteResponse {
  isUpvote: boolean | null;
  upVotes: number;
  downVotes: number;
}

export interface BaseCompStat {
  games: number;
  compStats: CompStat[];
}

export interface CompStat {
  name: string;
  units: Unit[];
  stat: Stat;
}

const BASE_API_URL = import.meta.env.VITE_BASE_API_URL;

export default async function fetchRequest<T>(
  endpoint: string,
  method: "GET" | "POST" | "PATCH" | "PUT" = "GET",
  body?: any,
  identifier?: string | null,
  authToken?: string
) {
  if (endpoint.includes("{identifier}") && !identifier) {
    throw new Error("Key is required for this endpoint.");
  }
  if (identifier) {
    endpoint = endpoint.replace("{identifier}", identifier);
  }

  const url = new URL(BASE_API_URL + endpoint);

  const requestOptions: RequestInit = {
    method: method,
    headers: {
      "Content-Type": "application/json",
      ...(authToken && { Authorization: `Bearer ${authToken}` }),
    },
    body: body ? JSON.stringify(body) : undefined,
  };

  const response = await fetch(url, requestOptions);
  return response.json() as Promise<T>;
}

const newItem: Item = {
  key: "all",
  name: "all",
  desc: "",
  imageUrl: allOption,
  recipe: [],
  tags: [],
};

export const fetchTraits = async () => {
  let fetchedTraits = await fetchRequest<Trait[]>(ENDPOINT.TRAIT_FETCH);
  return [{ key: "all", name: "Any Synergy", imageUrl: allOption }, ...fetchedTraits];
};

export const fetchComponents = async () => {
  let fetchedComponents = await fetchRequest<Item[]>(ENDPOINT.COMPONENT_FETCH);
  return [newItem, ...fetchedComponents];
};

export const fetchUserGuides = async (type: string) => {
  let fetchedUserGuides = await fetchRequest<UserGuide[]>(ENDPOINT.GUIDE_FETCH.replace("{identifier}", type));
  return fetchedUserGuides;
};
