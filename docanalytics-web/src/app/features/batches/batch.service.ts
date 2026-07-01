import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Meta } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { SortDir } from '../../shared/components/data-table.component';
import { BatchListItem, BatchListQuery, BatchSortBy } from './batch.models';

const DEFAULT_QUERY: BatchListQuery = {
  page: 1, pageSize: 20, status: 'all', source: null,
  from: null, to: null, search: null, sortBy: 'last_updated', sortDir: 'desc',
};

@Injectable({ providedIn: 'root' })
export class BatchService {
  private http = inject(HttpClient);
  private base = environment.apiBase;
  private ctx = new HttpContext().set(SKIP_ERROR_TOAST, true);

  private _batches = signal<BatchListItem[]>([]);
  private _meta = signal<Meta | null>(null);
  private _loading = signal(false);
  private _error = signal<string | null>(null);
  private _query = signal<BatchListQuery>({ ...DEFAULT_QUERY });

  readonly batches = this._batches.asReadonly();
  readonly meta = this._meta.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly query = this._query.asReadonly();

  loadBatches(): void {
    const q = this._query();
    let params = new HttpParams()
      .set('page', q.page)
      .set('pageSize', q.pageSize)
      .set('sortBy', q.sortBy)
      .set('sortDir', q.sortDir);

    if (q.status && q.status !== 'all') params = params.set('status', q.status);
    if (q.source) params = params.set('source', q.source);
    if (q.from) params = params.set('from', q.from);
    if (q.to) params = params.set('to', q.to);
    if (q.search?.trim()) params = params.set('search', q.search.trim());

    this._loading.set(true);
    this._error.set(null);

    this.http.get<ApiResponse<BatchListItem[]>>(`${this.base}/batches`, { params, context: this.ctx })
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (res) => { this._batches.set(res.data ?? []); this._meta.set(res.meta ?? null); },
        error: () => this._error.set('Could not load batches. Please retry.'),
      });
  }

  private patch(p: Partial<BatchListQuery>, resetPage = true): void {
    this._query.update(q => ({ ...q, ...p, page: resetPage ? 1 : (p.page ?? q.page) }));
    this.loadBatches();
  }

  setFilters(f: { status: string; source: string | null; from: string | null; to: string | null }): void { this.patch(f); }
  setSearch(search: string): void { this.patch({ search }); }
  setSort(sortBy: BatchSortBy, sortDir: SortDir): void { this.patch({ sortBy, sortDir }, false); }
  setPage(page: number): void { this.patch({ page }, false); }
  setPageSize(pageSize: number): void { this.patch({ pageSize }); }
  reset(): void { this._query.set({ ...DEFAULT_QUERY }); this.loadBatches(); }
}
