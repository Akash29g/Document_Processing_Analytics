import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { FileDetail, InvoiceDetail, RetryFileResponse } from './file-details.models';

@Injectable({ providedIn: 'root' })
export class FileDetailsService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBase;
  private readonly silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  private _fileId: string | null = null;

  // ── detail slice ──
  private _detail = signal<FileDetail | null>(null);
  private _detailLoading = signal(false);
  private _detailError = signal<string | null>(null);
  readonly detail = this._detail.asReadonly();
  readonly detailLoading = this._detailLoading.asReadonly();
  readonly detailError = this._detailError.asReadonly();

  // ── invoice slice ──
  private _invoice = signal<InvoiceDetail | null>(null);
  private _invoiceLoading = signal(false);
  private _invoiceError = signal<string | null>(null);
  private _hasInvoice = signal(true);
  readonly invoice = this._invoice.asReadonly();
  readonly invoiceLoading = this._invoiceLoading.asReadonly();
  readonly invoiceError = this._invoiceError.asReadonly();
  readonly hasInvoice = this._hasInvoice.asReadonly();

  // ── retry slice ──
  private _retrying = signal(false);
  private _retryError = signal<string | null>(null);
  readonly retrying = this._retrying.asReadonly();
  readonly retryError = this._retryError.asReadonly();

  load(fileId: string): void {
    this._fileId = fileId;
    this._retryError.set(null);
    this.loadDetails();
    this.loadLineItems();
  }

  loadDetails(): void {
    if (!this._fileId) return;
    this._detailLoading.set(true);
    this._detailError.set(null);
    this.http
      .get<ApiResponse<FileDetail>>(`${this.base}/files/${this._fileId}/details`, this.silent)
      .pipe(finalize(() => this._detailLoading.set(false)))
      .subscribe({
        next: (res) => this._detail.set(res.data),
        error: (err) => this._detailError.set(this.msg(err, 'Could not load file details.')),
      });
  }

  loadLineItems(): void {
    if (!this._fileId) return;
    this._invoiceLoading.set(true);
    this._invoiceError.set(null);
    this._hasInvoice.set(true);
    this.http
      .get<ApiResponse<InvoiceDetail>>(`${this.base}/files/${this._fileId}/line-items`, this.silent)
      .pipe(finalize(() => this._invoiceLoading.set(false)))
      .subscribe({
        next: (res) => this._invoice.set(res.data),
        error: (err) => {
          if (err?.status === 404) {
            this._hasInvoice.set(false);
            this._invoice.set(null);
          } else {
            this._invoiceError.set(this.msg(err, 'Could not load line items.'));
          }
        },
      });
  }

  /** Re-queues a failed file. Admin only — server enforces the role. */
  retryFile(): void {
    if (!this._fileId) return;
    this._retrying.set(true);
    this._retryError.set(null);
    this.http
      .post<ApiResponse<RetryFileResponse>>(
        `${this.base}/files/${this._fileId}/retry`,
        {},
        this.silent,
      )
      .pipe(finalize(() => this._retrying.set(false)))
      .subscribe({
        next: () => this.loadDetails(), // refresh — badge will flip to Queued
        error: (err) => this._retryError.set(this.msg(err, 'Retry failed. Please try again.')),
      });
  }

  downloadLogs(): void {
    if (!this._fileId) return;
    this.http
      .get(`${this.base}/files/${this._fileId}/logs`, {
        context: new HttpContext().set(SKIP_ERROR_TOAST, true),
        responseType: 'blob',
        observe: 'response',
      })
      .subscribe({
        next: (resp) => {
          const blob = resp.body!;
          const url = URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download =
            this.filenameFrom(resp.headers.get('content-disposition')) ??
            `file_${this._fileId}_log.txt`;
          a.click();
          URL.revokeObjectURL(url);
        },
      });
  }

  downloadOriginal(): void {
    if (!this._fileId) return;
    this.http
      .get<ApiResponse<{ url: string }>>(
        `${this.base}/files/${this._fileId}/download-url`,
        this.silent,
      )
      .subscribe({
        next: (res) => {
          if (res.data?.url) window.open(res.data.url, '_blank');
        },
      });
  }

  reset(): void {
    this._fileId = null;
    this._detail.set(null);
    this._detailError.set(null);
    this._invoice.set(null);
    this._invoiceError.set(null);
    this._hasInvoice.set(true);
    this._retryError.set(null);
  }

  private msg(err: any, fallback: string): string {
    return err?.error?.error?.message ?? fallback;
  }

  private filenameFrom(cd: string | null): string | null {
    if (!cd) return null;
    const m = /filename="?([^"]+)"?/i.exec(cd);
    return m ? m[1] : null;
  }
}
