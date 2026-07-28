import { TestBed } from '@angular/core/testing';
import { Location } from '@angular/common';
import { of, throwError } from 'rxjs';
import { TwoFactorSetupComponent } from './two-factor-setup.component';
import { AuthService } from '../../core/services/auth.service';

vi.mock('qrcode', () => ({
  toDataURL: vi.fn().mockResolvedValue('data:image/png;base64,FAKE'),
}));

describe('TwoFactorSetupComponent', () => {
  let authServiceMock: {
    setupTwoFactor: ReturnType<typeof vi.fn>;
    confirmTwoFactor: ReturnType<typeof vi.fn>;
  };
  let locationMock: { back: ReturnType<typeof vi.fn> };

  const setupResponse = {
    data: {
      secret: 'SECRETBASE32',
      otp_auth_uri: 'otpauth://totp/DocAnalytics:user@test.com?secret=SECRETBASE32',
      manual_key: 'SECR ETBA SE32',
    },
    error: null,
  };

  function createFixture() {
    const fixture = TestBed.createComponent(TwoFactorSetupComponent);
    return { fixture, component: fixture.componentInstance };
  }

  async function renderAtScanStep() {
    const { fixture, component } = createFixture();
    fixture.detectChanges();
    await Promise.resolve();
    await Promise.resolve();
    fixture.detectChanges();
    return { fixture, component };
  }

  beforeEach(() => {
    authServiceMock = {
      setupTwoFactor: vi.fn().mockReturnValue(of(setupResponse)),
      confirmTwoFactor: vi.fn(),
    };
    locationMock = { back: vi.fn() };

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Location, useValue: locationMock },
      ],
    });
  });

  it('renders the scan step with QR code and manual key after setup resolves', async () => {
    const { fixture, component } = await renderAtScanStep();

    expect(authServiceMock.setupTwoFactor).toHaveBeenCalledTimes(1);
    expect((component as any).step()).toBe('scan');
    expect((component as any).manualKey()).toBe('SECR ETBA SE32');
    expect((component as any).qrDataUrl()).toBe('data:image/png;base64,FAKE');

    expect(fixture.nativeElement.textContent).toContain('SECR ETBA SE32');
    const img = fixture.nativeElement.querySelector('img');
    expect(img?.getAttribute('src')).toBe('data:image/png;base64,FAKE');
  });

  it('renders an error alert when setup returns no data', async () => {
    authServiceMock.setupTwoFactor.mockReturnValue(of({ data: null, error: 'boom' }));
    const { fixture, component } = createFixture();
    fixture.detectChanges();
    await Promise.resolve();
    fixture.detectChanges();

    expect((component as any).error()).toBe('Could not start 2FA setup.');
    expect(fixture.nativeElement.textContent).toContain('Could not start 2FA setup.');
  });

  it('renders an error alert when the setup request fails', async () => {
    authServiceMock.setupTwoFactor.mockReturnValue(throwError(() => new Error('network')));
    const { fixture, component } = createFixture();
    fixture.detectChanges();
    await Promise.resolve();
    fixture.detectChanges();

    expect((component as any).error()).toBe('Could not start 2FA setup.');
    expect(fixture.nativeElement.querySelector('.alert')?.textContent).toContain(
      'Could not start 2FA setup.',
    );
  });

  it('goBack() delegates to Location.back()', () => {
    const { fixture, component } = createFixture();
    fixture.detectChanges();
    (component as any).goBack();

    expect(locationMock.back).toHaveBeenCalledTimes(1);
  });

  describe('confirm()', () => {
    it('does nothing if the code is not exactly 6 characters', async () => {
      const { component } = await renderAtScanStep();
      (component as any).code.set('123');

      (component as any).confirm();

      expect(authServiceMock.confirmTwoFactor).not.toHaveBeenCalled();
    });

    it('enables 2FA and renders recovery codes on success', async () => {
      authServiceMock.confirmTwoFactor.mockReturnValue(
        of({ data: { recovery_codes: ['AAAA-1111', 'BBBB-2222'] }, error: null }),
      );
      const { fixture, component } = await renderAtScanStep();
      (component as any).code.set('123456');

      (component as any).confirm();
      fixture.detectChanges();

      expect((component as any).step()).toBe('confirmed');
      const text: string = fixture.nativeElement.textContent;
      expect(text).toContain('AAAA-1111');
      expect(text).toContain('BBBB-2222');
      expect(text).toContain('2FA is now enabled');
    });

    it('renders an error and stays on the scan step for an invalid code', async () => {
      authServiceMock.confirmTwoFactor.mockReturnValue(of({ data: null, error: 'bad code' }));
      const { fixture, component } = await renderAtScanStep();
      (component as any).code.set('000000');

      (component as any).confirm();
      fixture.detectChanges();

      expect((component as any).error()).toBe('Invalid code. Check your app and try again.');
      expect((component as any).step()).toBe('scan');
      expect(fixture.nativeElement.textContent).toContain(
        'Invalid code. Check your app and try again.',
      );
    });

    it('sets an error when the confirm request itself fails', async () => {
      authServiceMock.confirmTwoFactor.mockReturnValue(throwError(() => new Error('network')));
      const { fixture, component } = await renderAtScanStep();
      (component as any).code.set('123456');

      (component as any).confirm();
      fixture.detectChanges();

      expect((component as any).error()).toBe('Invalid code. Check your app and try again.');
      expect((component as any).loading()).toBe(false);
    });

    it('shows a "Verifying…" state while the confirm request is in flight', async () => {
      const { fixture, component } = await renderAtScanStep();
      (component as any).code.set('123456');
      (component as any).loading.set(true);
      fixture.detectChanges();

      expect(fixture.nativeElement.textContent).toContain('Verifying');
    });
  });
});
