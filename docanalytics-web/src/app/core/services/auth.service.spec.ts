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
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  afterEach(() => httpMock.verify());

  const loginPayload = {
    data: {
      token: 'jwt-123',
      user: { id: 'u1', email: 'a@org.com', role: 'Viewer' },
      sites: [{ site_id: 's1', site_name: 'Plant One' }]
    },
    error: null,
  };

  it('login() stores token + sites on success', () => {
    service.login('a@org.com', 'pw').subscribe();
    httpMock.expectOne(r => r.url === `${base}/auth/login`).flush(loginPayload);
    expect(localStorage.getItem('da_token')).toBe('jwt-123');
    expect(service.sites().length).toBe(1);
  });

  it('logout() clears the token', () => {
    service.login('a@org.com', 'pw').subscribe();
    httpMock.expectOne(r => r.url === `${base}/auth/login`).flush(loginPayload);
    service.logout();
    expect(localStorage.getItem('da_token')).toBeNull();
  });

  it('hasSiteAccess() reflects the sites list', () => {
    service.login('a@org.com', 'pw').subscribe();
    httpMock.expectOne(r => r.url === `${base}/auth/login`).flush(loginPayload);
    expect(service.hasSiteAccess('s1')).toBe(true);
    expect(service.hasSiteAccess('s999')).toBe(false);
  });

  it('ensureSession() returns false when there is no token', async () => {
    const ok = await service.ensureSession();
    expect(ok).toBe(false);
  });

  it('ensureSession() rehydrates via /auth/me when a token exists', async () => {
    localStorage.setItem('da_token', 'jwt-123');
    // rebuild so the constructor picks up the token from storage
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);

    const p = service.ensureSession();
    httpMock.expectOne(r => r.url === `${base}/auth/me`)
      .flush({
        data: {
          user: { id: 'u1', email: 'a@org.com', role: 'Viewer' },
          sites: [{ site_id: 's1', site_name: 'Plant One' }]
        }, error: null
      });
    expect(await p).toBe(true);
    expect(service.sites().length).toBe(1);
  });

});
