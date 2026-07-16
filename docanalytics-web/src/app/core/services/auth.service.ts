import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, catchError, finalize, firstValueFrom, map, of, shareReplay, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import {
  AuthUser,
  LoginResponse,
  MeResponse,
  RefreshResponse,
  SiteSummary,
} from '../models/auth.model';
import { Router } from '@angular/router';

const TOKEN_KEY = 'da_token';
const REFRESH_KEY = 'da_refresh'; // NEW (R4)

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiBase}/auth`;

  // --- writable signals (private) ---
  private readonly _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private readonly _refreshToken = signal<string | null>(localStorage.getItem(REFRESH_KEY)); // NEW
  private readonly _currentUser = signal<AuthUser | null>(null);
  private readonly _sites = signal<SiteSummary[]>([]);

  // Single in-flight refresh — prevents concurrent 401s from each rotating the
  // token (which would trip backend reuse-detection and log everyone out).
  private refreshInFlight$: Observable<string | null> | null = null;

  // --- public readonly views ---
  readonly token = this._token.asReadonly();
  readonly currentUser = this._currentUser.asReadonly();
  readonly sites = this._sites.asReadonly();
  readonly isAuthenticated = computed(() => !!this._token());

  /** POST /auth/login — stores token + refresh + user + sites on success. */
  login(email: string, password: string): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${this.baseUrl}/login`, { email, password })
      .pipe(
        tap((res) => {
          if (res.data) {
            this.setSession(res.data.token, res.data.refresh_token, res.data.user, res.data.sites);
          }
        }),
      );
  }

  /** GET /auth/me — rehydrates user + sites (used after a page refresh). */
  loadMe(): Observable<ApiResponse<MeResponse>> {
    return this.http.get<ApiResponse<MeResponse>>(`${this.baseUrl}/me`).pipe(
      tap((res) => {
        if (res.data) {
          this._currentUser.set(res.data.user);
          this._sites.set(res.data.sites);
        }
      }),
    );
  }

  /**
   * POST /auth/refresh — exchanges the stored refresh token for a new access
   * token (+ rotated refresh token). Shared in-flight so concurrent callers
   * reuse the same request. Emits the new access token, or null on failure.
   */
  refreshToken(): Observable<string | null> {
    const rt = this._refreshToken();
    if (!rt) return of(null);
    if (this.refreshInFlight$) return this.refreshInFlight$;

    this.refreshInFlight$ = this.http
      .post<ApiResponse<RefreshResponse>>(`${this.baseUrl}/refresh`, { refresh_token: rt })
      .pipe(
        map((res) => {
          if (res.data) {
            this._token.set(res.data.token);
            this._refreshToken.set(res.data.refresh_token);
            localStorage.setItem(TOKEN_KEY, res.data.token);
            localStorage.setItem(REFRESH_KEY, res.data.refresh_token);
            return res.data.token;
          }
          return null;
        }),
        catchError(() => {
          this.clearSession(); // refresh rejected → local logout (no server call to avoid loop)
          return of(null);
        }),
        finalize(() => {
          this.refreshInFlight$ = null;
        }),
        shareReplay(1),
      );

    return this.refreshInFlight$;
  }

  /**
   * Ensures the in-memory session is populated.
   * On a hard refresh the token survives in localStorage but signals are empty,
   * so we lazily call /auth/me. Returns false if there's no valid session.
   */
  async ensureSession(): Promise<boolean> {
    if (!this._token()) return false;
    if (this._currentUser()) return true;
    try {
      const res = await firstValueFrom(this.loadMe());
      return !!res.data;
    } catch {
      this.logout();
      return false;
    }
  }

  /** Full logout: best-effort server revoke, then clear local state. */
  logout(): void {
    const rt = this._refreshToken();
    if (rt) {
      // fire-and-forget revoke; ignore result (endpoint is AllowAnonymous)
      this.http
        .post(`${this.baseUrl}/logout`, { refresh_token: rt })
        .subscribe({ error: () => {} });
    }
    this.clearSession();
  }

  /** Used by siteAccessGuard (FR-5.3 client-side mirror). */
  hasSiteAccess(siteId: string): boolean {
    return this._sites().some((s) => s.site_id === siteId);
  }

  /** Where does this user land after auth? Developer → provisioning; others → first site. */
  routeAfterLogin(): void {
    const role = this._currentUser()?.role;
    if (role === 'Developer') {
      this.router.navigate(['/provision']);
      return;
    }
    const first = this._sites()[0];
    this.router.navigate(first ? ['/site', first.site_id] : ['/login']);
  }

  private setSession(
    token: string,
    refreshToken: string,
    user: AuthUser,
    sites: SiteSummary[],
  ): void {
    this._token.set(token);
    this._refreshToken.set(refreshToken);
    this._currentUser.set(user);
    this._sites.set(sites);
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(REFRESH_KEY, refreshToken);
  }

  private clearSession(): void {
    this._token.set(null);
    this._refreshToken.set(null);
    this._currentUser.set(null);
    this._sites.set([]);
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_KEY);
  }
}
