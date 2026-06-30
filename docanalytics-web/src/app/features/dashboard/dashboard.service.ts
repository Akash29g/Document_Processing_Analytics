import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { SeriesPoint, ChartSeries } from '../../core/models/dashboard.model';
import { finalize } from 'rxjs';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);
  private base = environment.apiBase; // '/api/v1'

  // 👈 shared option: tells the error interceptor "don't toast — I show errors inline"
  private readonly silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };
  
  // Akash's summary + recentFailures signals live here (keep on merge).

  // Shubh's charts half
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
    .pipe(finalize(() => this.throughputLoading.set(false)))   // runs on success AND error
    .subscribe({
      next: res => this.throughput.set(res.data?.points ?? []),
      error: () => this.throughputError.set('Could not load throughput.'),
    });
}


  
  loadStatusDistribution(): void {
  this.distributionLoading.set(true);
  this.distributionError.set(null);
  this.http.get<ApiResponse<ChartSeries>>(`${this.base}/dashboard/status-distribution`, this.silent)
    .pipe(finalize(() => this.distributionLoading.set(false)))  // runs on success AND error
    .subscribe({
      next: res => this.statusDistribution.set(res.data?.points ?? []),
      error: () => this.distributionError.set('Could not load status distribution.'),
    });
}

  

  // Shared — the 30s poll calls this
  readonly lastUpdated = signal<Date | null>(null);
  refreshAll(): void {
    this.loadThroughput();
    this.loadStatusDistribution();
    // Akash adds: this.loadSummary(); this.loadRecentFailures();
    this.lastUpdated.set(new Date());
  }
}
