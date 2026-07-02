import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject,
} from '@angular/core';
import { DashboardService } from './dashboard.service';
import { RefreshTimerService } from '../../core/services/refresh-timer.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { RefreshTimerComponent } from '../../shared/components/refresh-timer.component';
import { StatCardComponent } from '../../shared/components/stat-card.component';
import {
  ColumnDef, DataTableComponent, DtCellDirective, SortState,
} from '../../shared/components/data-table.component';
import { ThroughputChartComponent } from './throughput-chart.component';
import { StatusDistributionChartComponent } from './status-distribution-chart.component';
import { FailuresSortBy, RecentFailure } from './dashboard.models';

const DASHBOARD_REFRESH_MS = 30_000;

@Component({
  selector: 'app-dashboard',
  imports: [
    StatCardComponent, DataTableComponent, DtCellDirective, DatePipe,
    ThroughputChartComponent, StatusDistributionChartComponent, RefreshTimerComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="dash">
      <div class="dash-head">
        <h1 class="page-title">Dashboard</h1>
        <app-refresh-timer
          [lastUpdated]="dash.lastUpdated()"
          [intervalMs]="refreshMs"
          [busy]="busy()"
          (refresh)="dash.refreshAll()" />
      </div>

      <!-- FR-1.1 counters -->
      <div class="counters">
        <app-stat-card title="Queued"      [value]="dash.summary()?.queued ?? 0"></app-stat-card>
        <app-stat-card title="In Progress" [value]="dash.summary()?.in_progress ?? 0"></app-stat-card>
        <app-stat-card title="Completed"   [value]="dash.summary()?.completed ?? 0"></app-stat-card>
        <app-stat-card title="Failed"      [value]="dash.summary()?.failed ?? 0"></app-stat-card>
      </div>
      @if (dash.summaryError()) {
        <p class="inline-error">{{ dash.summaryError() }}
          <button type="button" (click)="dash.loadSummary()">Retry</button>
        </p>
      }

      <!-- FR-1.2 / FR-1.3 charts -->
      <div class="charts-grid">
        <app-throughput-chart
          [data]="dash.throughput()"
          [loading]="dash.throughputLoading()"
          [error]="dash.throughputError()"
          (retry)="dash.refreshAll()"/>
        <app-status-distribution-chart
          [data]="dash.statusDistribution()"
          [loading]="dash.distributionLoading()"
          [error]="dash.distributionError()"
          (retry)="dash.refreshAll()" />
      </div>

      <!-- FR-1.4 recent failures -->
      <h2 class="section-title">Recent Failures</h2>
      <app-data-table
        [columns]="columns"
        [rows]="dash.failures()"
        [loading]="dash.failuresLoading()"
        [error]="dash.failuresError()"
        emptyMessage="No recent failures 🎉"
        [sortBy]="dash.failuresQuery().sortBy"
        [sortDir]="dash.failuresQuery().sortDir"
        [page]="dash.failuresQuery().page"
        [pageSize]="dash.failuresQuery().pageSize"
        [totalCount]="dash.failuresMeta()?.total_count ?? 0"
        [totalPages]="dash.failuresMeta()?.total_pages ?? 1"
        (sortChange)="onSort($event)"
        (pageChange)="dash.setFailuresPage($event)"
        (pageSizeChange)="dash.setFailuresPageSize($event)"
        (retry)="dash.loadFailures()">

        <ng-template dtCell="error" let-row>
          <span class="err-code">{{ row.error_code || '—' }}</span>
          @if (row.error_message) { <span class="err-msg"> — {{ row.error_message }}</span> }
        </ng-template>

        <ng-template dtCell="failed_at" let-row>
          {{ row.failed_at | date: 'short' }}
        </ng-template>
      </app-data-table>
    </section>
  `,
  styles: [`
    .dash { display: flex; flex-direction: column; gap: var(--space-3, 24px); padding: var(--space-3, 24px); }
    .dash-head { display: flex; align-items: center; justify-content: space-between; flex-wrap: wrap; gap: var(--space-2); }
    .page-title { font-family: var(--font-display); color: var(--dark-gray); margin: 0; }
    .section-title { font-family: var(--font-display); font-size: 1.05rem; color: var(--dark-gray); margin: 0; }
    .counters { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: var(--space-2, 16px); }
    .charts-grid { display: grid; grid-template-columns: 1fr 1fr; gap: var(--space-2, 16px); }
    @media (max-width: 1100px) { .charts-grid { grid-template-columns: 1fr; } }
    .inline-error { color: var(--text-error); font-size: 0.85rem; }
    .inline-error button { margin-left: 8px; }
    .err-code { font-weight: 600; color: var(--dark-gray); }
    .err-msg { color: var(--dark-gray-3); }
  `],
})
export class DashboardComponent {
  protected readonly dash = inject(DashboardService);
  private readonly poll = inject(RefreshTimerService);
  private readonly site = inject(SiteContextService);
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
    // recurring 30s + pause-on-hidden + refresh-on-return; tick is guarded too
    this.poll.start(DASHBOARD_REFRESH_MS, () => {
      if (this.site.selectedSiteId()) this.dash.refreshAll();
    }, this.destroyRef);
  }

  protected onSort(s: SortState): void {
    this.dash.setFailuresSort(s.sortBy as FailuresSortBy, s.sortDir);
  }
}
