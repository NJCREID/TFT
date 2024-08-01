export interface Item {
  key: string;
  ingameKey: string;
  name: string;
  desc: string;
  shortDesc: string;
  fromDesc: string;
  image_url: string;
  compositions?: string[];
  isNormal?: boolean;
  tags: string[];
  isUnique?: boolean;
  isSupport?: boolean;
  isArtifact?: boolean;
  isFromItem?: boolean;
}

export const ITEM_TYPES = [
  { key: "all", name: "All types" },
  { key: "unique", name: "Unique" },
  { key: "artifact", name: "Artifact" },
  { key: "fromitem", name: "From Item" },
  { key: "support", name: "Support" },
  { key: "radiant", name: "Radiant" },
  { key: "storyweaver", name: "Storyweaver" },
  { key: "inkshadow", name: "Ink Shadow" },
  { key: "emblem", name: "Emblem" },
];
