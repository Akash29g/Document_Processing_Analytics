import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { map } from 'rxjs/operators';
import { BatchService } from './batch.service';
import { BatchFile } from './batch.models';
import { SiteContextService } from '../../core/services/site-context.service';
import { StatCardComponent } from '../../shared/components/stat-card/stat-card.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import { ColumnDef, DataTableComponent, DtCellDirective } from '../../shared/components/data-table.component';

@Component({
  selector: 'app-batch-detail',
  imports: [RouterLink, StatCardComponent, StatusBadgeComponent, DataTableComponent, DtCellDirective, DatePipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="batch">
      <a class="bd-back" routerLink="..">← Back to batches</a>

      @if (batch.detailLoading()) {
        <!-- header skeleton -->
        <div class="head">
          <div class="titles">
            <div class="skel" style="height:11px;width:60px;"></div>
            <div class="skel" style="height:20px;width:280px;margin-top:8px;"></div>
            <div class="skel" style="height:13px;width:180px;margin-top:8px;"></div>
          </div>
          <div class="skel" style="height:26px;width:90px;border-radius:13px;"></div>
        </div>
        <div class="counters">
          <app-stat-card title="Uploaded"   [loading]="true"></app-stat-card>
          <app-stat-card title="Processing" [loading]="true"></app-stat-card>
          <app-stat-card title="Completed"  [loading]="true"></app-stat-card>
          <app-stat-card title="Failed"     [loading]="true"></app-stat-card>
        </div>

      } @else if (batch.detailError()) {
        <p class="inline-error">{{ batch.detailError() }}
          <button type="button" (click)="batch.loadDetail()">Retry</button></p>

      } @else if (batch.detail(); as d) {
        <!-- summary header -->
        <div class="head">
          <div class="titles">
            <p class="eyebrow">Batch</p>
            <h1 class="page-title">{{ d.id }}</h1>
            <p class="source">Source: {{ d.source }} · {{ d.total_files }} files</p>
          </div>
          <app-status-badge [status]="d.status" />
        </div>

        <!-- file_stats counters -->
        <div class="counters">
          <app-stat-card title="Uploaded"   [value]="d.file_stats.uploaded"></app-stat-card>
          <app-stat-card title="Processing" [value]="d.file_stats.processing"></app-stat-card>
          <app-stat-card title="Completed"  [value]="d.file_stats.completed"></app-stat-card>
          <app-stat-card title="Failed"     [value]="d.file_stats.failed"></app-stat-card>
        </div>
        <p class="times">
          Submitted {{ d.times.submitted_at | date: 'short' }}
          · Updated {{ d.times.last_updated_at | date: 'short' }}
          @if (d.times.completed_at) { · Completed {{ d.times.completed_at | date: 'short' }} }
        </p>
      }

      <!-- nested files table (Akash's DataTable) — pagination only, no sort -->
      <h2 class="section-title">Files</h2>
      <app-data-table
        [columns]="fileColumns" [rows]="batch.files()" [clickable]="true" [rowId]="fileRowId"
        [loading]="batch.filesLoading()" [error]="batch.filesError()"
        emptyMessage="No files in this batch"
        [page]="batch.filesQuery().page" [pageSize]="batch.filesQuery().pageSize"
        [totalCount]="batch.filesMeta()?.total_count ?? 0" [totalPages]="batch.filesMeta()?.total_pages ?? 1"
        (pageChange)="batch.setFilesPage($event)" (pageSizeChange)="batch.setFilesPageSize($event)"
        (retry)="batch.loadFiles()" (rowClick)="openFile($event)">
        <ng-template dtCell="status" let-row><app-status-badge [status]="row.status" /></ng-template>
        <ng-template dtCell="created_at" let-row>{{ row.created_at | date: 'short' }}</ng-template>
      </app-data-table>
    </section>
  `,
  styles: [`
    .batch { display: flex; flex-direction: column; gap: var(--space-3, 24px); padding: var(--space-3, 24px); }
    .bd-back { display: inline-block; font-size: .85rem; }
    .head { display: flex; align-items: flex-start; justify-content: space-between; gap: var(--space-2); flex-wrap: wrap; }
    .eyebrow { margin: 0; font-size: 0.72rem; text-transform: uppercase; letter-spacing: .04em; color: var(--dark-gray-3); }
    .page-title { font-family: var(--font-display); color: var(--dark-gray); margin: 2px 0 0; font-size: 1.1rem; word-break: break-all; }
    .source { margin: 4px 0 0; font-size: 0.82rem; color: var(--dark-gray-3); }
    .section-title { font-family: var(--font-display); font-size: 1.05rem; color: var(--dark-gray); margin: 0; }
    .counters { display: grid; grid-template-columns: repeat(auto-fit, minmax(160px, 1fr)); gap: var(--space-2, 16px); }
    .times { margin: 0; font-size: 0.8rem; color: var(--dark-gray-3); }
    .inline-error { color: var(--text-error); font-size: 0.85rem; }
    .inline-error button { margin-left: 8px; }

    /* skeleton shimmer (tokens → auto-flips in dark) */
    .skel {
      background: linear-gradient(90deg, var(--light-gray) 25%, var(--cool-gray) 37%, var(--light-gray) 63%);
      background-size: 400% 100%; animation: skel 1.4s ease infinite; border-radius: 4px;
    }
    @keyframes skel { 0% { background-position: 100% 50%; } 100% { background-position: 0 50%; } }
  `]
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
