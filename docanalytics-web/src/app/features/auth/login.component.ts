import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly step = signal<'credentials' | 'mfa'>('credentials');
  readonly challengeToken = signal<string | null>(null);
  readonly mfaCode = signal('');
  readonly mfaError = signal<string | null>(null);
  readonly mfaLoading = signal(false);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  constructor() {
    // If a valid session already exists (e.g. user hits /login with a live token),
    // skip the form and route by role.
    this.auth.ensureSession().then((ok) => {
      if (ok) this.routeByRole();
    });
  }

  isInvalid(name: 'email' | 'password'): boolean {
    const c = this.form.controls[name];
    return c.invalid && (c.touched || c.dirty);
  }

  submit(): void {
    if (this.loading()) return; // ⬅️ guard against double/rapid submits

    this.errorMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const { email, password } = this.form.getRawValue();

    this.auth.login(email, password).subscribe({
      next: (res) => {
        this.loading.set(false);
        const data = res.data;
        if (res.error || !data) {
          this.errorMessage.set('Invalid email or password.');
          return;
        }

        if ('requires_two_factor' in data) {
          this.challengeToken.set(data.challenge_token);
          this.step.set('mfa');
          return;
        }

        if (data.must_change_password) {
          this.router.navigate(['/change-password']);
          return;
        }

        if (!this.routeByRole()) {
          this.errorMessage.set('Your account has no site access. Contact your administrator.');
          this.auth.logout();
        }
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        if (err.status === 429) {
          this.errorMessage.set(this.rateLimitMessage(err));
        } else if (err.status === 401) {
          this.errorMessage.set('Invalid email or password.');
        } else if (err.status === 0) {
          this.errorMessage.set('Cannot reach the server. Check your connection and try again.');
        } else {
          this.errorMessage.set('Something went wrong. Please try again.');
        }
      },
    });
  }

  submitMfa(): void {
    const token = this.challengeToken();
    const code = this.mfaCode().trim();
    if (!token || code.length < 6) return;

    this.mfaError.set(null);
    this.mfaLoading.set(true);
    this.auth.loginWithTwoFactor(token, code).subscribe({
      next: (res) => {
        this.mfaLoading.set(false);
        if (res.error || !res.data) {
          this.mfaError.set('Invalid or expired code. Try again or use a recovery code.');
          return;
        }
        if (res.data.must_change_password) {
          this.router.navigate(['/change-password']);
          return;
        }
        if (!this.routeByRole()) {
          this.mfaError.set('Your account has no site access. Contact your administrator.');
          this.auth.logout();
        }
      },
      error: (err: HttpErrorResponse) => {
        this.mfaLoading.set(false);
        if (err.status === 429) {
          this.mfaError.set(this.rateLimitMessage(err));
        } else {
          this.mfaError.set('Invalid or expired code. Try again or use a recovery code.');
        }
      },
    });
  }

  /** Builds a friendly rate-limit message, using Retry-After header if the server sends one. */
  private rateLimitMessage(err: HttpErrorResponse): string {
    const retryAfter = err.headers?.get?.('Retry-After');
    const seconds = retryAfter ? parseInt(retryAfter, 10) : null;
    if (seconds && !isNaN(seconds) && seconds > 0) {
      return `Too many attempts. Please wait ${seconds}s and try again.`;
    }
    return 'Too many attempts. Please wait a moment and try again.';
  }

  backToCredentials(): void {
    this.step.set('credentials');
    this.challengeToken.set(null);
    this.mfaCode.set('');
    this.mfaError.set(null);
  }

  /** Route by role: Developer → provisioning console; others → first site. Returns false if nowhere to go. */
  private routeByRole(): boolean {
    if (this.auth.currentUser()?.role === 'Developer') {
      this.router.navigate(['/provision']);
      return true;
    }
    const sites = this.auth.sites();
    if (!sites.length) return false;
    this.router.navigate(['/site', sites[0].site_id, 'dashboard']);
    return true;
  }
}
