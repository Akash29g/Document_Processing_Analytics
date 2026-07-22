import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  const base = environment.apiBase;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  const loginPayload = {
    data: {
      token: 'jwt-123',
      // server still sends refresh via Set-Cookie (HttpOnly); body value is ignored by the client
      user: { id: 'u1', email: 'a@org.com', role: 'Viewer' },
      sites: [{ site_id: 's1', site_name: 'Plant One' }],
    },
    error: null,
  };

  /** helper: log in and flush so the session is populated */
  const doLogin = () => {
    service.login('a@org.com', 'pw').subscribe();
    httpMock.expectOne((r) => r.url === `${base}/auth/login`).flush(loginPayload);
  };

  it('login() stores access token + sites on success', () => {
    doLogin();
    expect(localStorage.getItem('da_token')).toBe('jwt-123');
    expect(service.sites().length).toBe(1);
  });

  it('logout() revokes server-side (via cookie) and clears the session', () => {
    doLogin();
    service.logout();
    const req = httpMock.expectOne((r) => r.url === `${base}/auth/logout`);
    expect(req.request.body).toEqual({});                 // token is in the cookie, not the body
    expect(req.request.withCredentials).toBe(true);       // cookie must ride along
    req.flush({ data: { logged_out: true }, error: null });
    expect(localStorage.getItem('da_token')).toBeNull();
  });

  it('hasSiteAccess() reflects the sites list', () => {
    doLogin();
    expect(service.hasSiteAccess('s1')).toBe(true);
    expect(service.hasSiteAccess('s999')).toBe(false);
  });

  it('refreshToken() stores the rotated access token and emits it', () => {
    doLogin();
    let emitted: string | null = 'unset';
    service.refreshToken().subscribe((t) => (emitted = t));

    const req = httpMock.expectOne((r) => r.url === `${base}/auth/refresh`);
    expect(req.request.body).toEqual({});                 // refresh token comes from the cookie
    expect(req.request.withCredentials).toBe(true);
    req.flush({ data: { token: 'jwt-456' }, error: null });

    expect(emitted).toBe('jwt-456');
    expect(localStorage.getItem('da_token')).toBe('jwt-456');
  });

  it('refreshToken() clears the session and emits null on failure', () => {
    doLogin();
    let emitted: string | null = 'unset';
    service.refreshToken().subscribe((t) => (emitted = t));

    httpMock
      .expectOne((r) => r.url === `${base}/auth/refresh`)
      .flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(emitted).toBeNull();
    expect(localStorage.getItem('da_token')).toBeNull();
  });

  it('refreshToken() is single-flight — concurrent calls make one request', () => {
    doLogin();
    let a: string | null = 'unset';
    let b: string | null = 'unset';

    service.refreshToken().subscribe((t) => (a = t));
    service.refreshToken().subscribe((t) => (b = t));

    // expectOne asserts EXACTLY one matching request — proves the in-flight share
    const req = httpMock.expectOne((r) => r.url === `${base}/auth/refresh`);
    req.flush({ data: { token: 'jwt-789' }, error: null });

    expect(a).toBe('jwt-789');
    expect(b).toBe('jwt-789');
  });

  it('ensureSession() returns false when there is no token', async () => {
    const ok = await service.ensureSession();
    expect(ok).toBe(false);
  });

  it('ensureSession() rehydrates via /auth/me when a token exists', async () => {
    localStorage.setItem('da_token', 'jwt-123');
    // rebuild so the constructor picks up the token from storage
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);

    const p = service.ensureSession();
    httpMock
      .expectOne((r) => r.url === `${base}/auth/me`)
      .flush({
        data: {
          user: { id: 'u1', email: 'a@org.com', role: 'Viewer' },
          sites: [{ site_id: 's1', site_name: 'Plant One' }],
        },
        error: null,
      });

    expect(await p).toBe(true);
    expect(service.sites().length).toBe(1);
  });
});
