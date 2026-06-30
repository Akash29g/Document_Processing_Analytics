import { DatePipe } from '@angular/common';
import { Component, effect, inject } from '@angular/core';
import { SiteContextService } from '../../core/services/site-context.service';
import { StatCardComponent } from '../../shared/components/stat-card.component';
import {
  ColumnDef, DataTableComponent, DtCellDirective, SortState,
} from '../../shared/components/data-table.component';
import { DashboardService } from './dashboard.service';
import { FailuresSortBy, RecentFailure } from './dashboard.models';

@Component({
  selector: 'app-dashboard',
  imports: [StatCardComponent, DataTableComponent, DtCellDirective, DatePipe],
  template: `
    <section class="page">
      <h1 class="page-title">Dashboard</h1>

      <!-- FR-1.1 counters -->
      <div class="counters">
        <app-stat-card title="Queued"      [value]="summary()?.queued ?? 0"></app-stat-card>
        <app-stat-card title="In Progress" [value]="summary()?.in_progress ?? 0"></app-stat-card>
        <app-stat-card title="Completed"   [value]="summary()?.completed ?? 0"></app-stat-card>
        <app-stat-card title="Failed"      [value]="summary()?.failed ?? 0"></app-stat-card>
      </div>
      @if (summaryError()) {
        <p class="inline-error">{{ summaryError() }}
          <button type="button" (click)="dash.loadSummary()">Retry</button>
        </p>
      }

      <!-- FR-1.4 recent failures -->
      <h2 class="section-title">Recent Failures</h2>
      <app-data-table
        [columns]="columns"
        [rows]="failures()"
        [loading]="failuresLoading()"
        [error]="failuresError()"
        emptyMessage="No recent failures 🎉"
        [sortBy]="query().sortBy"
        [sortDir]="query().sortDir"
        [page]="query().page"
        [pageSize]="query().pageSize"
        [totalCount]="failuresMeta()?.total_count ?? 0"
        [totalPages]="failuresMeta()?.total_pages ?? 1"
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
    .page { padding: var(--space-3, 24px); display: flex; flex-direction: column; gap: var(--space-3, 24px); }
    .page-title { font-family: var(--font-display); color: var(--dark-gray); margin: 0; }
    .section-title { font-family: var(--font-display); font-size: 1.05rem; color: var(--dark-gray); margin: 0; }
    .counters { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: var(--space-2, 16px); }
    .inline-error { color: var(--text-error); font-size: 0.85rem; }
    .inline-error button { margin-left: 8px; }
    .err-code { font-weight: 600; color: var(--dark-gray); }
    .err-msg { color: var(--dark-gray-3); }
  `],
})
export class DashboardComponent {
  protected readonly dash = inject(DashboardService);
  private readonly siteCtx = inject(SiteContextService);

  protected readonly summary = this.dash.summary;
  protected readonly summaryError = this.dash.summaryError;
  protected readonly failures = this.dash.failures;
  protected readonly failuresMeta = this.dash.failuresMeta;
  protected readonly failuresLoading = this.dash.failuresLoading;
  protected readonly failuresError = this.dash.failuresError;
  protected readonly query = this.dash.failuresQuery;

  protected readonly columns: ColumnDef<RecentFailure>[] = [
    { key: 'file_name', header: 'File Name', sortable: true },
    { key: 'failed_step', header: 'Failed Step', sortable: true },
    { key: 'error', header: 'Error', sortable: false },
    { key: 'failed_at', header: 'Failed At', sortable: true, align: 'right', width: '160px' },
  ];

  constructor() {
    // Initial load + reload whenever the site changes (FR-5.2).
    effect(() => {
      const siteId = this.siteCtx.selectedSiteId();
      if (siteId) this.dash.refreshAll();
    });
  }

  protected onSort(s: SortState): void {
    this.dash.setFailuresSort(s.sortBy as FailuresSortBy, s.sortDir);
  }
}
