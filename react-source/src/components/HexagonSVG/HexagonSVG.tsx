import { UNIT_COLORS } from "../../data/UnitData";

export default function HexagonSVG({
  imageUrl,
  styles,
  cost,
}: {
  imageUrl?: string;
  styles?: React.CSSProperties | undefined;
  cost?: number | undefined;
}) {
  return (
    <svg
      className="app__hexagonsvg"
      version="1.1"
      xmlns="http://www.w3.org/2000/svg"
      xmlnsXlink="http://www.w3.org/1999/xlink"
      viewBox="0 0 84 96"
      xmlSpace="preserve"
      style={styles ? styles : {}}
    >
      <defs>
        <clipPath id="hexagonClip">
          <path
            d="M186,25.1606451 L186,70.8393549 L226,93.6964978 L266,70.8393549 L266,25.1606451 L226,2.30350221 L186,25.1606451 Z"
            transform="translate(-184, 0)"
          />
        </clipPath>
      </defs>
      <path
        stroke="none"
        fill="var(--color-secondary)"
        strokeWidth="4"
        d="M186,25.1606451 L186,70.8393549 L226,93.6964978 L266,70.8393549 L266,25.1606451 L226,2.30350221 L186,25.1606451 Z"
        transform="translate(-184, 0)"
      />
      {imageUrl && (
        <image
          xlinkHref={imageUrl}
          width="100%"
          height="100%"
          clipPath="url(#hexagonClip)"
          preserveAspectRatio="xMidYMid slice"
        />
      )}

      <path
        stroke={cost ? UNIT_COLORS[cost] : "var(--color-primary)"}
        fill="none"
        strokeWidth="4"
        d="M186,25.1606451 L186,70.8393549 L226,93.6964978 L266,70.8393549 L266,25.1606451 L226,2.30350221 L186,25.1606451 Z"
        transform="translate(-184, 0)"
      />
    </svg>
  );
}
