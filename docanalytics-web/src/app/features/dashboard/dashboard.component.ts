import { ChangeDetectionStrategy, Component, DestroyRef, effect, inject } from '@angular/core';
import { DashboardService } from './dashboard.service';
import { RefreshTimerService } from '../../core/services/refresh-timer.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { RefreshTimerComponent } from '../../shared/components/refresh-timer.component';
import { ThroughputChartComponent } from './throughput-chart.component';
import { StatusDistributionChartComponent } from './status-distribution-chart.component';

// FR-1.5 configurable interval — single source of truth (could later move to environment.ts)
const DASHBOARD_REFRESH_MS = 30_000;

@Component({
  selector: 'app-dashboard',
  standalone: true,
  // 🔵 keep Akash's StatCard/DataTable imports here too when you merge
  imports: [ThroughputChartComponent, StatusDistributionChartComponent, RefreshTimerComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="dash">
      <div class="dash-head">
        <h1 class="page-title">Dashboard</h1>
        <app-refresh-timer
          [lastUpdated]="dash.lastUpdated()"
          [intervalMs]="refreshMs"
          [busy]="dash.throughputLoading() || dash.distributionLoading()"
          (refresh)="dash.refreshAll()" />
      </div>

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
    .dash-head { display: flex; align-items: center; justify-content: space-between; }
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

  protected refreshMs = DASHBOARD_REFRESH_MS;

  constructor() {
    // 🔁 Refetch ONLY on subsequent site switches — the poll's initial tick owns first load.
    let firstSiteRun = true;
    effect(() => {
      const siteId = this.site.selectedSiteId();
      if (!siteId) return;
      if (firstSiteRun) { firstSiteRun = false; return; }
      this.dash.refreshAll();
    });

    // ⏱️ 30s heartbeat: timer(0, …) handles first load + instant refetch on tab-return.
    this.poll.start(DASHBOARD_REFRESH_MS, () => this.dash.refreshAll(), this.destroyRef);
  }
}
