import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { ChartCardComponent } from '../../../shared/components/chart-card.component';
import { SeriesPoint } from '../../../core/models/dashboard.model';

@Component({
  selector: 'app-status-distribution-chart',
  standalone: true,
  imports: [ChartCardComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,

  templateUrl: './status-distribution-chart.component.html',
  styleUrl: './status-distribution-chart.component.css',
})
export class StatusDistributionChartComponent {
  data = input<SeriesPoint[]>([]);
  loading = input(false);
  error = input<string | null>(null);
  retry = output<void>();

  rows = computed(() => {
    const d = this.data();
    const total = d.reduce((sum, p) => sum + p.value, 0) || 1;
    return d.map(p => {
      const k = p.label.toLowerCase().replace(/\s+/g, '');
      return {
        label: p.label,
        value: p.value,
        pct: Math.round((p.value / total) * 100),
        key: k === 'inprogress' ? 'processing' : k, // "In Progress" OR "Processing" → same blue fill
      };
    });
  });
}
