// One generic series shape — both throughput & status-distribution return this.
export interface SeriesPoint {
  label: string;   // throughput: "2026-05-30"  |  distribution: "Completed"
  value: number;
}
export interface ChartSeries {
  points: SeriesPoint[];
}
