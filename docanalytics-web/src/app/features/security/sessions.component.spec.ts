import { TestBed } from '@angular/core/testing';
import { Location } from '@angular/common';
import { provideRouter } from '@angular/router';
import { signal } from '@angular/core';
import { SessionsComponent } from './sessions.component';
import { SessionsService } from './sessions.service';

describe('SessionsComponent', () => {
  let sessionsServiceMock: {
    load: ReturnType<typeof vi.fn>;
    revoke: ReturnType<typeof vi.fn>;
    revokeAllOthers: ReturnType<typeof vi.fn>;
    sessions: ReturnType<typeof signal<any[]>>;
    loading: ReturnType<typeof signal<boolean>>;
    error: ReturnType<typeof signal<string | null>>;
  };
  let locationMock: { back: ReturnType<typeof vi.fn> };

  function createFixture() {
    const fixture = TestBed.createComponent(SessionsComponent);
    return { fixture, component: fixture.componentInstance };
  }

  beforeEach(() => {
    sessionsServiceMock = {
      load: vi.fn(),
      revoke: vi.fn(),
      revokeAllOthers: vi.fn(),
      sessions: signal<any[]>([]),
      loading: signal(false),
      error: signal<string | null>(null),
    };
    locationMock = { back: vi.fn() };

    TestBed.configureTestingModule({
      providers: [
        { provide: SessionsService, useValue: sessionsServiceMock },
        { provide: Location, useValue: locationMock },
        provideRouter([]),
      ],
    });
  });

  it('calls sessions.load() on init and renders the loading state', () => {
    sessionsServiceMock.loading.set(true);
    const { fixture } = createFixture();
    fixture.detectChanges();

    expect(sessionsServiceMock.load).toHaveBeenCalledTimes(1);
    expect(fixture.nativeElement.textContent).toContain('Loading sessions');
  });

  it('renders an error alert with a working Retry button', () => {
    sessionsServiceMock.error.set('Could not load active sessions.');
    const { fixture } = createFixture();
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Could not load active sessions.');
    const retryBtn: HTMLButtonElement = fixture.nativeElement.querySelector('.alert button');
    retryBtn.click();

    expect(sessionsServiceMock.load).toHaveBeenCalledTimes(2);
  });

  it('renders the sessions table with current-device badge and per-row revoke buttons', () => {
    sessionsServiceMock.sessions.set([
      {
        id: 's1',
        device_label: 'Chrome on Windows',
        ip_address: '1.2.3.4',
        created_at: '2026-01-01T00:00:00Z',
        last_used_at: '2026-01-02T00:00:00Z',
        is_current: true,
      },
      {
        id: 's2',
        device_label: 'Firefox on Mac',
        ip_address: '5.6.7.8',
        created_at: '2026-01-01T00:00:00Z',
        last_used_at: null,
        is_current: false,
      },
    ]);
    const { fixture } = createFixture();
    fixture.detectChanges();

    const text: string = fixture.nativeElement.textContent;
    expect(text).toContain('Chrome on Windows');
    expect(text).toContain('This device');
    expect(text).toContain('Firefox on Mac');
    expect(fixture.nativeElement.querySelectorAll('.btn-revoke').length).toBe(1);
  });

  it('goBack() delegates to Location.back()', () => {
    const { fixture, component } = createFixture();
    fixture.detectChanges();
    component.goBack();

    expect(locationMock.back).toHaveBeenCalledTimes(1);
  });

  describe('revoke()', () => {
    it('calls sessions.revoke(id) when the user confirms', () => {
      vi.spyOn(window, 'confirm').mockReturnValue(true);
      const { fixture, component } = createFixture();
      fixture.detectChanges();

      (component as any).revoke('session-1');

      expect(sessionsServiceMock.revoke).toHaveBeenCalledWith('session-1');
    });

    it('does not call sessions.revoke() when the user cancels', () => {
      vi.spyOn(window, 'confirm').mockReturnValue(false);
      const { fixture, component } = createFixture();
      fixture.detectChanges();

      (component as any).revoke('session-1');

      expect(sessionsServiceMock.revoke).not.toHaveBeenCalled();
    });
  });

  describe('revokeOthers()', () => {
    it('calls sessions.revokeAllOthers() via the rendered button when confirmed', () => {
      vi.spyOn(window, 'confirm').mockReturnValue(true);
      sessionsServiceMock.sessions.set([
        {
          id: 's1',
          device_label: 'A',
          ip_address: null,
          created_at: '2026-01-01T00:00:00Z',
          last_used_at: null,
          is_current: true,
        },
      ]);
      const { fixture } = createFixture();
      fixture.detectChanges();

      const btn: HTMLButtonElement = fixture.nativeElement.querySelector('.btn-revoke-all');
      btn.click();

      expect(sessionsServiceMock.revokeAllOthers).toHaveBeenCalledTimes(1);
    });

    it('does not call sessions.revokeAllOthers() when the user cancels', () => {
      vi.spyOn(window, 'confirm').mockReturnValue(false);
      const { fixture, component } = createFixture();
      fixture.detectChanges();

      (component as any).revokeOthers();

      expect(sessionsServiceMock.revokeAllOthers).not.toHaveBeenCalled();
    });
  });
});
