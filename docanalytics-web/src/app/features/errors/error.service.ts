import { HttpClient, HttpContext, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Meta } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { SortDir } from '../../shared/components/data-table/data-table.component';
import { ChartPoint, ErrorListItem, ErrorQuery, ErrorSortBy } from './errors.models';

const DEFAULT_QUERY: ErrorQuery = {
  page: 1, pageSize: 20, step: null, source: null,
  from: null, to: null, sortBy: 'failed_at', sortDir: 'desc', // ⚠️ VERIFY default sort token
};
const TOP_N = 10;

@Injectable({ providedIn: 'root' })
export class ErrorService {
  private http = inject(HttpClient);
  private base = environment.apiBase;
  private silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  // ===== 1 · Top-10 frequencies =====
  private _top = signal<ChartPoint[]>([]);
  private _topLoading = signal(false);
  private _topError = signal<string | null>(null);
  readonly top = this._top.asReadonly();
  readonly topLoading = this._topLoading.asReadonly();
  readonly topError = this._topError.asReadonly();

  loadTop(): void {
    const params = new HttpParams().set('topN', TOP_N);
    this._topLoading.set(true); this._topError.set(null);
    this.http.get<ApiResponse<{ points: ChartPoint[] }>>(`${this.base}/errors/top-frequencies`, { params, ...this.silent })
      .pipe(finalize(() => this._topLoading.set(false)))
      .subscribe({
        next: (res) => this._top.set(res.data?.points ?? []),
        error: () => this._topError.set('Could not load top errors. Please retry.'),
      });
  }

  // ===== 2 · Trend (respects from/to) =====
  private _trend = signal<ChartPoint[]>([]);
  private _trendLoading = signal(false);
  private _trendError = signal<string | null>(null);
  readonly trend = this._trend.asReadonly();
  readonly trendLoading = this._trendLoading.asReadonly();
  readonly trendError = this._trendError.asReadonly();

  loadTrend(): void {
    const q = this._query();
    let params = new HttpParams();
    if (q.from) params = params.set('from', q.from);
    if (q.to) params = params.set('to', q.to);
    this._trendLoading.set(true); this._trendError.set(null);
    this.http.get<ApiResponse<{ points: ChartPoint[] }>>(`${this.base}/errors/trend`, { params, ...this.silent })
      .pipe(finalize(() => this._trendLoading.set(false)))
      .subscribe({
        next: (res) => this._trend.set(res.data?.points ?? []),
        error: () => this._trendError.set('Could not load error trend. Please retry.'),
      });
  }

  // ===== 3 · Filtered/paginated errors list =====
  private _errors = signal<ErrorListItem[]>([]);
  private _meta = signal<Meta | null>(null);
  private _loading = signal(false);
  private _error = signal<string | null>(null);
  private _query = signal<ErrorQuery>({ ...DEFAULT_QUERY });
  readonly errors = this._errors.asReadonly();
  readonly meta = this._meta.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly query = this._query.asReadonly();

  // lowercase param keys — ASP.NET binding is case-insensitive; matches Akash's working batches code
  private buildParams(q: ErrorQuery): HttpParams {
    let params = new HttpParams()
      .set('page', q.page).set('pageSize', q.pageSize)
      .set('sortBy', q.sortBy).set('sortDir', q.sortDir);
    if (q.step) params = params.set('step', q.step);
    if (q.source) params = params.set('source', q.source);
    if (q.from) params = params.set('from', q.from);
    if (q.to) params = params.set('to', q.to);
    return params;
  }

  loadErrors(): void {
    const params = this.buildParams(this._query());
    this._loading.set(true); this._error.set(null);
    this.http.get<ApiResponse<ErrorListItem[]>>(`${this.base}/errors`, { params, ...this.silent })
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (res) => { this._errors.set(res.data ?? []); this._meta.set(res.meta ?? null); },
        error: () => this._error.set('Could not load errors. Please retry.'),
      });
  }

  private patch(p: Partial<ErrorQuery>, resetPage = true): void {
    this._query.update(q => ({ ...q, ...p, page: resetPage ? 1 : (p.page ?? q.page) }));
    this.loadErrors();
  }

  // FilterBar's first field is repurposed as STEP on this page ('all' → null)
  setFilters(f: { status: string; source: string | null; from: string | null; to: string | null }): void {
    const step = f.status && f.status !== 'all' ? f.status : null;
    this.patch({ step, source: f.source, from: f.from, to: f.to });
    this.loadTrend(); // trend follows the same date range
  }
  setSort(sortBy: ErrorSortBy, sortDir: SortDir): void { this.patch({ sortBy, sortDir }); }
  setPage(page: number): void { this.patch({ page }, false); }
  setPageSize(pageSize: number): void { this.patch({ pageSize }); }

  // ===== 4 · CSV export (applies current filters; keeps server filename) =====
  private _exporting = signal(false);
  private _exportError = signal<string | null>(null);
  readonly exporting = this._exporting.asReadonly();
  readonly exportError = this._exportError.asReadonly();

  exportCsv(): void {
    const params = this.buildParams(this._query());
    this._exporting.set(true); this._exportError.set(null);
    this.http.get(`${this.base}/errors/export`, {
      params, observe: 'response', responseType: 'blob', ...this.silent,
    })
      .pipe(finalize(() => this._exporting.set(false)))
      .subscribe({
        next: (res) => this.saveBlob(res),
        error: () => this._exportError.set('CSV export failed. Please retry.'),
      });
  }

  private saveBlob(res: HttpResponse<Blob>): void {
    const blob = res.body;
    if (!blob) { this._exportError.set('CSV export returned no file.'); return; }
    const cd = res.headers.get('content-disposition') ?? '';
    const m = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(cd);
    const filename = m ? decodeURIComponent(m[1]) : 'errors_export.csv';
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url; a.download = filename;
    document.body.appendChild(a); a.click();
    a.remove(); URL.revokeObjectURL(url);
  }

  load(): void { this.loadTop(); this.loadTrend(); this.loadErrors(); }
}
