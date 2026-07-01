import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject, untracked } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BatchService } from './batch.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { FilterBarComponent, FilterValues, FilterOption } from '../../shared/components/filter-bar.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';
import {
  ColumnDef, DataTableComponent, DtCellDirective, SortState,
} from '../../shared/components/data-table.component';
import { BatchListItem, BatchSortBy } from './batch.models';

@Component({
  selector: 'app-batch-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, RouterLink, FilterBarComponent, StatusBadgeComponent,
    DataTableComponent, DtCellDirective,
  ],
  template: `
    <section class="page">
      <header class="page-head">
        <h2 class="page-title">Batches</h2>
        <p class="page-sub">Browse and inspect document batches for this site.</p>
      </header>

      <div class="toolbar">
        <app-filter-bar
          [statusOptions]="statusOptions"
          [sourceOptions]="sourceOptions()"
          (changed)="onFilters($event)" />
        <input class="search-input" type="search" placeholder="Search by Batch ID…"
               [value]="svc.query().search ?? ''" (input)="onSearch($event)" />
      </div>

      <app-data-table
        [columns]="columns"
        [rows]="svc.batches()"
        [loading]="svc.loading()"
        [error]="svc.error()"
        emptyMessage="No batches match your filters."
        [sortBy]="svc.query().sortBy"
        [sortDir]="svc.query().sortDir"
        [page]="svc.query().page"
        [pageSize]="svc.query().pageSize"
        [totalCount]="svc.meta()?.total_count ?? 0"
        [totalPages]="svc.meta()?.total_pages ?? 1"
        (sortChange)="onSort($event)"
        (pageChange)="svc.setPage($event)"
        (pageSizeChange)="svc.setPageSize($event)"
        (retry)="svc.loadBatches()">

        <ng-template dtCell="transaction_id" let-row>
          <a class="batch-link" [routerLink]="[row.transaction_id]">{{ row.transaction_id }}</a>
        </ng-template>

        <ng-template dtCell="state" let-row>
          <app-status-badge [status]="row.state" />
        </ng-template>

        <ng-template dtCell="submitted_at" let-row>
          {{ row.submitted_at | date:'medium' }}
        </ng-template>

        <ng-template dtCell="last_updated" let-row>
          {{ row.last_updated_at | date:'medium' }}
        </ng-template>
      </app-data-table>
    </section>
  `,
  styles: [`
    .page { display: flex; flex-direction: column; gap: var(--space-3); padding: var(--space-3); }
    .page-title { margin: 0; font-family: var(--font-display); color: var(--dark-gray); }
    .page-sub { margin: 4px 0 0; color: var(--dark-gray-3); font-size: 0.85rem; }
    .toolbar { display: flex; flex-wrap: wrap; align-items: flex-end; gap: var(--space-2); }
    .search-input { height: 50px; min-width: 240px; padding: 0 10px; font: inherit;
                    border: 1px solid var(--cool-gray); border-radius: 6px; }
    .search-input:focus { outline: none; border-color: var(--slate-blue); }
    .batch-link { color: var(--slate-blue); font-family: monospace; text-decoration: none; }
    .batch-link:hover { text-decoration: underline; }
  `],
})
export class BatchListComponent {
  protected svc = inject(BatchService);
  private site = inject(SiteContextService);
  private destroyRef = inject(DestroyRef);
  private searchTimer?: ReturnType<typeof setTimeout>;

  // Status filter — VALUE 'in_progress' stays (backend maps → Processing); only the LABEL reads "Processing".
  //  VERIFY backend /batches?status=queued is wired before shipping the Queued option.
  protected statusOptions: FilterOption[] = [
    { value: 'all', label: 'All statuses' },
    { value: 'queued', label: 'Queued' },
    { value: 'in_progress', label: 'Processing' },
    { value: 'completed', label: 'Completed' },
    { value: 'failed', label: 'Failed' },
  ];

  // built from the endpoint
  protected sourceOptions = computed<FilterOption[]>(() =>
    this.svc.sources().map(s => ({ value: s, label: s })),
  );

  // Column keys = backend sort tokens. 'last_updated' is a sort token, so its
  // display value is pulled from last_updated_at via the accessor + cell template.
  protected columns: ColumnDef<BatchListItem>[] = [
    { key: 'transaction_id', header: 'Batch ID', width: '300px' },
    { key: 'state', header: 'Status', sortable: true },
    { key: 'total_files', header: 'Files', sortable: true, align: 'right' },
    { key: 'source_system', header: 'Source', sortable: true },
    { key: 'submitted_at', header: 'Submitted', sortable: true },
    { key: 'last_updated', header: 'Updated', sortable: true, value: r => r.last_updated_at },
  ];

  constructor() {
    effect(() => {
      const siteId = this.site.selectedSiteId();
      if (!siteId) return;
      untracked(() => {
        this.svc.loadBatches();
        this.svc.loadSources();
      });
    });
  }

  onFilters(f: FilterValues) { this.svc.setFilters(f); }
  onSort(s: SortState) { this.svc.setSort(s.sortBy as BatchSortBy, s.sortDir); }

  onSearch(e: Event) {
    const v = (e.target as HTMLInputElement).value;
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => this.svc.setSearch(v), 350);
  }
}
