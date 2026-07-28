import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { SessionSummary } from '../../core/models/auth.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';

@Injectable({ providedIn: 'root' })
export class SessionsService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiBase}/auth/sessions`;

  readonly sessions = signal<SessionSummary[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.http
      .get<ApiResponse<SessionSummary[]>>(this.baseUrl, {
        context: new HttpContext().set(SKIP_ERROR_TOAST, true),
      })
      .subscribe({
        next: (res) => {
          this.loading.set(false);
          this.sessions.set(res.data ?? []);
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Could not load active sessions.');
        },
      });
  }

  revoke(id: string): void {
    this.http
      .delete<ApiResponse<{ revoked: boolean }>>(`${this.baseUrl}/${id}`, {
        context: new HttpContext().set(SKIP_ERROR_TOAST, true),
      })
      .subscribe({
        next: () => this.sessions.update((list) => list.filter((s) => s.id !== id)),
        error: () => this.error.set('Could not revoke that session.'),
      });
  }

  revokeAllOthers(): void {
    this.http
      .post<ApiResponse<{ revoked_count: number }>>(
        `${this.baseUrl}/revoke-others`,
        {},
        { context: new HttpContext().set(SKIP_ERROR_TOAST, true) },
      )
      .subscribe({
        next: () => this.sessions.update((list) => list.filter((s) => s.is_current)),
        error: () => this.error.set('Could not log out other devices.'),
      });
  }
}
