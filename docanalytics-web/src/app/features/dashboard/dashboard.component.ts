import { ChangeDetectionStrategy, Component, DestroyRef, inject } from '@angular/core';
import { DashboardService } from './dashboard.service';
import { RefreshTimerService } from '../../core/services/refresh-timer.service';
import { ThroughputChartComponent } from './throughput-chart.component';
import { StatusDistributionChartComponent } from './status-distribution-chart.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  // 🔵 keep Akash's StatCard/DataTable imports here too when you merge
  imports: [ThroughputChartComponent, StatusDistributionChartComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="dash">
      <h1 class="page-title">Dashboard</h1>

      <!-- 🔵 Akash: summary counter tiles + recent-failures table render here -->

      <!-- 🟣 Shubh: charts -->
      <div class="charts-grid">
        <app-throughput-chart
          [data]="dash.throughput()"
          [loading]="dash.throughputLoading()"
          [error]="dash.throughputError()" />
        <app-status-distribution-chart
          [data]="dash.statusDistribution()"
          [loading]="dash.distributionLoading()"
          [error]="dash.distributionError()" />
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .dash { display: flex; flex-direction: column; gap: var(--space-2); }
    .page-title { font-family: var(--font-display); color: var(--dark-gray); margin: 0; }
    .charts-grid { display: grid; grid-template-columns: 1fr 1fr; gap: var(--space-2); }
    @media (max-width: 1100px) { .charts-grid { grid-template-columns: 1fr; } }
  `]
})
export class DashboardComponent {
  protected dash = inject(DashboardService);
  private poll = inject(RefreshTimerService);
  private destroyRef = inject(DestroyRef);

  constructor() {
    // single poll → refreshAll() loads all four datasets (yours + Akash's)
    this.poll.start(30_000, () => this.dash.refreshAll(), this.destroyRef);
  }
}
