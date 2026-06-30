import { ChangeDetectionStrategy, Component, DestroyRef, effect, inject } from '@angular/core';
import { DashboardService } from './dashboard.service';
import { RefreshTimerService } from '../../core/services/refresh-timer.service';
import { SiteContextService } from '../../core/services/site-context.service'; // 👈 adjust path if yours differs
import { ThroughputChartComponent } from './throughput-chart.component';
import { StatusDistributionChartComponent } from './status-distribution-chart.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [ThroughputChartComponent, StatusDistributionChartComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="dash">
      <h1 class="page-title">Dashboard</h1>

      <!-- 🔵 Akash: summary counter tiles + recent-failures table render here -->

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
  private site = inject(SiteContextService);
  private destroyRef = inject(DestroyRef);

  constructor() {
    // 🔁 Refetch immediately whenever the selected site changes (also fires on first load).
    effect(() => {
      const siteId = this.site.selectedSiteId();   // tracked dependency
      if (siteId) {
        this.dash.refreshAll();
      }
    });

    // ⏱️ Background 30s heartbeat (pauses on hidden tab).
    this.poll.start(30_000, () => this.dash.refreshAll(), this.destroyRef);
  }
}
