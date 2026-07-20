import { HttpInterceptorFn, HttpContextToken } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';
import { AuthService } from '../services/auth.service';

// the on/off switch — defaults to "show toast"
export const SKIP_ERROR_TOAST = new HttpContextToken<boolean>(() => false);

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(ToastService);
  const auth = inject(AuthService);

  return next(req).pipe(
    catchError((err) => {
      const isLogin = req.url.includes('/auth/login');
      const isRefresh = req.url.includes('/auth/refresh');
      const isLogout = req.url.includes('/auth/logout');

      // ── NEW (B6): rate-limited (429) → friendly toast for ANY endpoint, then stop ──
      if (err.status === 429) {
        const retryAfter = err.headers.get('Retry-After');
        const wait = retryAfter ? ` Try again in ${retryAfter}s.` : '';
        toast.warning(`You're doing that too fast.${wait}`);
        return throwError(() => err);
      }

      // Login handles its own errors inline — skip all global toasts/redirects.
      if (isLogin) {
        return throwError(() => err);
      }

      const skipToast = req.context.get(SKIP_ERROR_TOAST);
      const apiMsg = err?.error?.error?.message as string | undefined;

      if (err.status === 401) {
        // A failed refresh/logout must NOT trigger another refresh (avoid loops).
        if (isRefresh || isLogout) {
          auth.logout();
          router.navigate(['/login']);
          if (!skipToast) toast.error('Session expired — please log in again.');
          return throwError(() => err);
        }

        // Attempt ONE silent refresh, then replay the original request.
        return auth.refreshToken().pipe(
          switchMap((newToken) => {
            if (!newToken) {
              // refresh already cleared the session in AuthService
              router.navigate(['/login']);
              if (!skipToast) toast.error('Session expired — please log in again.');
              return throwError(() => err);
            }
            const retried = req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } });
            return next(retried);
          }),
        );
      } else if (!skipToast) {
        if (err.status === 403) {
          toast.error('You are not authorized for this site.');
        } else if (err.status === 0) {
          toast.error('Cannot reach the server. Is the API running?');
        } else {
          toast.error(apiMsg ?? `Something went wrong (${err.status}).`);
        }
      }

      return throwError(() => err);
    }),
  );
};
