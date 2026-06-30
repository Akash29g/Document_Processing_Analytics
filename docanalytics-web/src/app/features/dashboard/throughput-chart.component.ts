import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ChartCardComponent } from '../../shared/components/chart-card.component';
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
      [empty]="!loading() && !error() && data().length === 0">

      <svg class="chart" [attr.viewBox]="'0 0 ' + W + ' ' + H"
           preserveAspectRatio="none" role="img" aria-label="Throughput over time">
        @for (gl of gridLines(); track gl) {
          <line class="grid" x1="0" [attr.y1]="gl" [attr.x2]="W" [attr.y2]="gl" />
        }
        <polyline class="line" [attr.points]="linePoints()" />
      </svg>
    </app-chart-card>
  `,
  styles: [`
    .chart { width: 100%; height: 220px; }
    .grid { stroke: var(--light-gray); stroke-width: 1; }
    .line { fill: none; stroke: var(--slate-blue); stroke-width: 2; }
  `]
})
export class ThroughputChartComponent {
  data = input<SeriesPoint[]>([]);
  loading = input(false);
  error = input<string | null>(null);

  readonly W = 600;
  readonly H = 240;
  private pad = { top: 16, right: 12, bottom: 20, left: 30 };

  private maxVal = computed(() => Math.max(1, ...this.data().map(p => p.value)));

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
