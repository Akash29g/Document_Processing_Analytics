import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { ChartCardComponent } from '../../../shared/components/chart-card/chart-card.component';
import { SeriesPoint } from '../../../core/models/dashboard.model';

@Component({
  selector: 'app-throughput-chart',
  standalone: true,
  imports: [ChartCardComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './throughput-chart.component.html',
  styleUrl: './throughput-chart.component.css',
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
