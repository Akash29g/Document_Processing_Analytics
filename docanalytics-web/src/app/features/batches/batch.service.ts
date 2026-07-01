import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Meta } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { BatchDetail, BatchFile, FilesQuery } from './batch.models';

@Injectable({ providedIn: 'root' })
export class BatchService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBase;
  private readonly silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  private readonly _batchId = signal<string | null>(null);

  // ── Batch Detail (FR-2.4)
  private readonly _detail = signal<BatchDetail | null>(null);
  private readonly _detailLoading = signal(false);
  private readonly _detailError = signal<string | null>(null);
  readonly detail = this._detail.asReadonly();
  readonly detailLoading = this._detailLoading.asReadonly();
  readonly detailError = this._detailError.asReadonly();

  loadDetail(): void {
    const id = this._batchId();
    if (!id) return;
    this._detailLoading.set(true); this._detailError.set(null);
    this.http.get<ApiResponse<BatchDetail>>(`${this.base}/batches/${id}`, this.silent)
      .pipe(finalize(() => this._detailLoading.set(false)))
      .subscribe({
        next: (res) => this._detail.set(res.data ?? null),
        error: () => this._detailError.set('Could not load batch details.'),
      });
  }

  // ── Files (nested list — page + pageSize only, no sort)
  private readonly _files = signal<BatchFile[]>([]);
  private readonly _filesMeta = signal<Meta | null>(null);
  private readonly _filesLoading = signal(false);
  private readonly _filesError = signal<string | null>(null);
  private readonly _filesQuery = signal<FilesQuery>({ page: 1, pageSize: 10 });
  readonly files = this._files.asReadonly();
  readonly filesMeta = this._filesMeta.asReadonly();
  readonly filesLoading = this._filesLoading.asReadonly();
  readonly filesError = this._filesError.asReadonly();
  readonly filesQuery = this._filesQuery.asReadonly();

  loadFiles(): void {
    const id = this._batchId();
    if (!id) return;
    const q = this._filesQuery();
    this._filesLoading.set(true); this._filesError.set(null);
    const params = new HttpParams().set('page', q.page).set('pageSize', q.pageSize);
    this.http.get<ApiResponse<BatchFile[]>>(`${this.base}/batches/${id}/files`, { params, ...this.silent })
      .pipe(finalize(() => this._filesLoading.set(false)))
      .subscribe({
        next: (res) => { this._files.set(res.data ?? []); this._filesMeta.set(res.meta ?? null); },
        error: () => this._filesError.set('Could not load files.'),
      });
  }
  setFilesPage(page: number): void { this._filesQuery.update(q => ({ ...q, page })); this.loadFiles(); }
  setFilesPageSize(pageSize: number): void { this._filesQuery.update(q => ({ ...q, pageSize, page: 1 })); this.loadFiles(); }

  // ── Entry point the page calls
  load(batchId: string): void {
    this._batchId.set(batchId);
    this._filesQuery.set({ page: 1, pageSize: 10 });
    this.loadDetail();
    this.loadFiles();
  }
}
