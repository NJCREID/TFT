import { useState } from "react";
import "./oddsofhitting.css";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  LineElement,
  PointElement,
  Title,
  Tooltip,
  Legend,
  ScriptableChartContext,
  ScriptableScaleContext,
} from "chart.js";
import { Line } from "react-chartjs-2";

const BagSizes = [30, 25, 18, 10, 9];

const UnitOdds = [
  [1, 0, 0, 0, 0],
  [1, 0, 0, 0, 0],
  [0.75, 0.25, 0, 0, 0],
  [0.55, 0.3, 0.15, 0, 0],
  [0.45, 0.33, 0.2, 0.02, 0],
  [0.3, 0.4, 0.25, 0.05, 0],
  [0.19, 0.3, 0.4, 0.1, 0.01],
  [0.18, 0.25, 0.32, 0.22, 0.03],
  [0.1, 0.2, 0.25, 0.35, 0.1],
  [0.05, 0.1, 0.2, 0.4, 0.25],
  [0.01, 0.02, 0.12, 0.5, 0.35],
];

const DistinctChampions = [14, 13, 13, 12, 8];

ChartJS.register(CategoryScale, LinearScale, LineElement, PointElement, Title, Tooltip, Legend);

const OddsOfHitting = () => {
  const [unitCost, setUnitCost] = useState(1);
  const [currentLevel, setCurrentLevel] = useState(2);
  const [unitCopiesOut, setUnitCopiesOut] = useState(0);
  const [sameCostUnitsOut, setSameCostUnitsOut] = useState(0);
  const [goldToRoll, setGoldToRoll] = useState(30);

  function calculateProbabilities() {
    let transitionMatrix = getTransitionMatrix();
    let powerMatrix = power(transitionMatrix, 5 * Math.floor(goldToRoll / 2));

    const pprob = powerMatrix[0];
    const cprob = Array(10)
      .fill(0)
      .map((_, i) => {
        if (i === 0) return "1";
        return (1 - pprob.slice(0, i).reduce((sum, value) => sum + value, 0)).toFixed(2);
      });
    return [pprob, cprob];
  }

  function power(a: number[][], n: number): number[][] {
    let newmat = a;
    for (let i = 0; i < n - 1; i++) {
      newmat = multiply(newmat, a);
    }
    return newmat;
  }

  function multiply(a: number[][], b: number[][]): number[][] {
    const aNumRows = a.length;
    const bNumCols = b[0].length;
    const m = Array(aNumRows)
      .fill(0)
      .map(() => Array(bNumCols).fill(0));

    for (let r = 0; r < aNumRows; ++r) {
      for (let c = 0; c < bNumCols; ++c) {
        m[r][c] = a[r].reduce((sum, _, i) => sum + a[r][i] * b[i][c], 0);
      }
    }
    return m;
  }

  function getTransitionMatrix(): number[][] {
    const mat = Array(10)
      .fill(0)
      .map(() => Array(10).fill(0));
    for (let i = 0; i < 10; i++) {
      for (let j = 0; j < 10; j++) {
        if (i === 9 && j === 9) {
          mat[i][j] = 1;
        } else {
          const p = getTransitionProb();
          mat[i][j] = j === i ? 1 - p : j === i + 1 ? p : 0;
        }
      }
    }
    return mat;
  }

  function getTransitionProb(): number {
    const howManyLeft = Math.max(0, BagSizes[unitCost - 1] - unitCopiesOut);
    const poolSize = BagSizes[unitCost - 1] * DistinctChampions[unitCost - 1] - sameCostUnitsOut;
    return getCostProb(currentLevel, unitCost) * (howManyLeft / poolSize);
  }

  function getCostProb(lvl: number, cost: number): number {
    return UnitOdds[lvl - 1][cost - 1];
  }

  const [pprob, cprob] = calculateProbabilities();

  const data = {
    labels: Array.from({ length: pprob.length }, (_, i) => i),
    datasets: [
      {
        label: "Probability of Hitting Exactly",
        data: pprob.map((prob) => (typeof prob === "string" ? parseFloat(prob) : prob)),
        fill: false,
        borderColor: "#f2bf43",
        backgroundColor: "rgba(242, 191, 67, 0.2)",
        pointBackgroundColor: "#f2bf43",
        pointRadius: 5,
        tension: 0.1,
      },
      {
        label: "Cumulative Probability of Hitting At Least",
        data: cprob.map((prob) => (typeof prob === "string" ? parseFloat(prob) : prob)),
        fill: false,
        borderColor: "#df075a",
        backgroundColor: "rgba(223, 7, 90, 0.2)",
        pointBackgroundColor: "#df075a",
        pointBorderColor: "#240204",
        pointBorderWidth: 2,
        pointRadius: 5,
        tension: 0.1,
      },
    ],
  };

  const getFontSize = (context: ScriptableScaleContext | ScriptableChartContext) => {
    var avgSize = Math.round((context.chart.height + context.chart.width) / 2);
    var size = Math.round(avgSize / 32);
    size = size > 16 ? 16 : size;
    return {
      size: size,
    };
  };

  const options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        labels: {
          color: "#ffffff",
          font: (context: ScriptableChartContext) => getFontSize(context),
        },
      },
      title: {
        display: true,
        text: "Probability of Hitting",
        color: "#ffffff",
        font: (context: ScriptableChartContext) => getFontSize(context),
      },
    },
    scales: {
      x: {
        grid: {
          color: "#444444",
        },
        ticks: {
          color: "#ffffff",
          font: (context: ScriptableScaleContext) => getFontSize(context),
        },
      },
      y: {
        grid: {
          color: "#444444",
        },
        ticks: {
          color: "#ffffff",
          font: (context: ScriptableScaleContext) => getFontSize(context),
        },
      },
    },
  };

  return (
    <div className="app__oddsofhitting page_padding">
      <div className="app__oddsofhitting-wrapper">
        <p className="p__opensans-title-large">Odds of Hitting</p>
        <div className="app__oddsofhitting-container">
          <div className="app__oddsofhitting-filters">
            <div className="filter-item">
              <label className="p__opensans p__bold" htmlFor="unitCost">
                Unit Cost: {unitCost}
              </label>
              <input
                type="range"
                id="unitCost"
                min="1"
                max="5"
                value={unitCost}
                onChange={(e) => setUnitCost(parseInt(e.target.value))}
              />
            </div>
            <div className="filter-item">
              <label className="p__opensans p__bold" htmlFor="currentLevel">
                Current Level: {currentLevel}
              </label>
              <input
                type="range"
                id="currentLevel"
                min="2"
                max="11"
                value={currentLevel}
                onChange={(e) => setCurrentLevel(parseInt(e.target.value))}
              />
            </div>
            <div className="filter-item">
              <label className="p__opensans p__bold" htmlFor="unitCopiesOut">
                Number of Unit Copies Already Out:
              </label>
              <input
                type="number"
                id="unitCopiesOut"
                min="0"
                value={unitCopiesOut}
                onChange={(e) => setUnitCopiesOut(parseInt(e.target.value))}
              />
            </div>
            <div className="filter-item">
              <label className="p__opensans p__bold" htmlFor="sameCostUnitsOut">
                Number of Units of the Same Cost Already Out:
              </label>
              <input
                type="number"
                id="sameCostUnitsOut"
                min="0"
                value={sameCostUnitsOut}
                onChange={(e) => setSameCostUnitsOut(parseInt(e.target.value))}
              />
            </div>
            <div className="filter-item">
              <label className="p__opensans p__bold" htmlFor="goldToRoll">
                Amount of Gold You Want to Roll With:
              </label>
              <input
                type="number"
                id="goldToRoll"
                min="0"
                value={goldToRoll}
                onChange={(e) => setGoldToRoll(parseInt(e.target.value))}
              />
            </div>
          </div>
          <div className="app__oddsofhitting-chart">
            <Line data={data} options={options} />
          </div>
        </div>
      </div>
    </div>
  );
};

export default OddsOfHitting;
