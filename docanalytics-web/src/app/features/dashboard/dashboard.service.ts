import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Meta } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { ChartSeries, SeriesPoint } from '../../core/models/dashboard.model';
import {
  DashboardSummary, FailuresSortBy, RecentFailure, RecentFailuresQuery, StepPercentile,
} from './dashboard.models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBase;

  // tells the error interceptor "don't toast — widgets show errors inline"
  private readonly silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  // ───────── Dev A · Summary (FR-1.1) ─────────
  private readonly _summary = signal<DashboardSummary | null>(null);
  private readonly _summaryLoading = signal(false);
  private readonly _summaryError = signal<string | null>(null);
  readonly summary = this._summary.asReadonly();
  readonly summaryLoading = this._summaryLoading.asReadonly();
  readonly summaryError = this._summaryError.asReadonly();

  loadSummary(): void {
    this._summaryLoading.set(true);
    this._summaryError.set(null);
    this.http.get<ApiResponse<DashboardSummary>>(`${this.base}/dashboard/summary`, this.silent)
      .pipe(finalize(() => this._summaryLoading.set(false)))
      .subscribe({
        next: (res) => this._summary.set(res.data),
        error: () => this._summaryError.set('Could not load summary counters.'),
      });
  }

  // ───────── Dev A · Recent Failures (FR-1.4) ─────────
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
      .set('page', q.page).set('pageSize', q.pageSize)
      .set('sortBy', q.sortBy).set('sortDir', q.sortDir);
    this.http.get<ApiResponse<RecentFailure[]>>(
      `${this.base}/dashboard/recent-failures`, { params, ...this.silent })
      .pipe(finalize(() => this._failuresLoading.set(false)))
      .subscribe({
        next: (res) => { this._failures.set(res.data ?? []); this._failuresMeta.set(res.meta ?? null); },
        error: () => this._failuresError.set('Could not load recent failures.'),
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

  // ── S-5 · Step percentiles ──
  private _percentiles = signal<StepPercentile[]>([]);
  private _percentilesLoading = signal(false);
  private _percentilesError = signal<string | null>(null);
  readonly percentiles = this._percentiles.asReadonly();
  readonly percentilesLoading = this._percentilesLoading.asReadonly();
  readonly percentilesError = this._percentilesError.asReadonly();

  loadPercentiles(): void {
    this._percentilesLoading.set(true);
    this._percentilesError.set(null);
    this.http
      .get<ApiResponse<StepPercentile[]>>(`${this.base}/dashboard/step-percentiles`, this.silent)
      .pipe(finalize(() => this._percentilesLoading.set(false)))
      .subscribe({
        next: (res) => this._percentiles.set(res.data ?? []),
        error: () => this._percentilesError.set('Failed to load processing-time percentiles.'),
      });
  }


  // ───────── Dev B · Throughput + Status Distribution ─────────
  readonly throughput = signal<SeriesPoint[]>([]);
  readonly throughputLoading = signal(false);
  readonly throughputError = signal<string | null>(null);
  readonly statusDistribution = signal<SeriesPoint[]>([]);
  readonly distributionLoading = signal(false);
  readonly distributionError = signal<string | null>(null);

  loadThroughput(): void {
    this.throughputLoading.set(true);
    this.throughputError.set(null);
    this.http.get<ApiResponse<ChartSeries>>(`${this.base}/dashboard/throughput`, this.silent)
      .pipe(finalize(() => this.throughputLoading.set(false)))
      .subscribe({
        next: (res) => this.throughput.set(res.data?.points ?? []),
        error: () => this.throughputError.set('Could not load throughput.'),
      });
  }

  loadStatusDistribution(): void {
    this.distributionLoading.set(true);
    this.distributionError.set(null);
    this.http.get<ApiResponse<ChartSeries>>(`${this.base}/dashboard/status-distribution`, this.silent)
      .pipe(finalize(() => this.distributionLoading.set(false)))
      .subscribe({
        next: (res) => this.statusDistribution.set(res.data?.points ?? []),
        error: () => this.distributionError.set('Could not load status distribution.'),
      });
  }

  // ───────── CO-OWNED poll target (FR-1.5) ─────────
  readonly lastUpdated = signal<Date | null>(null);
  refreshAll(): void {
    this.loadSummary();
    this.loadFailures();
    this.loadThroughput();
    this.loadStatusDistribution();
    this.loadPercentiles();
    this.lastUpdated.set(new Date());
  }
}
