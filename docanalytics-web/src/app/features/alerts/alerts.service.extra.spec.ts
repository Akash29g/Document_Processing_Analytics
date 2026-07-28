import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AlertsService } from './alerts.service';
import { environment } from '../../../environments/environment';

describe('AlertsService — extra coverage', () => {
  let httpMock: HttpTestingController;
  const base = `${environment.apiBase}/alerts`;

  function setup() {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    httpMock = TestBed.inject(HttpTestingController);
    return TestBed.inject(AlertsService);
  }

  afterEach(() => {
    httpMock?.verify();
  });

  const rule: any = {
    id: 'r1',
    name: 'High volume',
    threshold_percent: 10,
    window_minutes: 30,
    email: 'a@b.com',
    cooldown_minutes: 60,
    is_enabled: true,
  };

  const payload: any = {
    name: 'High volume',
    threshold_percent: 10,
    window_minutes: 30,
    email: 'a@b.com',
    cooldown_minutes: 60,
    is_enabled: true,
  };

  it('loadRecipients() populates recipients from the response', () => {
    const service = setup();
    service.loadRecipients();

    const req = httpMock.expectOne(`${base}/recipients`);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [{ email: 'a@b.com' }], error: null });

    expect(service.recipients()).toEqual([{ email: 'a@b.com' }]);
  });

  it('loadRecipients() defaults to an empty list when data is missing', () => {
    const service = setup();
    service.loadRecipients();

    httpMock.expectOne(`${base}/recipients`).flush({ data: null, error: null });

    expect(service.recipients()).toEqual([]);
  });

  describe('loadRules()', () => {
    it('sets rules and toggles loading on success', () => {
      const service = setup();
      service.loadRules();
      expect(service.loading()).toBe(true);

      httpMock.expectOne(base).flush({ data: [rule], error: null });

      expect(service.loading()).toBe(false);
      expect(service.rules()).toEqual([rule]);
      expect(service.error()).toBeNull();
    });

    it('sets an error and stops loading on failure', () => {
      const service = setup();
      service.loadRules();

      httpMock.expectOne(base).flush('nope', { status: 500, statusText: 'Server Error' });

      expect(service.loading()).toBe(false);
      expect(service.error()).toBe('Could not load alert rules.');
    });
  });

  it('create() posts the payload, toggles saving, and reloads the rules list', () => {
    const service = setup();
    service.create(payload);
    expect(service.saving()).toBe(true);

    const postReq = httpMock.expectOne(base);
    expect(postReq.request.method).toBe('POST');
    expect(postReq.request.body).toEqual(payload);
    postReq.flush({ data: rule, error: null });

    expect(service.saving()).toBe(false);

    httpMock.expectOne(base).flush({ data: [rule], error: null });
    expect(service.rules()).toEqual([rule]);
  });

  it('update() puts the payload, toggles saving, and reloads the rules list', () => {
    const service = setup();
    service.update('r1', payload);
    expect(service.saving()).toBe(true);

    const putReq = httpMock.expectOne(`${base}/r1`);
    expect(putReq.request.method).toBe('PUT');
    expect(putReq.request.body).toEqual(payload);
    putReq.flush({ data: rule, error: null });

    expect(service.saving()).toBe(false);
    httpMock.expectOne(base).flush({ data: [rule], error: null });
  });

  it('toggle() calls update() with is_enabled flipped and the same other fields', () => {
    const service = setup();
    service.toggle(rule);

    const putReq = httpMock.expectOne(`${base}/${rule.id}`);
    expect(putReq.request.method).toBe('PUT');
    expect(putReq.request.body).toEqual({
      name: rule.name,
      threshold_percent: rule.threshold_percent,
      window_minutes: rule.window_minutes,
      email: rule.email,
      cooldown_minutes: rule.cooldown_minutes,
      is_enabled: !rule.is_enabled,
    });
    putReq.flush({ data: rule, error: null });

    httpMock.expectOne(base).flush({ data: [rule], error: null });
  });

  it('remove() deletes the rule and reloads the rules list', () => {
    const service = setup();
    service.remove('r1');

    const delReq = httpMock.expectOne(`${base}/r1`);
    expect(delReq.request.method).toBe('DELETE');
    delReq.flush(null);

    httpMock.expectOne(base).flush({ data: [], error: null });
    expect(service.rules()).toEqual([]);
  });

  describe('loadNotifications()', () => {
    it('defaults to unread-only, sets notifications, and invokes onDone', () => {
      const service = setup();
      const onDone = vi.fn();
      service.loadNotifications(true, onDone);

      const req = httpMock.expectOne(`${base}/notifications?unread=true`);
      req.flush({ data: [{ id: 'n1', is_read: false }], error: null });

      expect(service.notifications()).toEqual([{ id: 'n1', is_read: false }]);
      expect(onDone).toHaveBeenCalledTimes(1);
    });

    it('fetches all notifications when unreadOnly is false', () => {
      const service = setup();
      service.loadNotifications(false);

      httpMock.expectOne(`${base}/notifications`).flush({ data: [], error: null });
    });
  });

  it('markRead() marks only the matching notification as read', () => {
    const service = setup();
    service.loadNotifications(true);
    httpMock.expectOne(`${base}/notifications?unread=true`).flush({
      data: [
        { id: 'n1', is_read: false },
        { id: 'n2', is_read: false },
      ],
      error: null,
    });

    service.markRead('n1');
    httpMock.expectOne(`${base}/notifications/n1/read`).flush({ data: null, error: null });

    expect(service.notifications()).toEqual([
      { id: 'n1', is_read: true },
      { id: 'n2', is_read: false },
    ]);
    expect(service.unreadCount()).toBe(1);
  });

  it('markAllRead() marks every notification as read', () => {
    const service = setup();
    service.loadNotifications(true);
    httpMock.expectOne(`${base}/notifications?unread=true`).flush({
      data: [
        { id: 'n1', is_read: false },
        { id: 'n2', is_read: false },
      ],
      error: null,
    });

    service.markAllRead();
    httpMock.expectOne(`${base}/notifications/read-all`).flush({ data: null, error: null });

    expect(service.unreadCount()).toBe(0);
  });

  it('clear() empties the notifications list', () => {
    const service = setup();
    service.loadNotifications(true);
    httpMock.expectOne(`${base}/notifications?unread=true`).flush({
      data: [{ id: 'n1', is_read: false }],
      error: null,
    });
    expect(service.notifications().length).toBe(1);

    service.clear();
    expect(service.notifications()).toEqual([]);
  });
});
