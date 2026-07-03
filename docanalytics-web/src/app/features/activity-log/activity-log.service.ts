import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Meta } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { SortDir } from '../../shared/components/data-table/data-table.component';
import { ActivityLogItem, ActivityLogQuery, ActivityLogSortBy } from './activity-log.models';

@Injectable({ providedIn: 'root' })
export class ActivityLogService {
  private http = inject(HttpClient);
  private base = environment.apiBase;

  // widget renders its own inline error → opt out of the global toast (NFR-2)
  private silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  // ── list slice ──
  private _rows = signal<ActivityLogItem[]>([]);
  private _meta = signal<Meta | null>(null);
  private _loading = signal(false);
  private _error = signal<string | null>(null);
  readonly rows = this._rows.asReadonly();
  readonly meta = this._meta.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();

  private _query: ActivityLogQuery = {
    page: 1, pageSize: 20,
    eventType: null, entityType: null, entity: null,
    from: null, to: null,
    sortBy: 'ts', sortDir: 'desc',           // newest first
  };
  get query(): ActivityLogQuery { return this._query; }

  // lowercase-first keys — ASP.NET binding is case-insensitive; matches your batches code
  private buildParams(q: ActivityLogQuery): HttpParams {
    let p = new HttpParams()
      .set('page', q.page)
      .set('pageSize', q.pageSize)
      .set('sortBy', q.sortBy)
      .set('sortDir', q.sortDir);
    if (q.eventType) p = p.set('eventType', q.eventType);
    if (q.entityType) p = p.set('entityType', q.entityType);
    if (q.entity) p = p.set('entity', q.entity);
    if (q.from) p = p.set('from', q.from);
    if (q.to) p = p.set('to', q.to);
    return p;
  }

  load(): void {
    this._loading.set(true);
    this._error.set(null);
    this.http
      .get<ApiResponse<ActivityLogItem[]>>(`${this.base}/activity-log`, {
        params: this.buildParams(this._query),
        ...this.silent,
      })
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (res) => {
          this._rows.set(res.data ?? []);
          this._meta.set(res.meta ?? null);
        },
        error: (err) => this._error.set(this.msg(err, 'Failed to load activity log.')),
      });
  }

  private patch(p: Partial<ActivityLogQuery>, resetPage = true): void {
    this._query = { ...this._query, ...p, page: resetPage ? 1 : (p.page ?? this._query.page) };
    this.load();
  }

  setFilters(f: { eventType: string | null; from: string | null; to: string | null }): void {
    this.patch({ eventType: f.eventType, from: f.from, to: f.to });
  }
  setEntitySearch(entity: string): void { this.patch({ entity: entity.trim() || null }); }
  setSort(sortBy: ActivityLogSortBy, sortDir: SortDir): void { this.patch({ sortBy, sortDir }); }
  setPage(page: number): void { this.patch({ page }, false); }
  setPageSize(pageSize: number): void { this.patch({ pageSize }); }

  private msg(err: any, fallback: string): string {
    return err?.error?.error?.message ?? fallback;
  }
}
