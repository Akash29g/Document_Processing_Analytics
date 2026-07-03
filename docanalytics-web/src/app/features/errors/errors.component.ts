import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { ErrorService } from './error.service';
import { ErrorListItem, ErrorSortBy } from './errors.models';
import { SiteContextService } from '../../core/services/site-context.service';
import { ChartCardComponent } from '../../shared/components/chart-card/chart-card.component';
import { FilterBarComponent, FilterOption, FilterValues } from '../../shared/components/filter-bar.component';
import { ColumnDef, DataTableComponent, DtCellDirective, SortState } from '../../shared/components/data-table.component';

@Component({
  selector: 'app-errors',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, ChartCardComponent, FilterBarComponent, DataTableComponent, DtCellDirective],
  template: `
    <section class="page">
      <header class="page-head">
        <div>
          <h2 class="page-title">Error Analysis</h2>
          <p class="page-sub">Top failures, trend over time, and the full error log for this site.</p>
        </div>
        <button class="export-btn" type="button" [disabled]="svc.exporting()" (click)="svc.exportCsv()">
          {{ svc.exporting() ? 'Exporting…' : '⬇ Export CSV' }}
        </button>
      </header>

      @if (svc.exportError()) { <p class="inline-error">{{ svc.exportError() }}</p> }

     <div class="charts">
  <app-chart-card title="Top 10 Error Types" subtitle="Most frequent failure codes"
    [loading]="svc.topLoading()" [error]="svc.topError()" [empty]="!svc.top().length"
    emptyMessage="No errors recorded." (retry)="svc.loadTop()">
    <div class="bars">
      @for (p of svc.top(); track p.label) {
        <div class="bar-row">
          <span class="bar-label" [title]="p.label">{{ p.label }}</span>
          <div class="bar-track"><div class="bar-fill" [style.width.%]="pct(p.value, topMax())"></div></div>
          <span class="bar-val">{{ p.value }}</span>
        </div>
      }
    </div>
  </app-chart-card>

  <app-chart-card title="Error Trend" subtitle="Failures per day"
    [loading]="svc.trendLoading()" [error]="svc.trendError()" [empty]="!svc.trend().length"
    emptyMessage="No trend data." (retry)="svc.loadTrend()">
    <div class="trend">
      @for (p of svc.trend(); track p.label) {
        <div class="col" [title]="p.label + ': ' + p.value">
          <span class="col-val">{{ p.value }}</span>
          <div class="col-bar" [style.height.%]="pct(p.value, trendMax())"></div>
          <span class="col-label">{{ shortDate(p.label) }}</span>
        </div>
      }
    </div>
  </app-chart-card>
</div>


      <app-filter-bar
        statusLabel="Step" [statusOptions]="stepOptions" [sourceOptions]="sourceOptions"
        (changed)="onFilters($event)" />

      <app-data-table
        [columns]="columns" [rows]="svc.errors()" [loading]="svc.loading()" [error]="svc.error()"
        emptyMessage="No errors match your filters."
        [sortBy]="svc.query().sortBy" [sortDir]="svc.query().sortDir"
        [page]="svc.query().page" [pageSize]="svc.query().pageSize"
        [totalCount]="svc.meta()?.total_count ?? 0" [totalPages]="svc.meta()?.total_pages ?? 1"
        (sortChange)="onSort($event)" (pageChange)="svc.setPage($event)"
        (pageSizeChange)="svc.setPageSize($event)" (retry)="svc.loadErrors()">
        <ng-template dtCell="failed_at" let-row>{{ row.failed_at | date:'medium' }}</ng-template>
        <ng-template dtCell="error_code" let-row><span class="err-chip">{{ row.error_code }}</span></ng-template>
      </app-data-table>
    </section>
  `,
  styles: [`
    .page { display: flex; flex-direction: column; gap: var(--space-3); padding: var(--space-3); }
    .page-head { display: flex; align-items: flex-start; justify-content: space-between; gap: var(--space-2); flex-wrap: wrap; }
    .page-title { margin: 0; font-family: var(--font-display); color: var(--dark-gray); }
    .page-sub { margin: 4px 0 0; color: var(--dark-gray-3); font-size: 0.85rem; }
    .export-btn { height: 38px; padding: 0 16px; cursor: pointer; border-radius: 6px;
      border: 1px solid var(--slate-blue); background: var(--slate-blue); color: var(--white); font: inherit; }
    .export-btn:disabled { opacity: .55; cursor: default; }
    .inline-error { margin: 0; color: var(--text-error); font-size: 0.85rem; }
    .charts { display: grid; grid-template-columns: repeat(auto-fit, minmax(340px, 1fr)); gap: var(--space-2); }
    .bars { display: flex; flex-direction: column; gap: 8px; width: 100%; }
    .bar-row { display: grid; grid-template-columns: 190px 1fr 36px; align-items: center; gap: 10px; }
    .bar-label { font-size: 0.78rem; color: var(--dark-gray); font-family: monospace;
      overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
    .bar-track { height: 14px; background: var(--bg-light); border-radius: 7px; overflow: hidden; }
    .bar-fill { height: 100%; background: var(--status-error); border-radius: 7px; }
    .bar-val { font-size: 0.78rem; color: var(--dark-gray-3); text-align: right; }
    .trend { display: flex; align-items: flex-end; gap: 6px; width: 100%; height: 180px; }
    .col { flex: 1; display: flex; flex-direction: column; align-items: center; justify-content: flex-end; height: 100%; }
    .col-bar { width: 100%; min-height: 2px; background: var(--slate-blue); border-radius: 4px 4px 0 0; }
    .col-label { margin-top: 4px; font-size: 0.62rem; color: var(--dark-gray-3); }
    .col-val { font-size: 0.7rem; font-weight: 600; color: var(--dark-gray); margin-bottom: 2px; }
    .err-chip { font-family: monospace; font-size: 0.75rem; color: var(--text-error);
      background: var(--light-gray); padding: 2px 6px; border-radius: 4px; }
  `],
})
export class ErrorsComponent {
  protected svc = inject(ErrorService);
  private site = inject(SiteContextService);

  // ⚠️ VERIFY step tokens/casing accepted by backend `step` filter (seed shows Validate/Transform/Load)
  protected stepOptions: FilterOption[] = [
    { value: 'all', label: 'All steps' },
    { value: 'Upload', label: 'Upload' },
    { value: 'Validate', label: 'Validate' },
    { value: 'Transform', label: 'Transform' },
    { value: 'Load', label: 'Load' },
  ];
  // ⚠️ VERIFY full source list from DbSeeder sources[]
  protected sourceOptions: FilterOption[] = [
    { value: 'S3_Bucket_Alpha', label: 'S3_Bucket_Alpha' },
    { value: 'SFTP_Beta', label: 'SFTP_Beta' },
    { value: 'Manual_Upload', label: 'Manual_Upload' },
    { value: 'API_Upload', label: 'API_Upload' },
    { value: 'Azure_Blob_Gamma', label: 'Azure_Blob_Gamma' },
  ];

  protected columns: ColumnDef<ErrorListItem>[] = [
    { key: 'failed_at', header: 'Failed At', sortable: true, width: '170px' },
    { key: 'file_name', header: 'File', sortable: true },
    { key: 'error_code', header: 'Error', sortable: true, width: '160px' },
    { key: 'error_message', header: 'Message' },
    { key: 'step', header: 'Step', sortable: true, width: '110px' },
    { key: 'source', header: 'Source', sortable: true, width: '150px' },
    { key: 'suggested_fix', header: 'Suggested Fix' },
  ];

  protected topMax = computed(() => Math.max(1, ...this.svc.top().map(p => p.value)));
  protected trendMax = computed(() => Math.max(1, ...this.svc.trend().map(p => p.value)));

  constructor() {
    // reload everything on site switch (same guarded-effect pattern as batches)
    effect(() => { const s = this.site.selectedSiteId(); if (s) this.svc.load(); });
  }

  protected pct(v: number, max: number): number { return Math.round((v / max) * 100); }
  protected shortDate(label: string): string { return label?.length >= 10 ? label.slice(5) : label; } // MM-DD
  protected onFilters(f: FilterValues): void { this.svc.setFilters(f); }
  protected onSort(s: SortState): void { this.svc.setSort(s.sortBy as ErrorSortBy, s.sortDir); }
}
