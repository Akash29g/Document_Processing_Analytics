import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { ErrorService } from '../error.service';
import { ErrorListItem, ErrorSortBy } from '../errors.models';
import { SiteContextService } from '../../../core/services/site-context.service';
import { ChartCardComponent } from '../../../shared/components/chart-card/chart-card.component';
import { FilterBarComponent, FilterOption, FilterValues } from '../../../shared/components/filter-bar.component';
import { ColumnDef, DataTableComponent, DtCellDirective, SortState } from '../../../shared/components/data-table.component';

@Component({
  selector: 'app-errors',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, ChartCardComponent, FilterBarComponent, DataTableComponent, DtCellDirective],
  templateUrl: './errors.component.html',
  styleUrl: './errors.component.css',
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
