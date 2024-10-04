export interface Item {
  inGameKey: string;
  key: string;
  name: string;
  tags: string[];
  recipe: string[];
  desc: string;
  fromDesc?: string;
  shortDesc?: string;
  isComponent?: boolean;
  affectedTraitKey?: string;
  traitCompatiblityKey?: string;
  unitCompatiblityKey?: string;
}

export const ITEM_TYPES = [
  { inGameKey: "unique", name: "Unique" },
  { inGameKey: "artifact", name: "Artifact" },
  { inGameKey: "fromitem", name: "From Item" },
  { inGameKey: "support", name: "Support" },
  { inGameKey: "radiant", name: "Radiant" },
  { inGameKey: "storyweaver", name: "Storyweaver" },
  { inGameKey: "inkshadow", name: "Ink Shadow" },
  { inGameKey: "emblem", name: "Emblem" },
];
