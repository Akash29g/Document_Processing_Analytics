import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject, untracked, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BatchService } from './batch.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { FilterBarComponent, FilterValues, FilterOption } from '../../shared/components/filter-bar/filter-bar.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import {
  ColumnDef, DataTableComponent, DtCellDirective, SortState,
} from '../../shared/components/data-table/data-table.component';
import { BatchListItem, BatchSortBy } from './batch.models';
import { AuthService } from '../../core/services/auth.service';


@Component({
  selector: 'app-batch-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, RouterLink, FilterBarComponent, StatusBadgeComponent,
    DataTableComponent, DtCellDirective,
  ],

  templateUrl: './batch-list.component.html',
  styleUrl: './batch-list.component.css',

})
export class BatchListComponent {
  protected svc = inject(BatchService);
  private site = inject(SiteContextService);
  private destroyRef = inject(DestroyRef);
  private searchTimer?: ReturnType<typeof setTimeout>;

  auth = inject(AuthService);
  isAdmin = computed(() => this.auth.currentUser()?.role === 'Admin');

  pendingDelete = signal<string | null>(null);

  confirmDelete(id: string) { this.pendingDelete.set(id); }

  doDelete() {
    const id = this.pendingDelete();
    if (!id) return;
    this.svc.deleteBatch(id).subscribe({
      next: () => { this.pendingDelete.set(null); this.svc.loadBatches(); },
      error: () => this.pendingDelete.set(null),
    });
  }


  // Status filter — VALUE 'in_progress' stays (backend maps → Processing); LABEL reads "Processing".
  // 'queued' now supported after the backend MapStatusToState fix.
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
    { key: 'actions', header: '', width: '52px' },

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
