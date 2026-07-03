import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',       
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
