import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, effect, inject, untracked } from '@angular/core';
import { ActivityLogService } from './activity-log.service';
import { ActivityLogItem, ActivityLogSortBy } from './activity-log.models';
import { SiteContextService } from '../../core/services/site-context.service';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import {
  FilterBarComponent, FilterOption, FilterValues,
} from '../../shared/components/filter-bar.component';
import {
  ColumnDef, DataTableComponent, DtCellDirective, SortState,
} from '../../shared/components/data-table.component';

@Component({
  selector: 'app-activity-log',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, StatusBadgeComponent, FilterBarComponent, DataTableComponent, DtCellDirective,
  ],
  template: `
    <section class="al">
      <header class="al-head">
        <div>
          <p class="al-eyebrow">Audit trail</p>
          <h2 class="al-title">Activity Log</h2>
        </div>
        <input
          class="al-search"
          type="search"
          placeholder="Search entity (file / batch)…"
          (input)="onSearch($event)" />
      </header>

      <app-filter-bar
        statusLabel="Event type"
        [statusOptions]="eventTypeOptions"
        [showSource]="false"
        [showDateRange]="true"
        (changed)="onFilters($event)" />

      <app-data-table
        [columns]="columns"
        [rows]="svc.rows()"
        [loading]="svc.loading()"
        [error]="svc.error()"
        emptyMessage="No activity for this site yet."
        [sortBy]="svc.query.sortBy"
        [sortDir]="svc.query.sortDir"
        [page]="svc.meta()?.page ?? 1"
        [pageSize]="svc.meta()?.page_size ?? 20"
        [totalCount]="svc.meta()?.total_count ?? 0"
        [totalPages]="svc.meta()?.total_pages ?? 1"
        (sortChange)="onSort($event)"
        (pageChange)="svc.setPage($event)"
        (pageSizeChange)="svc.setPageSize($event)"
        (retry)="svc.load()">

        <ng-template dtCell="ts" let-row>
          {{ row.ts | date: 'medium' }}
        </ng-template>

        <ng-template dtCell="event_type" let-row>
          <span class="al-evt">{{ eventLabel(row.event_type) }}</span>
        </ng-template>

        <ng-template dtCell="entity" let-row>
          <span class="al-entity">{{ row.entity ?? '—' }}</span>
          <span class="al-tag">{{ row.entity_type }}</span>
        </ng-template>

        <ng-template dtCell="transition" let-row>
          @if (row.old_state && row.new_state) {
            <span class="al-transition">
              <app-status-badge [status]="row.old_state" />
              <span class="al-arrow">→</span>
              <app-status-badge [status]="row.new_state" />
            </span>
          } @else if (row.new_state) {
            <app-status-badge [status]="row.new_state" />
          } @else {
            <span class="al-muted">—</span>
          }
        </ng-template>

        <ng-template dtCell="actor" let-row>
          <span class="al-actor">{{ row.actor }}</span>
        </ng-template>
      </app-data-table>
    </section>
  `,
  styles: [`
    .al { display: flex; flex-direction: column; gap: var(--space-2); }
    .al-head { display: flex; align-items: flex-end; justify-content: space-between; gap: var(--space-2); flex-wrap: wrap; }
    .al-eyebrow { margin: 0; font-size: 0.72rem; letter-spacing: .08em; text-transform: uppercase; color: var(--dark-gray-3); }
    .al-title { margin: 2px 0 0; font-family: var(--font-display); font-size: 1.25rem; color: var(--dark-gray); }
    .al-search {
      min-width: 240px; padding: 8px 12px; border: 1px solid var(--cool-gray);
      border-radius: 8px; font: inherit; color: var(--dark-gray); background: var(--white);
    }
    .al-search:focus { outline: none; border-color: var(--slate-blue); }
    .al-evt { font-weight: 600; color: var(--dark-gray); }
    .al-entity { color: var(--dark-gray); }
    .al-tag {
      margin-left: 8px; padding: 1px 8px; border-radius: 999px; font-size: 0.7rem;
      background: var(--bg-light); color: var(--dark-gray-3); border: 1px solid var(--cool-gray);
    }
    .al-transition { display: inline-flex; align-items: center; gap: 6px; }
    .al-arrow { color: var(--dark-gray-3); }
    .al-actor { color: var(--dark-gray-3); }
    .al-muted { color: var(--cool-gray); }
    @media (max-width: 1180px) { .al-search { flex: 1 1 100%; } }
  `],
})
export class ActivityLogComponent {
  protected svc = inject(ActivityLogService);
  private siteCtx = inject(SiteContextService);
  private searchTimer: any;

  protected readonly eventTypeOptions: FilterOption[] = [
    { value: 'all', label: 'All events' },
    { value: 'FILE_STATE_CHANGED', label: 'File state changed' },
    { value: 'BATCH_SUBMITTED', label: 'Batch submitted' },
    { value: 'BATCH_COMPLETED', label: 'Batch completed' },
    { value: 'BATCH_FAILED', label: 'Batch failed' },
  ];

  // 'transition' has no backing field → rendered via the dtCell template (not sortable)
  protected readonly columns: ColumnDef<ActivityLogItem>[] = [
    { key: 'ts', header: 'Timestamp', sortable: true, width: '190px' },
    { key: 'event_type', header: 'Event', sortable: true },
    { key: 'entity', header: 'Entity', sortable: true },
    { key: 'transition', header: 'Change' },
    { key: 'actor', header: 'Actor', width: '120px' },
  ];

  private static readonly EVENT_LABELS: Record<string, string> = {
    FILE_STATE_CHANGED: 'File state changed',
    BATCH_SUBMITTED: 'Batch submitted',
    BATCH_COMPLETED: 'Batch completed',
    BATCH_FAILED: 'Batch failed',
  };

  constructor() {
    // load on entry + reload on site switch; loader runs untracked so query-signal
    // reads inside load() don't re-fire the effect (the R3/R4 lesson).
    effect(() => {
      const site = this.siteCtx.selectedSiteId();
      if (!site) return;
      untracked(() => this.svc.load());
    });
  }

  protected eventLabel(t: string): string { return ActivityLogComponent.EVENT_LABELS[t] ?? t; }

  protected onFilters(f: FilterValues): void {
    this.svc.setFilters({
      eventType: f.status === 'all' ? null : f.status,   // first field repurposed as Event type
      from: f.from,
      to: f.to,
    });
  }

  protected onSort(s: SortState): void {
    this.svc.setSort(s.sortBy as ActivityLogSortBy, s.sortDir);
  }

  protected onSearch(e: Event): void {
    const v = (e.target as HTMLInputElement).value;
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => this.svc.setEntitySearch(v), 300);  // debounce
  }
}
