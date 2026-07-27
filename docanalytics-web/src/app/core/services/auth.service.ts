import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, catchError, finalize, firstValueFrom, map, of, shareReplay, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import {
  AuthUser,
  LoginResponse,
  LoginOrChallenge,
  MeResponse,
  RefreshResponse,
  SiteSummary,
  TwoFactorSetupResponse,
  TwoFactorConfirmResponse,
} from '../models/auth.model';
import { Router } from '@angular/router';

const TOKEN_KEY = 'da_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiBase}/auth`;

  private readonly _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private readonly _currentUser = signal<AuthUser | null>(null);
  private readonly _sites = signal<SiteSummary[]>([]);

  // Single in-flight refresh — prevents concurrent 401s from each rotating the
  // cookie (which would trip backend reuse-detection and log everyone out).
  private refreshInFlight$: Observable<string | null> | null = null;

  readonly token = this._token.asReadonly();
  readonly currentUser = this._currentUser.asReadonly();
  readonly sites = this._sites.asReadonly();
  readonly isAuthenticated = computed(() => !!this._token());

  /** POST /auth/login — stores access token + user + sites. The refresh token is set by the server as an HttpOnly cookie. */
  login(email: string, password: string): Observable<ApiResponse<LoginOrChallenge>> {
    return this.http
      .post<ApiResponse<LoginOrChallenge>>(
        `${this.baseUrl}/login`,
        { email, password },
        { withCredentials: true }, // needed to receive the Set-Cookie
      )
      .pipe(
        tap((res) => {
          const data = res.data;
          // Only set the session on a FULL login — not on the requires_two_factor branch.
          if (data && !('requires_two_factor' in data)) {
            this.setSession(data.token, data.user, data.sites);
          }
        }),
      );
  }

  /** POST /auth/login/2fa — completes a 2FA-gated login using the challenge token + a 6-digit (or recovery) code. */
  loginWithTwoFactor(challengeToken: string, code: string): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(
        `${this.baseUrl}/login/2fa`,
        { challenge_token: challengeToken, code },
        { withCredentials: true },
      )
      .pipe(
        tap((res) => {
          if (res.data) this.setSession(res.data.token, res.data.user, res.data.sites);
        }),
      );
  }

  /** POST /auth/2fa/setup — returns the secret + otpauth URI for client-side QR rendering. */
  setupTwoFactor(): Observable<ApiResponse<TwoFactorSetupResponse>> {
    return this.http.post<ApiResponse<TwoFactorSetupResponse>>(`${this.baseUrl}/2fa/setup`, {});
  }

  /** POST /auth/2fa/confirm — verifies the first code, enables 2FA, returns one-time recovery codes. */
  confirmTwoFactor(code: string): Observable<ApiResponse<TwoFactorConfirmResponse>> {
    return this.http.post<ApiResponse<TwoFactorConfirmResponse>>(`${this.baseUrl}/2fa/confirm`, {
      code,
    });
  }

  /** POST /auth/2fa/disable — re-verifies password, clears 2FA. */
  disableTwoFactor(password: string): Observable<ApiResponse<{ disabled: boolean }>> {
    return this.http.post<ApiResponse<{ disabled: boolean }>>(`${this.baseUrl}/2fa/disable`, {
      password,
    });
  }

  /** POST /auth/forgot-password — always resolves 200 (generic message, no enumeration). */
  forgotPassword(email: string): Observable<ApiResponse<{ message: string }>> {
    return this.http.post<ApiResponse<{ message: string }>>(`${this.baseUrl}/forgot-password`, {
      email,
    });
  }

  /** POST /auth/reset-password — consumes the emailed token and sets a new password. */
  resetPassword(token: string, newPassword: string): Observable<ApiResponse<{ reset: boolean }>> {
    return this.http.post<ApiResponse<{ reset: boolean }>>(`${this.baseUrl}/reset-password`, {
      token,
      new_password: newPassword,
    });
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
   * POST /auth/refresh — the refresh token rides along automatically in the
   * HttpOnly cookie (withCredentials). Server rotates the cookie and returns a
   * new access token. Shared in-flight so concurrent callers reuse one request.
   */
  refreshToken(): Observable<string | null> {
    if (this.refreshInFlight$) return this.refreshInFlight$;

    this.refreshInFlight$ = this.http
      .post<ApiResponse<RefreshResponse>>(`${this.baseUrl}/refresh`, {}, { withCredentials: true })
      .pipe(
        map((res) => {
          if (res.data) {
            this._token.set(res.data.token);
            localStorage.setItem(TOKEN_KEY, res.data.token);
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
   * Ensures the in-memory session is populated. On a hard refresh the token
   * survives in localStorage but signals are empty, so we lazily call /auth/me.
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

  /** Full logout: best-effort server revoke (cookie sent automatically), then clear local state. */
  logout(): void {
    this.http
      .post(`${this.baseUrl}/logout`, {}, { withCredentials: true })
      .subscribe({ error: () => {} });
    this.clearSession();
  }

  hasSiteAccess(siteId: string): boolean {
    return this._sites().some((s) => s.site_id === siteId);
  }

  routeAfterLogin(): void {
    const role = this._currentUser()?.role;
    if (role === 'Developer') {
      this.router.navigate(['/provision']);
      return;
    }
    const first = this._sites()[0];
    this.router.navigate(first ? ['/site', first.site_id] : ['/login']);
  }

  private setSession(token: string, user: AuthUser, sites: SiteSummary[]): void {
    this._token.set(token);
    this._currentUser.set(user);
    this._sites.set(sites);
    localStorage.setItem(TOKEN_KEY, token);
  }

  private clearSession(): void {
    this._token.set(null);
    this._currentUser.set(null);
    this._sites.set([]);
    localStorage.removeItem(TOKEN_KEY);
  }
}
