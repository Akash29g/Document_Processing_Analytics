import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';
import { toDataURL } from 'qrcode';
import { Location } from '@angular/common';

@Component({
  selector: 'app-two-factor-setup',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './two-factor-setup.component.html',
  styleUrl: './two-factor-setup.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TwoFactorSetupComponent {
  private auth = inject(AuthService);

  protected step = signal<'loading' | 'scan' | 'confirmed' | 'already-enabled'>('loading');
  protected qrDataUrl = signal<string | null>(null);
  protected manualKey = signal('');
  protected code = signal('');
  protected error = signal<string | null>(null);
  protected loading = signal(false);
  protected recoveryCodes = signal<string[]>([]);

  // ── disable-2FA sub-flow ──────────────────────────────────────
  protected disablePassword = signal('');
  protected disableError = signal<string | null>(null);
  protected disableLoading = signal(false);
  protected disabled = signal(false);

  constructor(private location: Location) {
    this.startSetup();
  }

  goBack(): void {
    this.location.back();
  }

  private startSetup(): void {
    this.error.set(null);
    this.auth.setupTwoFactor().subscribe({
      next: async (res) => {
        if (!res.data) {
          this.error.set(res.error?.message ?? 'Could not start 2FA setup.');
          return;
        }
        this.manualKey.set(res.data.manual_key);
        this.qrDataUrl.set(await toDataURL(res.data.otp_auth_uri));
        this.step.set('scan');
      },
      error: (err: HttpErrorResponse) => {
        const code = err.error?.error?.code;
        const message = err.error?.error?.message;

        if (code === 'TWO_FACTOR_ALREADY_ENABLED') {
          this.error.set(message ?? 'Two-factor authentication is already enabled.');
          this.step.set('already-enabled');
          return;
        }
        this.error.set(message ?? 'Could not start 2FA setup.');
      },
    });
  }

  protected confirm(): void {
    if (this.code().length !== 6) return;
    this.loading.set(true);
    this.error.set(null);
    this.auth.confirmTwoFactor(this.code()).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.error || !res.data) {
          this.error.set('Invalid code. Check your app and try again.');
          return;
        }
        this.recoveryCodes.set(res.data.recovery_codes);
        this.step.set('confirmed');
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Invalid code. Check your app and try again.');
      },
    });
  }

  protected disableTwoFactor(): void {
    const password = this.disablePassword().trim();
    if (!password) return;

    this.disableLoading.set(true);
    this.disableError.set(null);
    this.auth.disableTwoFactor(password).subscribe({
      next: (res) => {
        this.disableLoading.set(false);
        if (res.error || !res.data?.disabled) {
          this.disableError.set('Incorrect password. Try again.');
          return;
        }
        this.disabled.set(true);
        this.disablePassword.set('');
      },
      error: () => {
        this.disableLoading.set(false);
        this.disableError.set('Incorrect password. Try again.');
      },
    });
  }

  /** After disabling, let the user immediately re-enable if they want to. */
  protected restartSetup(): void {
    this.disabled.set(false);
    this.step.set('loading');
    this.startSetup();
  }
}
