import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <div class="login-wrap">
      <form class="login-card" [formGroup]="form" (ngSubmit)="submit()" novalidate>
        <h1 class="brand">DocAnalytics</h1>
        <p class="subtitle">Sign in to your monitoring dashboard</p>

        <!-- top-level error (bad creds / server / network) -->
        @if (errorMessage()) {
          <div class="alert" role="alert">{{ errorMessage() }}</div>
        }

        <label class="field">
          <span class="label">Email</span>
          <input
            type="email"
            formControlName="email"
            autocomplete="username"
            placeholder="you@company.com"
            [class.invalid]="isInvalid('email')"
          />
          @if (isInvalid('email')) {
            <span class="hint">
              @if (form.controls.email.hasError('required')) { Email is required. }
              @else { Enter a valid email address. }
            </span>
          }
        </label>

        <label class="field">
          <span class="label">Password</span>
          <input
            type="password"
            formControlName="password"
            autocomplete="current-password"
            placeholder="••••••••"
            [class.invalid]="isInvalid('password')"
          />
          @if (isInvalid('password')) {
            <span class="hint">Password is required.</span>
          }
        </label>

        <!-- Swap this for Shubh's <app-button> once the Round-1 atom merges. -->
        <button type="submit" class="btn" [disabled]="loading()">
          @if (loading()) { <span class="spinner" aria-hidden="true"></span> Signing in… }
          @else { Sign in }
        </button>
      </form>
    </div>
  `,
  styles: [`
    .login-wrap {
      min-height: 100vh;
      display: grid;
      place-items: center;
      background: var(--purple-900);
      padding: 24px;
    }
    .login-card {
      width: 100%;
      max-width: 380px;
      box-sizing: border-box;
      background: #fff;
      border: 1px solid var(--purple-200, #e6dbf0);
      border-radius: 14px;
      padding: 32px 28px;
      box-shadow: 0 10px 30px rgba(61, 17, 82, 0.08);
      display: flex;
      flex-direction: column;
      gap: 14px;
    }
    .brand { margin: 0; color: var(--purple-900, #3d1152); font-size: 1.6rem; }
    .subtitle { margin: 0 0 8px; color: var(--muted, #6b6480); font-size: 0.9rem; }
    .field { display: flex; flex-direction: column; gap: 6px; }
    .label { font-size: 0.82rem; font-weight: 600; color: var(--ink, #1a1430); }
    input {
      padding: 10px 12px;
      border: 1px solid var(--line, #ece8f1);
      border-radius: 8px;
      font-size: 0.95rem;
      outline: none;
      transition: border-color 0.15s;
    }
    input:focus { border-color: var(--purple-500, #7c3aed); }
    input.invalid { border-color: #d92d20; }
    .hint { font-size: 0.78rem; color: #d92d20; }
    .alert {
      background: #fef3f2;
      border: 1px solid #fda29b;
      color: #b42318;
      padding: 10px 12px;
      border-radius: 8px;
      font-size: 0.86rem;
    }
    .btn {
      margin-top: 6px;
      padding: 11px 16px;
      border: none;
      border-radius: 8px;
      background: var(--purple-900, #3d1152);
      color: #fff;
      font-size: 0.95rem;
      font-weight: 600;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      transition: background 0.15s;
    }
    .btn:hover:not(:disabled) { background: var(--purple-700, #5b2580); }
    .btn:disabled { opacity: 0.7; cursor: not-allowed; }
    .spinner {
      width: 15px; height: 15px;
      border: 2px solid rgba(255,255,255,0.5);
      border-top-color: #fff;
      border-radius: 50%;
      animation: spin 0.7s linear infinite;
    }
    @keyframes spin { to { transform: rotate(360deg); } }
  `],
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  constructor() {
    // If a valid session already exists (e.g. user hits /login with a live token),
    // skip the form and go straight to their first site's dashboard.
    this.auth.ensureSession().then((ok) => {
      if (ok) this.goToFirstSite();
    });
  }

  isInvalid(name: 'email' | 'password'): boolean {
    const c = this.form.controls[name];
    return c.invalid && (c.touched || c.dirty);
  }

  submit(): void {
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
        if (res.error || !res.data) {
          this.errorMessage.set('Invalid email or password.');
          return;
        }
        if (!this.goToFirstSite()) {
          this.errorMessage.set('Your account has no site access. Contact your administrator.');
          this.auth.logout();
        }
      },
      // Login 401 is handled HERE locally (not via the global "Session expired" toast).
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        if (err.status === 401) {
          this.errorMessage.set('Invalid email or password.');
        } else if (err.status === 0) {
          this.errorMessage.set('Cannot reach the server. Check your connection and try again.');
        } else {
          this.errorMessage.set('Something went wrong. Please try again.');
        }
      },
    });
  }

  /** Navigate to the first authorized site's dashboard. Returns false if none. */
  private goToFirstSite(): boolean {
    const sites = this.auth.sites();
    if (!sites.length) return false;
    this.router.navigate(['/site', sites[0].site_id, 'dashboard']);
    return true;
  }
}
