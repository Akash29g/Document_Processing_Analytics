import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { ChartCardComponent } from '../../shared/components/chart-card/chart-card.component';
import { SeriesPoint } from '../../core/models/dashboard.model';

@Component({
  selector: 'app-throughput-chart',
  standalone: true,
  imports: [ChartCardComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <app-chart-card
      title="Throughput"
      subtitle="Documents processed per day"
      [loading]="loading()"
      [error]="error()"
      [empty]="!loading() && !error() && data().length === 0"
      (retry)="retry.emit()">

      <div class="tp">
        <!-- Y axis title -->
        <span class="y-title">Documents</span>

        <!-- Y axis tick labels (max → mid → 0) -->
        <div class="y-labels">
          <span>{{ maxVal() }}</span>
          <span>{{ midVal() }}</span>
          <span>0</span>
        </div>

        <!-- the plot -->
        <svg class="chart" [attr.viewBox]="'0 0 ' + W + ' ' + H"
             preserveAspectRatio="none" role="img" aria-label="Documents processed per day">
          @for (gl of gridLines(); track gl) {
            <line class="grid" x1="0" [attr.y1]="gl" [attr.x2]="W" [attr.y2]="gl" />
          }
          <polyline class="line" [attr.points]="linePoints()" />
        </svg>

        <!-- X axis tick labels (first → last date) -->
        <div class="x-labels">
          <span>{{ firstLabel() }}</span>
          <span>{{ lastLabel() }}</span>
        </div>
        <!-- X axis title -->
        <span class="x-title">Date</span>
      </div>
    </app-chart-card>
  `,
  styles: [`
    .tp {
      display: grid;
      grid-template-columns: auto auto 1fr;   /* y-title | y-labels | plot */
      grid-template-rows: 1fr auto auto;       /* plot | x-labels | x-title */
      column-gap: var(--space-1);
      width: 100%;
    }
    .y-title {
      grid-column: 1; grid-row: 1;
      writing-mode: vertical-rl; transform: rotate(180deg);
      align-self: center; font-size: 0.7rem; color: var(--dark-gray-3);
    }
    .y-labels {
      grid-column: 2; grid-row: 1;
      display: flex; flex-direction: column; justify-content: space-between;
      text-align: right; font-size: 0.7rem; color: var(--dark-gray-3);
      padding-right: 4px;
    }
    .chart { grid-column: 3; grid-row: 1; width: 100%; height: 220px; }
    .x-labels {
      grid-column: 3; grid-row: 2;
      display: flex; justify-content: space-between;
      font-size: 0.7rem; color: var(--dark-gray-3); margin-top: 4px;
    }
    .x-title {
      grid-column: 3; grid-row: 3;
      text-align: center; font-size: 0.7rem; color: var(--dark-gray-3); margin-top: 2px;
    }
    .grid { stroke: var(--light-gray); stroke-width: 1; }
    .line { fill: none; stroke: var(--slate-blue); stroke-width: 2; }
  `]
})
export class ThroughputChartComponent {
  data = input<SeriesPoint[]>([]);
  loading = input(false);
  error = input<string | null>(null);
  retry = output<void>();

  readonly W = 600;
  readonly H = 240;
  private pad = { top: 16, right: 12, bottom: 20, left: 8 };

  // public so the template axis labels can read them
  maxVal = computed(() => Math.max(1, ...this.data().map(p => p.value)));
  midVal = computed(() => Math.round(this.maxVal() / 2));
  firstLabel = computed(() => this.data()[0]?.label ?? '');
  lastLabel = computed(() => {
    const d = this.data();
    return d.length ? d[d.length - 1].label : '';
  });

  private x(i: number, n: number): number {
    const innerW = this.W - this.pad.left - this.pad.right;
    return this.pad.left + (n <= 1 ? innerW / 2 : (innerW * i) / (n - 1));
  }
  private y(v: number): number {
    const innerH = this.H - this.pad.top - this.pad.bottom;
    return this.pad.top + innerH * (1 - v / this.maxVal());
  }

  linePoints = computed(() => {
    const d = this.data();
    return d.map((p, i) => `${this.x(i, d.length)},${this.y(p.value)}`).join(' ');
  });

  gridLines = computed(() => {
    const innerH = this.H - this.pad.top - this.pad.bottom;
    return [0, 0.25, 0.5, 0.75, 1].map(t => this.pad.top + innerH * t);
  });
}
