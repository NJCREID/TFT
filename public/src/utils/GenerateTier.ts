export default function GenerateTier(traitName: string, amount: number) {
  const traitTiers = TIERS[traitName];
  let tier = 0;

  for (const tierValue in traitTiers) {
    if (parseInt(tierValue) > amount) {
      break;
    }
    tier = traitTiers[tierValue];
  }
  return tier;
}

interface TierData {
  [trait: string]: {
    [tier: number]: number;
  };
}

export const TIERS: TierData = {
  Great: {
    1: 3,
  },
  Lovers: {
    1: 3,
  },
  SpiritWalker: {
    1: 3,
  },
  Artist: {
    1: 3,
  },
  Porcelain: {
    2: 1,
    4: 2,
    6: 3,
  },
  Sage: {
    2: 1,
    3: 2,
    4: 3,
    5: 4,
  },
  Dragonlord: {
    2: 1,
    3: 2,
    4: 3,
    5: 4,
  },
  Behemoth: {
    2: 1,
    4: 3,
    6: 3,
  },
  Reaper: {
    2: 1,
    4: 3,
  },
  Umbral: {
    2: 1,
    4: 2,
    6: 3,
    9: 4,
  },
  Dryad: {
    2: 1,
    4: 2,
    6: 3,
  },
  Invoker: {
    2: 1,
    4: 2,
    6: 3,
  },
  Heavenly: {
    2: 1,
    3: 1,
    4: 2,
    5: 3,
    6: 3,
    7: 4,
  },
  Altruist: {
    2: 1,
    3: 3,
    4: 3,
  },
  Fated: {
    3: 1,
    5: 2,
    7: 3,
    10: 4,
  },
  Duelist: {
    2: 1,
    4: 2,
    6: 3,
    8: 4,
  },
  Sniper: {
    2: 1,
    4: 3,
    6: 3,
  },
  Inkshadow: {
    3: 1,
    5: 2,
    7: 3,
  },
  Warden: {
    2: 1,
    4: 2,
    6: 3,
  },
  Arcanist: {
    2: 1,
    4: 3,
    6: 3,
    8: 4,
  },
  Exalted: {
    3: 2,
    5: 3,
  },
  Ghostly: {
    2: 1,
    4: 2,
    6: 3,
    8: 4,
  },
  Mythic: {
    3: 1,
    5: 2,
    7: 3,
    10: 4,
  },
  Storyweaver: {
    3: 1,
    5: 2,
    7: 3,
    10: 4,
  },
  Trickshot: {
    2: 1,
    4: 3,
  },
  Bruiser: {
    2: 1,
    4: 2,
    6: 3,
    8: 3,
  },
  Fortune: {
    3: 1,
    5: 2,
    7: 3,
  },
};
