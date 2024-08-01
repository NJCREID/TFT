// import allOption from "../assets/all-option.svg";

// interface TraitTiers {
//   description: string;
//   tiers: { [ingameKey: number]: [number, string] };
//   tierString: string;
//   image_url: string;
// }

// export interface TraitTiersData {
//   [trait: string]: TraitTiers;
// }

// export const TRAIT_TIERS: TraitTiersData = {
//   TFT11_Great: {
//     description:
//       "Every 3 casts, Wukong grows his weapon, modifying his Abilities.",
//     tiers: {
//       "1": [3, "Bonus Effect"],
//     },
//     tierString: "1",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Great.TFT_Set11.png",
//   },
//   TFT11_Lovers: {
//     description:
//       "Change which Lover takes the field depending on whether they are placed in the front or back 2 rows.\n\n When the fielded Lover casts, the other provides a bonus effect. \n\nFront: Altruist Rakan Back: Trickshot Xayah",
//     tiers: {
//       "1": [3, "Bonus Effect"],
//     },
//     tierString: "1",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Lovers.TFT_Set11.png",
//   },
//   TFT11_SpiritWalker: {
//     description:
//       "The first time the Spirit Walker drops below 50% Health, he unleashes the rage within, healing to full Health, gaining increased movement speed, and changing his Ability from Ram Slam to Tiger Strikes.",
//     tiers: {
//       "1": [3, "Bonus Effect"],
//     },
//     tierString: "1",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Spirit_Walker.TFT_Set11.png",
//   },
//   TFT11_Artist: {
//     description:
//       "The Artist paints the champion you place in a special bench slot. Get a 1-star copy of the champion placed there when the Artist's work is complete. \n\nRounds to Complete = Unit Cost",
//     tiers: {
//       "1": [3, "Bonus Effect"],
//     },
//     tierString: "1",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Artist.TFT_Set11.png",
//   },
//   TFT11_Porcelain: {
//     description:
//       "After casting, Porcelain champions boil, gaining Attack Speed and taking less damage for 4 seconds.",
//     tiers: {
//       "2": [1, "30AS% + 20% reduced damage"],
//       "4": [2, "60AS% + 35% reduced damage"],
//       "6": [3, "125AS% + 60% reduced damage"],
//     },
//     tierString: "2 / 4 / 6",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Porcelain.TFT_Set11.png",
//   },
//   TFT11_Sage: {
//     description:
//       "Combat start: Allies in the front 2 rows gain Omnivamp. Allies in the back 2 rows gain Ability Power.",
//     tiers: {
//       "2": [1, "12% Omnivamp, 15AP"],
//       "3": [2, "20% Omnivamp, 30AP"],
//       "4": [3, "30% Omnivamp, 45AP"],
//       "5": [4, "45% Omnivamp, 75AP"],
//     },
//     tierString: "2 / 3 / 4 / 5",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Sage.TFT_Set11.png",
//   },
//   TFT11_Dragonlord: {
//     description:
//       "After 8 seconds of combat, the Dragon strikes the board, dealing true damage to enemies and granting all allies Attack Speed for the rest of combat.",
//     tiers: {
//       "2": [1, "5% health damage, 12% attack speed"],
//       "3": [2, "10% health damage, 18% attack speed"],
//       "4": [3, "12% health damage and stuns for 1.5 seconds"],
//       "5": [4, "18% health damage, 45% attack speed"],
//     },
//     tierString: "2 / 3 / 4 / 5",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Dragonlord.TFT_Set11.png",
//   },
//   TFT11_Behemoth: {
//     description:
//       "Behemoths gain increased Armor and Magic Resist. Whenever a Behemoth dies, the nearest Behemoth gains 50% more for 5 seconds.",
//     tiers: {
//       "2": [1, "25 Armor and Magic Resist"],
//       "4": [3, "55 Armor and Magic Resist"],
//       "6": [3, "80 Armor and Magic Resist"],
//     },
//     tierString: "2 / 4 / 6",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Behemoth.TFT_Set11.png",
//   },
//   TFT11_Reaper: {
//     description: "Reapers abilities can critically strike",
//     tiers: {
//       "2": [1, "20% critical strike chance"],
//       "4": [
//         3,
//         "Additionally, Reapers bleed enemies for 45% bonus true damage over 3 seconds.",
//       ],
//     },
//     tierString: "2 / 4",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Reaper.TFT_Set11.png",
//   },
//   TFT11_Umbral: {
//     description:
//       "The moon illuminates hexes, Shielding units placed in them at the start of combat. \n\nUmbral units in illuminated hexes execute low Health enemies.",
//     tiers: {
//       "2": [1, "200 Shield; 10% Health execute"],
//       "4": [2, "500 Shield; 18% Health execute. More hexes are illuminated"],
//       "6": [3, "1000 Shield; 18% Health execute. Illuminate the whole board"],
//       "9": [
//         4,
//         "Executed enemies have a 100% chance to drop loot; 40% Health execute",
//       ],
//     },
//     tierString: "2 / 4 / 6 / 9",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Umbral.TFT_Set11.png",
//   },
//   TFT11_Dryad: {
//     description:
//       "Dryads gain Ability Power and 150 Health. Each enemy death grants permanent additional Health.",
//     tiers: {
//       "2": [1, "15 AP; 3 Health per enemy death"],
//       "4": [2, "30 AP; 7 Health per enemy death"],
//       "6": [3, "65 AP; 11 Health per enemy death"],
//     },
//     tierString: "2 / 4 / 6",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Dryad.TFT_Set11.png",
//   },
//   TFT11_Invoker: {
//     description: "Every 3 seconds, your units gain Mana.",
//     tiers: {
//       "2": [1, "5 Mana to all allies."],
//       "4": [2, "20 Mana to Invokers; 5 Mana to others."],
//       "6": [3, "35 Mana to Invokers; 20 Mana to others."],
//     },
//     tierString: "2 / 4 / 6",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_9_Preserver.png",
//   },
//   TFT11_Heavenly: {
//     description:
//       "Heavenly units grant unique stats to your team, increased by their star level and each Heavenly unit in play. \n\nKha'Zix - \nMalphite -  \nNeeko - \nQiyana - \nSoraka - \nWukong - \nEmblem - Omnivamp",
//     tiers: {
//       "2": [1, "100% bonus."],
//       "3": [1, "115% bonus."],
//       "4": [2, "135% bonus."],
//       "5": [3, "165% bonus."],
//       "6": [3, "200% bonus."],
//       "7": [4, "240% bonus."],
//     },
//     tierString: "2 / 3 / 4 / 5 / 6 / 7",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Heavenly.TFT_Set11.png",
//   },
//   TFT11_Altruist: {
//     description:
//       "Altruists heal the lowest Health ally for 15% of damage they deal. Your team gains Armor and Magic Resist.",
//     tiers: {
//       "2": [1, "10 Armor and Magic Resist"],
//       "3": [3, "20 Armor and Magic Resist"],
//       "4": [3, "40 Armor and Magic Resist"],
//     },
//     tierString: "2 / 3 / 4",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Altruist.TFT_Set11.png",
//   },
//   TFT11_Fated: {
//     description:
//       "Hover one Fated unit over another to form a pair and unlock a Fated Bonus. Your pair gains 20% Health.",
//     tiers: {
//       "3": [1, "Pair gets the Fated Bonus."],
//       "5": [2, "All Fated champions get 200% of the Fated Bonus"],
//       "7": [3, "All Fated champions get 300% of the Fated Bonus"],
//       "10": [4, "All Fated champions gain 300% of EVERY Fated Bonus"],
//     },
//     tierString: "3 / 5 / 7 / 10",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Fated.TFT_Set11.png",
//   },
//   TFT11_Duelist: {
//     description:
//       "Duelists gain Attack Speed on each attack, stacking up to 12 times.",
//     tiers: {
//       "2": [1, "5% Attack Speed"],
//       "4": [2, "9% Attack Speed"],
//       "6": [3, "13% Attack Speed;Duelists take 12% less damage"],
//       "8": [4, "18% Attack Speed; Duelists take 18% less damage"],
//     },
//     tierString: "2 / 4 / 6 / 8",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_8_Duelist.png",
//   },
//   TFT11_Sniper: {
//     description:
//       "Innate: Snipers gain 1 Attack Range. \n\nSnipers deal more damage to targets farther away.",
//     tiers: {
//       "2": [1, "7% damage per hex."],
//       "4": [3, "15% damage per hex."],
//       "6": [
//         3,
//         "30% damage per hex. Snipers gain an additional 2 Attack Range.",
//       ],
//     },
//     tierString: "2 / 4 / 6",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_3_Sniper.png",
//   },
//   TFT11_Inkshadow: {
//     description:
//       "Gain unique Inkshadow items. Inkshadow champions gain 5% bonus damage and damage reduction.",
//     tiers: {
//       "3": [
//         1,
//         "Gain unique Inkshadow items. Inkshadow champions gain 5% bonus damage and damage reduction.",
//       ],
//       "5": [
//         2,
//         "Gain unique Inkshadow items. Inkshadow champions gain 5% bonus damage and damage reduction.",
//       ],
//       "7": [
//         3,
//         "Gain unique Inkshadow items. Inkshadow champions gain 5% bonus damage and damage reduction.",
//       ],
//     },
//     tierString: "3 / 5 / 7",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Ink_Shadow.TFT_Set11.png",
//   },
//   TFT11_Warden: {
//     description:
//       "Wardens take less damage. For the first 10 seconds of combat, they take an additional 15% less damage.",
//     tiers: {
//       "2": [1, "10% less damage"],
//       "4": [2, "20% less damage"],
//       "6": [3, "33% less damage"],
//     },
//     tierString: "2 / 4 / 6",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Warden.TFT_Set11.png",
//   },
//   TFT11_Arcanist: {
//     description:
//       "Arcanists gain Ability Power and grant Ability Power to allies.",
//     tiers: {
//       "2": [1, "20 Mana to all allies."],
//       "4": [3, "50 Mana for Arcanists; 20 Mana for others"],
//       "6": [3, "85 Mana for Arcanists; 40 Mana for others"],
//       "8": [4, "125 Mana for Arcanists; 125 Mana for others"],
//     },
//     tierString: "2 / 4 / 6 / 8",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_6_Arcanist.png",
//   },
//   TFT11_Exalted: {
//     description:
//       "Your team gains 4% bonus damage, plus more based on your level. \n\nAfter combat, store 1 XP in a Soul Core. Sell the Core to claim the XP.",
//     tiers: {
//       "3": [2, "1% damage per level"],
//       "5": [3, "2.5% damage per level"],
//     },
//     tierString: "3 / 5",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Exalted.TFT_Set11.png",
//   },
//   TFT11_Ghostly: {
//     description:
//       "Upon dealing or taking damage 6 times, Ghostly units send 2 spectres to haunt nearby enemies and heal 2% max Health every 2 seconds. \n\nHaunted enemies take bonus damage for each spectre on them, and pass spectres on death.",
//     tiers: {
//       "2": [1, "5% per spectre"],
//       "4": [2, "10% per spectre"],
//       "6": [3, "16% per spectre"],
//       "8": [4, "32% per spectre"],
//     },
//     tierString: "2 / 4 / 6 / 8",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Ghostly.TFT_Set11.png",
//   },
//   TFT11_Mythic: {
//     description:
//       "Mythic champions gain Health, Ability Power, and Attack Damage. \n\nAfter 4 player combats, they become Epic, increasing the bonus by 50%.",
//     tiers: {
//       "3": [1, "+8% Health, 10% AP and AD"],
//       "5": [2, "+18% Health, 20% AP and AD"],
//       "7": [3, "+25% Health, 32% AP and AD"],
//       "10": [
//         4,
//         "Instantly become Epic. The bonus is increased by 250% instead.",
//       ],
//     },
//     tierString: "3 / 5 / 7 / 10",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Mythic.TFT_Set11.png",
//   },
//   TFT11_Storyweaver: {
//     description:
//       "Storyweavers summon a Hero named Kayle and evolve her. Storyweavers gain max Health. \n\nEach Storyweaver star level increases Kayle's Health and Ability Power. Kayle gets 15% Attack Speed for each game Stage.",
//     tiers: {
//       "3": [1, "Pick a supportive effect. 60 Health"],
//       "5": [2, "Pick a combat effect. 100 Health"],
//       "7": [3, "Pick a combat effect. 150 Health"],
//       "10": [4, "Ascend. 250 Health"],
//     },
//     tierString: "3 / 5 / 7 / 10",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Storyweaver.TFT_Set11.png",
//   },
//   TFT11_Trickshot: {
//     description:
//       "Trickshots' abilities ricochet. Each ricochet deals a percentage of the previous bounce's damage.",
//     tiers: {
//       "2": [1, "1 ricochet; 40% of previous damage"],
//       "4": [3, "2 ricochets; 60% of previous damage"],
//     },
//     tierString: "2 / 4",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_4_Sharpshooter.png",
//   },
//   TFT11_Bruiser: {
//     description:
//       "Your team gains 100 maximum Health. Bruisers gain additional maximum Health.",
//     tiers: {
//       "2": [1, "20% Health"],
//       "4": [2, "40% Health"],
//       "6": [3, "65% Health"],
//       "8": [
//         3,
//         "80% Health; Every 3 seconds, Bruisers deal 6% Health bonus physical damage on their next attack.",
//       ],
//     },
//     tierString: "2 / 4 / 6 / 8",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_6_Bruiser.png",
//   },
//   TFT11_Fortune: {
//     description:
//       "When you lose a fight, gain Luck. The more fights in a row you lose, the more Luck you get. \n\nLose Luck when you win.",
//     tiers: {
//       "3": [
//         1,
//         "Roll a die; in that many player combats, hold a Festival where you may convert your Luck into rewards.",
//       ],
//       "5": [2, "Heal 3 player health at the start of each player combat."],
//       "7": [
//         3,
//         "Fortune smiles! Gain extra rewards and more luck every turn no matter what, and hold a Festival every turn.",
//       ],
//     },
//     tierString: "3 / 5 / 7",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_4_Fortune.png",
//   },
// };

// export const TRAITS = [
//   { ingameKey: "all", key: "all", name: "Any Synergy", image_url: allOption },
//   {
//     key: "Great",
//     ingameKey: "TFT11_Great",
//     name: "Great",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Great.TFT_Set11.png",
//   },
//   {
//     key: "Lovers",
//     ingameKey: "TFT11_Lovers",
//     name: "Lovers",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Lovers.TFT_Set11.png",
//   },
//   {
//     key: "SpiritWalker",
//     ingameKey: "TFT11_SpiritWalker",
//     name: "Spirit Walker",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Spirit_Walker.TFT_Set11.png",
//   },
//   {
//     key: "Artist",
//     ingameKey: "TFT11_Artist",
//     name: "Artist",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Artist.TFT_Set11.png",
//   },
//   {
//     key: "Porcelain",
//     ingameKey: "TFT11_Porcelain",
//     name: "Porcelain",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Porcelain.TFT_Set11.png",
//   },
//   {
//     key: "Sage",
//     ingameKey: "TFT11_Sage",
//     name: "Sage",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Sage.TFT_Set11.png",
//   },
//   {
//     key: "Dragonlord",
//     ingameKey: "TFT11_Dragonlord",
//     name: "Dragonlord",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Dragonlord.TFT_Set11.png",
//   },
//   {
//     key: "Behemoth",
//     ingameKey: "TFT11_Behemoth",
//     name: "Behemoth",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Behemoth.TFT_Set11.png",
//   },
//   {
//     key: "Reaper",
//     ingameKey: "TFT11_Reaper",
//     name: "Reaper",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Reaper.TFT_Set11.png",
//   },
//   {
//     key: "Umbral",
//     ingameKey: "TFT11_Umbral",
//     name: "Umbral",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Umbral.TFT_Set11.png",
//   },
//   {
//     key: "Dryad",
//     ingameKey: "TFT11_Dryad",
//     name: "Dryad",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Dryad.TFT_Set11.png",
//   },
//   {
//     key: "Invoker",
//     ingameKey: "TFT11_Invoker",
//     name: "Invoker",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_9_Preserver.png",
//   },
//   {
//     key: "Heavenly",
//     ingameKey: "TFT11_Heavenly",
//     name: "Heavenly",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Heavenly.TFT_Set11.png",
//   },
//   {
//     key: "Altruist",
//     ingameKey: "TFT11_Altruist",
//     name: "Altruist",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Altruist.TFT_Set11.png",
//   },
//   {
//     key: "Fated",
//     ingameKey: "TFT11_Fated",
//     name: "Fated",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Fated.TFT_Set11.png",
//   },
//   {
//     key: "Duelist",
//     ingameKey: "TFT11_Duelist",
//     name: "Duelist",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_8_Duelist.png",
//   },
//   {
//     key: "Sniper",
//     ingameKey: "TFT11_Sniper",
//     name: "Sniper",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_3_Sniper.png",
//   },
//   {
//     key: "Inkshadow",
//     ingameKey: "TFT11_Inkshadow",
//     name: "Ink Shadow",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Ink_Shadow.TFT_Set11.png",
//   },
//   {
//     key: "Warden",
//     ingameKey: "TFT11_Warden",
//     name: "Warden",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Warden.TFT_Set11.png",
//   },
//   {
//     key: "Arcanist",
//     ingameKey: "TFT11_Arcanist",
//     name: "Arcanist",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_6_Arcanist.png",
//   },
//   {
//     key: "Exalted",
//     ingameKey: "TFT11_Exalted",
//     name: "Exalted",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Exalted.TFT_Set11.png",
//   },
//   {
//     key: "Ghostly",
//     ingameKey: "TFT11_Ghostly",
//     name: "Ghostly",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Ghostly.TFT_Set11.png",
//   },
//   {
//     key: "Mythic",
//     ingameKey: "TFT11_Mythic",
//     name: "Mythic",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Mythic.TFT_Set11.png",
//   },
//   {
//     key: "Storyweaver",
//     ingameKey: "TFT11_Storyweaver",
//     name: "Storyweaver",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_11_Storyweaver.TFT_Set11.png",
//   },
//   {
//     key: "Trickshot",
//     ingameKey: "TFT11_Trickshot",
//     name: "Trickshot",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_4_Sharpshooter.png",
//   },
//   {
//     key: "Bruiser",
//     ingameKey: "TFT11_Bruiser",
//     name: "Bruiser",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_6_Bruiser.png",
//   },
//   {
//     key: "Fortune",
//     ingameKey: "TFT11_Fortune",
//     name: "Fortune",
//     image_url:
//       "https://ddragon.leagueoflegends.com/cdn/14.7.1/img/tft-trait/Trait_Icon_4_Fortune.png",
//   },
// ];
