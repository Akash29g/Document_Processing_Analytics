import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { ChartCardComponent } from '../../../shared/components/chart-card/chart-card.component';
import { SeriesPoint } from '../../../core/models/dashboard.model';

@Component({
  selector: 'app-status-distribution-chart',
  standalone: true,
  imports: [ChartCardComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <app-chart-card
      title="Status Distribution"
      subtitle="Documents by current status"
      [loading]="loading()"
      [error]="error()"
      [empty]="!loading() && !error() && data().length === 0"
      (retry)="retry.emit()"
    >
      <div class="bars">
        @for (row of rows(); track row.label) {
          <div class="row">
            <span class="label">{{ row.label }}</span>
            <div class="track">
              <div class="fill" [class]="'st-' + row.key" [style.width.%]="row.pct"></div>
            </div>
            <span class="val">{{ row.value }} · {{ row.pct }}%</span>
          </div>
        }
      </div>
    </app-chart-card>
  `,
  styles: [
    `
      .bars {
        display: flex;
        flex-direction: column;
        gap: var(--space-2);
        width: 100%;
        align-self: flex-start;
      }
      .row {
        display: grid;
        grid-template-columns: 110px 1fr 90px;
        align-items: center;
        gap: var(--space-1);
      }
      .label {
        font-size: 0.82rem;
        color: var(--dark-gray);
      }
      .track {
        background: var(--light-gray);
        border: 1px solid var(--cool-gray);
        border-radius: 6px;
        height: 18px;
        overflow: hidden;
      }
      .fill {
        height: 100%;
        border-radius: 6px 0 0 6px;
        transition: width 0.3s ease;
      }
      .val {
        font-size: 0.78rem;
        color: var(--dark-gray-3);
        text-align: right;
      }
      /* status colors = fills only (AVEVA rule) */
      .st-completed {
        background: var(--status-confirmed);
      }
      .st-failed {
        background: var(--status-error);
      }
      .st-processing {
        background: var(--status-warning);
      }
      .st-queued {
        background: var(--cool-gray);
      }
    `,
  ],
})
export class StatusDistributionChartComponent {
  data = input<SeriesPoint[]>([]);
  loading = input(false);
  error = input<string | null>(null);
  retry = output<void>();

  rows = computed(() => {
    const d = this.data();
    const total = d.reduce((sum, p) => sum + p.value, 0) || 1;
    return d.map((p) => {
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
