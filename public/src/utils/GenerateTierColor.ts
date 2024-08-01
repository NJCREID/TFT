import GenerateTier from "./GenerateTier";

interface BackgroundColors {
  [key: number]: string[];
}

interface GenerateTierColorOptions {
  tier?: number;
  traitName?: string;
  amount?: number;
}

export default function GenerateTierColor({
  tier,
  traitName,
  amount,
}: GenerateTierColorOptions) {
  let newTier = tier || 0;
  if (!tier && traitName && amount) {
    newTier = GenerateTier(traitName, amount);
  }

  let backgroundStyle = {};
  let imageStyle = {};
  if (newTier === 4) {
    backgroundStyle = {
      backgroundImage: "var(--color-trait-tier4)",
      color: "#000",
      border: "2px solid var(--color-trait-tier4-border)",
    };
    imageStyle = {
      filter: "invert(100%)",
    };
  } else {
    const backgroundColors: BackgroundColors = {
      0: ["", ""],
      1: [
        "var(--color-trait-tier1)",
        "2px solid var(--color-trait-tier1-border)",
      ],
      2: [
        "var(--color-trait-tier2)",
        "2px solid var(--color-trait-tier2-border)",
      ],
      3: [
        "var(--color-trait-tier3)",
        "2px solid var(--color-trait-tier3-border)",
      ],
    };
    backgroundStyle = {
      backgroundColor: backgroundColors[newTier][0],
      border: backgroundColors[newTier][1],
      color: "#FFF",
    };
  }
  return { backgroundStyle, imageStyle };
}
