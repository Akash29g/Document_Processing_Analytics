import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, firstValueFrom, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { AuthUser, LoginResponse, MeResponse, SiteSummary } from '../models/auth.model';
import { Router } from '@angular/router';

const TOKEN_KEY = 'da_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiBase}/auth`;

  // --- writable signals (private) ---
  private readonly _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private readonly _currentUser = signal<AuthUser | null>(null);
  private readonly _sites = signal<SiteSummary[]>([]);

  // --- public readonly views ---
  readonly token = this._token.asReadonly();
  readonly currentUser = this._currentUser.asReadonly();
  readonly sites = this._sites.asReadonly();
  readonly isAuthenticated = computed(() => !!this._token());

  /** POST /auth/login — stores token + user + sites on success. */
  login(email: string, password: string): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${this.baseUrl}/login`, { email, password })
      .pipe(
        tap((res) => {
          if (res.data) {
            this.setSession(res.data.token, res.data.user, res.data.sites);
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

  logout(): void {
    this._token.set(null);
    this._currentUser.set(null);
    this._sites.set([]);
    localStorage.removeItem(TOKEN_KEY);
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

  private setSession(token: string, user: AuthUser, sites: SiteSummary[]): void {
    this._token.set(token);
    this._currentUser.set(user);
    this._sites.set(sites);
    localStorage.setItem(TOKEN_KEY, token);
  }
}
