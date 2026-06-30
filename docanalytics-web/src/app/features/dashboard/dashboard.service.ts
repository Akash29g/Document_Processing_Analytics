import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ApiResponse, Meta } from '../../core/models/api-response.model';
import {
  DashboardSummary, FailuresSortBy, RecentFailure, RecentFailuresQuery,
} from './dashboard.models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBase;

  // ───────────────────────── Dev A · Summary (FR-1.1) ─────────────────────────
  private readonly _summary = signal<DashboardSummary | null>(null);
  private readonly _summaryLoading = signal(false);
  private readonly _summaryError = signal<string | null>(null);
  readonly summary = this._summary.asReadonly();
  readonly summaryLoading = this._summaryLoading.asReadonly();
  readonly summaryError = this._summaryError.asReadonly();

  loadSummary(): void {
    this._summaryLoading.set(true);
    this._summaryError.set(null);
    this.http.get<ApiResponse<DashboardSummary>>(`${this.base}/dashboard/summary`).subscribe({
      next: (res) => { this._summary.set(res.data); this._summaryLoading.set(false); },
      error: () => { this._summaryError.set('Could not load summary counters.'); this._summaryLoading.set(false); },
    });
  }

  // ────────────────────── Dev A · Recent Failures (FR-1.4) ────────────────────
  private readonly _failures = signal<RecentFailure[]>([]);
  private readonly _failuresMeta = signal<Meta | null>(null);
  private readonly _failuresLoading = signal(false);
  private readonly _failuresError = signal<string | null>(null);
  private readonly _failuresQuery = signal<RecentFailuresQuery>({
    page: 1, pageSize: 10, sortBy: 'failed_at', sortDir: 'desc',
  });
  readonly failures = this._failures.asReadonly();
  readonly failuresMeta = this._failuresMeta.asReadonly();
  readonly failuresLoading = this._failuresLoading.asReadonly();
  readonly failuresError = this._failuresError.asReadonly();
  readonly failuresQuery = this._failuresQuery.asReadonly();

  loadFailures(): void {
    const q = this._failuresQuery();
    this._failuresLoading.set(true);
    this._failuresError.set(null);

    const params = new HttpParams()
      .set('page', q.page)
      .set('pageSize', q.pageSize)
      .set('sortBy', q.sortBy)
      .set('sortDir', q.sortDir);

    this.http.get<ApiResponse<RecentFailure[]>>(`${this.base}/dashboard/recent-failures`, { params }).subscribe({
      next: (res) => {
        this._failures.set(res.data ?? []);
        this._failuresMeta.set(res.meta ?? null);
        this._failuresLoading.set(false);
      },
      error: () => { this._failuresError.set('Could not load recent failures.'); this._failuresLoading.set(false); },
    });
  }

  setFailuresSort(sortBy: FailuresSortBy, sortDir: 'asc' | 'desc'): void {
    this._failuresQuery.update((q) => ({ ...q, sortBy, sortDir, page: 1 }));
    this.loadFailures();
  }
  setFailuresPage(page: number): void {
    this._failuresQuery.update((q) => ({ ...q, page }));
    this.loadFailures();
  }
  setFailuresPageSize(pageSize: number): void {
    this._failuresQuery.update((q) => ({ ...q, pageSize, page: 1 }));
    this.loadFailures();
  }

  // ───────────────────── Dev B · Throughput + Distribution ────────────────────
  // Shubh: add throughput/distribution signals + loadThroughput()/loadDistribution() here.

  // ─────────────────────────── CO-OWNED poll target ───────────────────────────
  // The RxJS poller (Shubh, FR-1.5) calls this every 30s. On merge, keep BOTH sets
  // of load() calls in here.
  refreshAll(): void {
    this.loadSummary();
    this.loadFailures();
    // this.loadThroughput();   // ← Shubh
    // this.loadDistribution(); // ← Shubh
  }
}
