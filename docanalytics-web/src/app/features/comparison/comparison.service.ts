import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, WritableSignal, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { ChartSeries, SeriesPoint } from '../../core/models/dashboard.model';

export interface RangeState {
  from: string;
  to: string;
  points: SeriesPoint[];
  loading: boolean;
  error: string | null;
}

const blank = (): RangeState => ({ from: '', to: '', points: [], loading: false, error: null });

@Injectable({ providedIn: 'root' })
export class ComparisonService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiBase}/dashboard/throughput`;

  // two independent range slots (A and B)
  readonly rangeA = signal<RangeState>(blank());
  readonly rangeB = signal<RangeState>(blank());

  loadA(from: string, to: string) { return this.load(this.rangeA, from, to); }
  loadB(from: string, to: string) { return this.load(this.rangeB, from, to); }

  private async load(slot: WritableSignal<RangeState>, from: string, to: string): Promise<void> {
    slot.update(s => ({ ...s, from, to, loading: true, error: null }));
    try {
      let params = new HttpParams();
      if (from) params = params.set('from', from);   // ISO date (yyyy-MM-dd)
      if (to) params = params.set('to', to);
      const res = await firstValueFrom(
        this.http.get<ApiResponse<ChartSeries>>(this.url, { params }));
      slot.update(s => ({ ...s, points: res.data?.points ?? [], loading: false }));
    } catch {
      slot.update(s => ({ ...s, loading: false, error: 'Could not load this range. Retry.' }));
    }
  }

  /** sum of completed files across a range's points */
  total(points: SeriesPoint[]): number {
    return points.reduce((sum, p) => sum + p.value, 0);
  }
}
