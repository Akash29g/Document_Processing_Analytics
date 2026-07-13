import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject, signal, computed } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { AlertRule, AlertRulePayload, Recipient, AlertNotification } from './alerts.models';

@Injectable({ providedIn: 'root' })
export class AlertsService {
  private http = inject(HttpClient);
  private base = `${environment.apiBase}/alerts`;
  private silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  private _rules = signal<AlertRule[]>([]);
  private _loading = signal(false);
  private _error = signal<string | null>(null);
  private _saving = signal(false);

  readonly rules = this._rules.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly saving = this._saving.asReadonly();
  private _recipients = signal<Recipient[]>([]);
  readonly recipients = this._recipients.asReadonly();

  // ── login notifications (fired alerts) ──
  private _notifications = signal<AlertNotification[]>([]);
  readonly notifications = this._notifications.asReadonly();
  readonly unreadCount = computed(() =>
    this._notifications().filter((n) => !n.is_read).length,
  );


  loadRecipients(): void {
    this.http.get<ApiResponse<Recipient[]>>(`${this.base}/recipients`, this.silent)
      .subscribe({ next: (res) => this._recipients.set(res.data ?? []) });
  }


  loadRules(): void {
    this._loading.set(true);
    this._error.set(null);
    this.http.get<ApiResponse<AlertRule[]>>(this.base, this.silent)
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (res) => this._rules.set(res.data ?? []),
        error: () => this._error.set('Could not load alert rules.'),
      });
  }

  create(payload: AlertRulePayload): void {
    this._saving.set(true);
    this.http.post<ApiResponse<AlertRule>>(this.base, payload)
      .pipe(finalize(() => this._saving.set(false)))
      .subscribe({ next: () => this.loadRules() });
  }

  update(id: string, payload: AlertRulePayload): void {
    this._saving.set(true);
    this.http.put<ApiResponse<AlertRule>>(`${this.base}/${id}`, payload)
      .pipe(finalize(() => this._saving.set(false)))
      .subscribe({ next: () => this.loadRules() });
  }

  /** enable/disable toggle = a small update */
  toggle(rule: AlertRule): void {
    this.update(rule.id, {
      name: rule.name,
      threshold_percent: rule.threshold_percent,
      window_minutes: rule.window_minutes,
      email: rule.email,
      cooldown_minutes: rule.cooldown_minutes,
      is_enabled: !rule.is_enabled,
    });
  }

  remove(id: string): void {
    this.http.delete<void>(`${this.base}/${id}`)
      .subscribe({ next: () => this.loadRules() });
  }

  /**
 * Load fired alerts (call on login / when the bell opens).
 * `onDone` lets the shell fire its critical-alert toast burst once loaded.
 */
  loadNotifications(unreadOnly = true, onDone?: () => void): void {
    const url = `${this.base}/notifications${unreadOnly ? '?unread=true' : ''}`;
    this.http.get<ApiResponse<AlertNotification[]>>(url, this.silent).subscribe({
      next: (res) => {
        this._notifications.set(res.data ?? []);
        onDone?.();
      },
    });
  }

  markRead(id: string): void {
    this.http
      .post<ApiResponse<unknown>>(`${this.base}/notifications/${id}/read`, {})
      .subscribe({
        next: () =>
          this._notifications.update((list) =>
            list.map((n) => (n.id === id ? { ...n, is_read: true } : n)),
          ),
      });
  }

  markAllRead(): void {
    this.http
      .post<ApiResponse<unknown>>(`${this.base}/notifications/read-all`, {})
      .subscribe({
        next: () =>
          this._notifications.update((list) =>
            list.map((n) => ({ ...n, is_read: true })),
          ),
      });
  }

  /** Wipe on logout so the next user doesn't inherit the badge. */
  clear(): void {
    this._notifications.set([]);
  }

}
