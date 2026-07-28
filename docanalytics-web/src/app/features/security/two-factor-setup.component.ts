import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
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

  protected step = signal<'loading' | 'scan' | 'confirmed'>('loading');
  protected qrDataUrl = signal<string | null>(null);
  protected manualKey = signal('');
  protected code = signal('');
  protected error = signal<string | null>(null);
  protected loading = signal(false);
  protected recoveryCodes = signal<string[]>([]);

  constructor(private location: Location) {
    this.startSetup();
  }

  goBack(): void {
    this.location.back();
  }

  private startSetup(): void {
    this.auth.setupTwoFactor().subscribe({
      next: async (res) => {
        if (res.error) {
          this.error.set(res.error.message);
          return;
        }
        if (!res.data) {
          this.error.set('Could not start 2FA setup.');
          return;
        }
        this.manualKey.set(res.data.manual_key);
        // QR is rendered CLIENT-SIDE from the otpauth:// URI — never generated server-side.
        this.qrDataUrl.set(await toDataURL(res.data.otp_auth_uri));
        this.step.set('scan');
      },
      error: () => this.error.set('Could not start 2FA setup.'),
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
}
