import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { map } from 'rxjs/operators';
import { BatchService } from '../batch.service';
import { BatchFile } from '../batch.models';
import { SiteContextService } from '../../../core/services/site-context.service';
import { StatCardComponent } from '../../../shared/components/stat-card/stat-card.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { ColumnDef, DataTableComponent, DtCellDirective } from '../../../shared/components/data-table.component';

@Component({
  selector: 'app-batch-detail',
  imports: [RouterLink, StatCardComponent, StatusBadgeComponent, DataTableComponent, DtCellDirective, DatePipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './batch-detail.component.html',
  styleUrl: './batch-detail.component.css',
})
export class BatchDetailComponent {
  protected readonly batch = inject(BatchService);
  private readonly site = inject(SiteContextService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  private readonly batchId = toSignal(
    this.route.paramMap.pipe(map(p => p.get('batchId'))), { initialValue: null as string | null });

  protected readonly fileColumns: ColumnDef<BatchFile>[] = [
    { key: 'file_name', header: 'File Name' },
    { key: 'file_type', header: 'Type', width: '80px' },
    { key: 'status', header: 'Status', width: '150px' },
    { key: 'current_step', header: 'Current Step', width: '140px' },
    { key: 'file_size_bytes', header: 'Size', align: 'right', width: '110px', value: (r) => this.formatSize(r.file_size_bytes) },
    { key: 'created_at', header: 'Created', align: 'right', width: '160px' },
  ];
  protected readonly fileRowId = (f: BatchFile) => f.id;

  constructor() {
    // re-fires on batch switch (param-only nav) AND on site switch — both guarded (R2 lesson)
    effect(() => {
      const id = this.batchId();
      const site = this.site.selectedSiteId();
      if (id && site) this.batch.load(id);
    });
  }

  private formatSize(bytes: number): string {
    if (!bytes) return '—';
    const kb = bytes / 1024;
    return kb < 1024 ? `${kb.toFixed(1)} KB` : `${(kb / 1024).toFixed(1)} MB`;
  }

  // navigate to /site/:siteId/batches/:batchId/files/:fileId (Akash's Round 4 route)
  protected openFile(f: BatchFile): void {
    this.router.navigate(['files', f.id], { relativeTo: this.route });
  }
}
