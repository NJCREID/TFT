import { Item } from "../data/ItemData";

const statPatterns = [
  {
    pattern:
      /\+(\d+%?) (Crit Chance|HP|Ability Power|Attack Speed|Armor|Attack Damage|Health|Crit|Range|Mana|Magic Resist|AP|All Stats)/,
    statGroup: 2,
  },
  {
    pattern:
      /(Crit Chance|HP|Ability Power|Attack Speed|Armor|Attack Damage|Health|Crit|Range|Mana|Magic Resist|AP|All Stats) \+(\d+%?)/,
    statGroup: 1,
  },
];

const stats: Record<string, string> = {
  "Crit Chance": "Crit",
  "Ability Power": "AP",
  Armor: "Armor",
  "Attack Damage": "Damage",
  "Attack Speed": "Speed",
  Health: "Health",
  Range: "Range",
  Mana: "Mana",
  "Magic Resist": "MR",
  AP: "AP",
  "All Stats": "All",
  HP: "Health",
  AD: "Damage",
};

export interface StatType {
  desc: string;
  value: string;
  stat: string;
}

export function getStatTypes(item: Item) {
  let desc = item.fromDesc || item.shortDesc || "";

  let splitDesc = [];
  let extractedStats: StatType[] = [];

  if (desc) {
    splitDesc = desc
      .split("<br>")
      .map((part) => part.trim())
      .filter((part) => part !== "");

    if (splitDesc.length === 1) {
      splitDesc = desc
        .split(",")
        .map((part) => part.trim())
        .filter((part) => part !== "");
    }

    splitDesc.forEach((desc) => {
      statPatterns.forEach(({ pattern, statGroup }) => {
        const match = desc.match(pattern);
        if (match) {
          let value = match[1] || match[2];
          let stat = match[statGroup];
          if (stat in stats) {
            extractedStats.push({
              desc: desc,
              value: value,
              stat: stats[stat],
            });
          }
        }
      });
    });
  }
  return extractedStats;
}
