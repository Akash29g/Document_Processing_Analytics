import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { AlertsService } from './alerts.service';

describe('AlertsService — notifications', () => {
  let service: AlertsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(AlertsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());   // no leftover/unexpected requests

  it('loads unread notifications and computes badge count', async () => {
    service.loadNotifications(true);
    const req = httpMock.expectOne(
      `${environment.apiBase}/alerts/notifications?unread=true`,
    );
    req.flush({
      data: [
        { id: '1', is_read: false, severity: 'critical', rule_name: 'R', message: 'm' },
        { id: '2', is_read: false, severity: 'warning', rule_name: 'R', message: 'm' },
      ],
      error: null,
    });
    expect(service.unreadCount()).toBe(2);
  });

  it('markRead optimistically flips the flag', () => {
    // seed two unread first
    service.loadNotifications(true);
    httpMock
      .expectOne(`${environment.apiBase}/alerts/notifications?unread=true`)
      .flush({
        data: [
          { id: '1', is_read: false, severity: 'critical', rule_name: 'R', message: 'm' },
          { id: '2', is_read: false, severity: 'warning', rule_name: 'R', message: 'm' },
        ],
        error: null,
      });
    expect(service.unreadCount()).toBe(2);

    service.markRead('1');
    httpMock
      .expectOne(`${environment.apiBase}/alerts/notifications/1/read`)
      .flush({ data: {}, error: null });

    expect(service.unreadCount()).toBe(1);
  });

  it('clear() empties the store on logout', () => {
    service.clear();
    expect(service.notifications().length).toBe(0);
  });
});
