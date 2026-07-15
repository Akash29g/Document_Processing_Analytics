import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, effect, inject, untracked } from '@angular/core';
import { ActivityLogService } from './activity-log.service';
import { ActivityLogItem, ActivityLogSortBy } from './activity-log.models';
import { SiteContextService } from '../../core/services/site-context.service';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import {
  FilterBarComponent,
  FilterOption,
  FilterValues,
} from '../../shared/components/filter-bar/filter-bar.component';
import {
  ColumnDef,
  DataTableComponent,
  DtCellDirective,
  SortState,
} from '../../shared/components/data-table/data-table.component';

@Component({
  selector: 'app-activity-log',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe,
    StatusBadgeComponent,
    FilterBarComponent,
    DataTableComponent,
    DtCellDirective,
  ],

  templateUrl: './activity-log.component.html',
  styleUrl: './activity-log.component.css',
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

  protected eventLabel(t: string): string {
    return ActivityLogComponent.EVENT_LABELS[t] ?? t;
  }

  protected onFilters(f: FilterValues): void {
    this.svc.setFilters({
      eventType: f.status === 'all' ? null : f.status, // first field repurposed as Event type
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
    this.searchTimer = setTimeout(() => this.svc.setEntitySearch(v), 300); // debounce
  }
}
