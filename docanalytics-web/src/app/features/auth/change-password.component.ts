import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './change-password.component.html',
  styleUrls: ['./login.component.css'],   // reuse the login card styles
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChangePasswordComponent {
  private http = inject(HttpClient);
  private router = inject(Router);
  private auth = inject(AuthService);

  protected current = '';
  protected next = '';
  protected confirm = '';
  protected loading = signal(false);
  protected error = signal<string | null>(null);

  protected async submit(): Promise<void> {
    this.error.set(null);
    if (this.next.length < 10) { this.error.set('New password must be at least 10 characters.'); return; }
    if (this.next !== this.confirm) { this.error.set('Passwords do not match.'); return; }

    this.loading.set(true);
    try {
      await firstValueFrom(this.http.post<ApiResponse<unknown>>(
        `${environment.apiBase}/auth/change-password`,
        { current_password: this.current, new_password: this.next }));
      // success → continue to wherever the role belongs
      this.auth.routeAfterLogin();
    } catch (e: any) {
      this.error.set(e?.error?.error?.message ?? 'Password change failed.');
    } finally {
      this.loading.set(false);
    }
  }
}
