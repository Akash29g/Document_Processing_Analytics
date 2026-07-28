import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

describe('AuthService — extra coverage (forgotPassword, resetPassword, ensureSession, routeAfterLogin, refreshToken)', () => {
  let httpMock: HttpTestingController;
  let routerMock: { navigate: ReturnType<typeof vi.fn> };
  const base = `${environment.apiBase}/auth`;

  function setup() {
    routerMock = { navigate: vi.fn() };
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: Router, useValue: routerMock },
      ],
    });
    httpMock = TestBed.inject(HttpTestingController);
    return TestBed.inject(AuthService);
  }

  beforeEach(() => {
    localStorage.clear();
  });

  afterEach(() => {
    httpMock?.verify();
  });

  it('forgotPassword() posts the email and returns the response', () => {
    const service = setup();
    let result: any;
    service.forgotPassword('a@b.com').subscribe((res) => (result = res));

    const req = httpMock.expectOne(`${base}/forgot-password`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'a@b.com' });
    req.flush({ data: { message: 'ok' }, error: null });

    expect(result.data.message).toBe('ok');
  });

  it('resetPassword() posts the token and new password', () => {
    const service = setup();
    let result: any;
    service.resetPassword('tok123', 'NewPass!1').subscribe((res) => (result = res));

    const req = httpMock.expectOne(`${base}/reset-password`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ token: 'tok123', new_password: 'NewPass!1' });
    req.flush({ data: { reset: true }, error: null });

    expect(result.data.reset).toBe(true);
  });

  describe('ensureSession()', () => {
    it('returns false immediately when there is no token', async () => {
      const service = setup();
      const result = await service.ensureSession();

      expect(result).toBe(false);
      httpMock.expectNone(`${base}/me`);
    });

    it('returns true without a network call when the user is already loaded', async () => {
      localStorage.setItem('da_token', 'existing-token');
      const service = setup();

      service.loadMe().subscribe();
      httpMock.expectOne(`${base}/me`).flush({
        data: { user: { id: 'u1', email: 'a@b.com', role: 'Viewer' }, sites: [] },
        error: null,
      });

      const result = await service.ensureSession();
      expect(result).toBe(true);
      httpMock.expectNone(`${base}/me`);
    });

    it('calls /me when a token exists but the user is not loaded, and returns true on success', async () => {
      localStorage.setItem('da_token', 'existing-token');
      const service = setup();

      const promise = service.ensureSession();
      httpMock.expectOne(`${base}/me`).flush({
        data: { user: { id: 'u1', email: 'a@b.com', role: 'Viewer' }, sites: [] },
        error: null,
      });

      expect(await promise).toBe(true);
    });

    it('logs out and returns false when /me fails', async () => {
      localStorage.setItem('da_token', 'stale-token');
      const service = setup();

      const promise = service.ensureSession();
      httpMock.expectOne(`${base}/me`).flush('error', { status: 500, statusText: 'Server Error' });

      // let the promise rejection's .catch() -> logout() microtask run
      await Promise.resolve();
      await Promise.resolve();

      httpMock.expectOne(`${base}/logout`).flush({ data: null, error: null });

      expect(await promise).toBe(false);
      expect(localStorage.getItem('da_token')).toBeNull();
    });
  });

  describe('routeAfterLogin()', () => {
    async function loadUser(role: string, sites: { site_id: string }[]) {
      const service = setup();
      service.loadMe().subscribe();
      httpMock.expectOne(`${base}/me`).flush({
        data: { user: { id: 'u1', email: 'a@b.com', role }, sites },
        error: null,
      });
      return service;
    }

    it('navigates to /provision for a Developer', async () => {
      const service = await loadUser('Developer', []);
      service.routeAfterLogin();
      expect(routerMock.navigate).toHaveBeenCalledWith(['/provision']);
    });

    it('navigates to the first site for a non-developer with sites', async () => {
      const service = await loadUser('Viewer', [{ site_id: 'site-1' } as any]);
      service.routeAfterLogin();
      expect(routerMock.navigate).toHaveBeenCalledWith(['/site', 'site-1']);
    });

    it('navigates to /login when there are no sites', async () => {
      const service = await loadUser('Viewer', []);
      service.routeAfterLogin();
      expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
    });
  });

  describe('refreshToken()', () => {
    it('sets the token and persists it to localStorage on success', async () => {
      const service = setup();
      const promise = new Promise<string | null>((resolve) => {
        service.refreshToken().subscribe(resolve);
      });

      httpMock.expectOne(`${base}/refresh`).flush({ data: { token: 'new-token' }, error: null });

      expect(await promise).toBe('new-token');
      expect(localStorage.getItem('da_token')).toBe('new-token');
      expect(service.token()).toBe('new-token');
    });

    it('clears the session and resolves null when the refresh is rejected', async () => {
      localStorage.setItem('da_token', 'stale-token');
      const service = setup();

      const promise = new Promise<string | null>((resolve) => {
        service.refreshToken().subscribe(resolve);
      });

      httpMock
        .expectOne(`${base}/refresh`)
        .flush('nope', { status: 401, statusText: 'Unauthorized' });

      expect(await promise).toBeNull();
      expect(localStorage.getItem('da_token')).toBeNull();
    });

    it('shares a single in-flight request across concurrent callers', () => {
      const service = setup();

      let firstResult: string | null | undefined;
      let secondResult: string | null | undefined;
      service.refreshToken().subscribe((v) => (firstResult = v));
      service.refreshToken().subscribe((v) => (secondResult = v));

      // Only ONE actual HTTP request should have gone out for both callers.
      const req = httpMock.expectOne(`${base}/refresh`);
      req.flush({ data: { token: 'shared-token' }, error: null });

      expect(firstResult).toBe('shared-token');
      expect(secondResult).toBe('shared-token');
    });

    it('resolves null (without clearing the session) when the refresh response has no data', async () => {
      const service = setup();
      const promise = new Promise<string | null>((resolve) => {
        service.refreshToken().subscribe(resolve);
      });

      httpMock.expectOne(`${base}/refresh`).flush({ data: null, error: 'unexpected' });

      expect(await promise).toBeNull();
    });
  });
});
