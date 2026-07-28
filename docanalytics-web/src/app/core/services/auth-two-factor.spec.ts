import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

describe('AuthService — 2FA & session extras', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiBase}/auth`;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('login() does NOT set the session when the response is a 2FA challenge', () => {
    service.login('user@test.com', 'Password123!').subscribe();

    httpMock.expectOne(`${baseUrl}/login`).flush({
      data: { requires_two_factor: true, challenge_token: 'chal-123' },
      error: null,
    });

    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBeNull();
  });

  it('login() sets the session on a full (non-2FA) login', () => {
    service.login('user@test.com', 'Password123!').subscribe();

    httpMock.expectOne(`${baseUrl}/login`).flush({
      data: {
        token: 'jwt-abc',
        user: { id: 'u1', email: 'user@test.com', role: 'Admin' },
        sites: [{ site_id: 's1', site_name: 'Site One' }],
        must_change_password: false,
      },
      error: null,
    });

    expect(service.isAuthenticated()).toBe(true);
    expect(service.token()).toBe('jwt-abc');
  });

  it('loginWithTwoFactor() posts the challenge token + code and sets the session on success', () => {
    service.loginWithTwoFactor('chal-123', '123456').subscribe();

    const req = httpMock.expectOne(`${baseUrl}/login/2fa`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ challenge_token: 'chal-123', code: '123456' });

    req.flush({
      data: {
        token: 'jwt-xyz',
        user: { id: 'u1', email: 'user@test.com', role: 'Admin' },
        sites: [],
        must_change_password: false,
      },
      error: null,
    });

    expect(service.isAuthenticated()).toBe(true);
    expect(service.token()).toBe('jwt-xyz');
  });

  it('loginWithTwoFactor() does not set the session when the server returns no data', () => {
    service.loginWithTwoFactor('chal-123', '000000').subscribe();

    httpMock
      .expectOne(`${baseUrl}/login/2fa`)
      .flush({ data: null, error: 'Invalid or expired code.' });

    expect(service.isAuthenticated()).toBe(false);
  });

  it('setupTwoFactor() posts to /2fa/setup', () => {
    service.setupTwoFactor().subscribe();

    const req = httpMock.expectOne(`${baseUrl}/2fa/setup`);
    expect(req.request.method).toBe('POST');
    req.flush({
      data: { secret: 's', otp_auth_uri: 'otpauth://...', manual_key: 'S ECR ET' },
      error: null,
    });
  });

  it('confirmTwoFactor() posts the code to /2fa/confirm', () => {
    service.confirmTwoFactor('123456').subscribe();

    const req = httpMock.expectOne(`${baseUrl}/2fa/confirm`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ code: '123456' });
    req.flush({ data: { recovery_codes: ['AAAA-1111'] }, error: null });
  });

  it('disableTwoFactor() posts the password to /2fa/disable', () => {
    service.disableTwoFactor('Password123!').subscribe();

    const req = httpMock.expectOne(`${baseUrl}/2fa/disable`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ password: 'Password123!' });
    req.flush({ data: { disabled: true }, error: null });
  });

  it('forgotPassword() posts the email', () => {
    service.forgotPassword('user@test.com').subscribe();

    const req = httpMock.expectOne(`${baseUrl}/forgot-password`);
    expect(req.request.body).toEqual({ email: 'user@test.com' });
    req.flush({ data: { message: 'ok' }, error: null });
  });

  it('resetPassword() posts token + new_password', () => {
    service.resetPassword('reset-tok', 'NewPassword123!').subscribe();

    const req = httpMock.expectOne(`${baseUrl}/reset-password`);
    expect(req.request.body).toEqual({ token: 'reset-tok', new_password: 'NewPassword123!' });
    req.flush({ data: { reset: true }, error: null });
  });

  it('hasSiteAccess() reflects the sites returned by a full login', () => {
    service.login('user@test.com', 'Password123!').subscribe();
    httpMock.expectOne(`${baseUrl}/login`).flush({
      data: {
        token: 'jwt-abc',
        user: { id: 'u1', email: 'user@test.com', role: 'Admin' },
        sites: [{ site_id: 's1', site_name: 'Site One' }],
        must_change_password: false,
      },
      error: null,
    });

    expect(service.hasSiteAccess('s1')).toBe(true);
    expect(service.hasSiteAccess('unknown-site')).toBe(false);
  });

  it('logout() clears the session even if the server call fails', () => {
    service.login('user@test.com', 'Password123!').subscribe();
    httpMock.expectOne(`${baseUrl}/login`).flush({
      data: {
        token: 'jwt-abc',
        user: { id: 'u1', email: 'user@test.com', role: 'Admin' },
        sites: [],
        must_change_password: false,
      },
      error: null,
    });
    expect(service.isAuthenticated()).toBe(true);

    service.logout();
    httpMock
      .expectOne(`${baseUrl}/logout`)
      .flush(null, { status: 500, statusText: 'Server Error' });

    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBeNull();
  });
});
