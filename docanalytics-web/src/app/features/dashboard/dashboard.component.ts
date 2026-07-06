import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject,
} from '@angular/core';
import { DashboardService } from './dashboard.service';
import { RefreshTimerService } from '../../core/services/refresh-timer.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { RefreshTimerComponent } from '../../shared/components/refresh-timer/refresh-timer.component';
import { StatCardComponent } from '../../shared/components/stat-card/stat-card.component';
import {
  ColumnDef, DataTableComponent, DtCellDirective, SortState,
} from '../../shared/components/data-table/data-table.component';
import { ThroughputChartComponent } from './throughput-chart/throughput-chart.component';
import { StatusDistributionChartComponent } from './status-distribution-chart/status-distribution-chart.component';
import { FailuresSortBy, RecentFailure } from './dashboard.models';
import { RealtimeService } from '../../core/services/realtime.service';


const DASHBOARD_REFRESH_MS = 30_000;

@Component({
  selector: 'app-dashboard',
  imports: [
    StatCardComponent, DataTableComponent, DtCellDirective, DatePipe,
    ThroughputChartComponent, StatusDistributionChartComponent, RefreshTimerComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent {
  protected readonly dash = inject(DashboardService);
  private readonly poll = inject(RefreshTimerService);
  private readonly site = inject(SiteContextService);
  protected readonly realtime = inject(RealtimeService); 
  private readonly destroyRef = inject(DestroyRef);

  protected readonly refreshMs = DASHBOARD_REFRESH_MS;

  protected readonly busy = computed(() =>
    this.dash.summaryLoading() || this.dash.failuresLoading() ||
    this.dash.throughputLoading() || this.dash.distributionLoading());

  protected readonly columns: ColumnDef<RecentFailure>[] = [
    { key: 'file_name', header: 'File Name', sortable: true },
    { key: 'failed_step', header: 'Failed Step', sortable: true },
    { key: 'error', header: 'Error', sortable: false },
    { key: 'failed_at', header: 'Failed At', sortable: true, align: 'right', width: '160px' },
  ];

  constructor() {
    // initial load + reload on site switch (guarded so we never fire site-less)
    effect(() => {
      const siteId = this.site.selectedSiteId();
      if (siteId) this.dash.refreshAll();
    });

    // NEW (S-1): live push → refresh the dashboard the instant a file changes state
    effect(() => {
      const evt = this.realtime.lastEvent();
      if (evt) this.dash.refreshAll();
    });

    // recurring 30s + pause-on-hidden + refresh-on-return; tick is guarded too
    this.poll.start(DASHBOARD_REFRESH_MS, () => {
      if (this.site.selectedSiteId()) this.dash.refreshAll();
    }, this.destroyRef);
  }

  protected onSort(s: SortState): void {
    this.dash.setFailuresSort(s.sortBy as FailuresSortBy, s.sortDir);
  }
}
