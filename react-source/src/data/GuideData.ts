import { Augment } from "./AugmentData";

import { Unit } from "./UnitData";
import { Item } from "./ItemData";
import { Tier } from "./TraitData";

export interface Hex {
  unit: Unit;
  currentItems: Item[];
  isStarred: boolean;
  coordinates: number | null;
}

export interface UserGuide {
  id: number;
  userId?: number;
  usersUsername?: string;
  initialUnit?: Unit;
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
  hexes: Hex[];
  traits: UserGuideTrait[];
  augments: Augment[];
  comments: Comment[];
  isUpvote: boolean;
}

export interface UserGuideTrait {
  value: number;
  name: string;
  inGameKey: string;
  tierString: string;
  tier: number;
  tiers: Tier[];
}

export interface Comment {
  author: string;
  userId: number;
  userGuideId: number;
  content: string;
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
