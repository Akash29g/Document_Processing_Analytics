import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { SessionsService } from './sessions.service';
import { environment } from '../../../environments/environment';
import { SessionSummary } from '../../core/models/auth.model';

describe('SessionsService', () => {
  let service: SessionsService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiBase}/auth/sessions`;

  const session1: SessionSummary = {
    id: 's1',
    device_label: 'Chrome on Windows',
    ip_address: '1.2.3.4',
    created_at: '2026-01-01T00:00:00Z',
    last_used_at: '2026-01-02T00:00:00Z',
    is_current: true,
  } as SessionSummary;

  const session2: SessionSummary = {
    id: 's2',
    device_label: 'Firefox on Mac',
    ip_address: '5.6.7.8',
    created_at: '2026-01-01T00:00:00Z',
    last_used_at: null,
    is_current: false,
  } as SessionSummary;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(SessionsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('load() populates sessions on success', () => {
    service.load();
    expect(service.loading()).toBe(true);

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [session1, session2], error: null });

    expect(service.loading()).toBe(false);
    expect(service.sessions()).toEqual([session1, session2]);
    expect(service.error()).toBeNull();
  });

  it('load() sets an error message on failure', () => {
    service.load();
    httpMock
      .expectOne(baseUrl)
      .flush({ message: 'boom' }, { status: 500, statusText: 'Server Error' });

    expect(service.loading()).toBe(false);
    expect(service.error()).toBe('Could not load active sessions.');
  });

  it('load() clears any previous error before firing a new request', () => {
    service.load();
    httpMock
      .expectOne(baseUrl)
      .flush({ message: 'boom' }, { status: 500, statusText: 'Server Error' });
    expect(service.error()).not.toBeNull();

    service.load();
    expect(service.error()).toBeNull();
    httpMock.expectOne(baseUrl).flush({ data: [], error: null });
  });

  it('revoke() removes the matching session from the local list on success', () => {
    service.sessions.set([session1, session2]);

    service.revoke('s2');

    const req = httpMock.expectOne(`${baseUrl}/s2`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ data: { revoked: true }, error: null });

    expect(service.sessions()).toEqual([session1]);
  });

  it('revoke() sets an error and leaves the list untouched on failure', () => {
    service.sessions.set([session1, session2]);

    service.revoke('s2');
    httpMock
      .expectOne(`${baseUrl}/s2`)
      .flush({ message: 'boom' }, { status: 500, statusText: 'Server Error' });

    expect(service.sessions()).toEqual([session1, session2]);
    expect(service.error()).toBe('Could not revoke that session.');
  });

  it('revokeAllOthers() keeps only the current session on success', () => {
    service.sessions.set([session1, session2]);

    service.revokeAllOthers();

    const req = httpMock.expectOne(`${baseUrl}/revoke-others`);
    expect(req.request.method).toBe('POST');
    req.flush({ data: { revoked_count: 1 }, error: null });

    expect(service.sessions()).toEqual([session1]);
  });

  it('revokeAllOthers() sets an error on failure, list untouched', () => {
    service.sessions.set([session1, session2]);

    service.revokeAllOthers();
    httpMock
      .expectOne(`${baseUrl}/revoke-others`)
      .flush({ message: 'boom' }, { status: 500, statusText: 'Server Error' });

    expect(service.error()).toBe('Could not log out other devices.');
    expect(service.sessions()).toEqual([session1, session2]);
  });
});
