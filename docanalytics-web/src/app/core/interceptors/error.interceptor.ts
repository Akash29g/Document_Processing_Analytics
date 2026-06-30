import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';

const TOKEN_KEY = 'da_token';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError(err => {
      const isLogin = req.url.includes('/auth/login');

      // Login handles its own errors inline — skip all global toasts/redirects.
      if (isLogin) {
        return throwError(() => err);
      }

      const apiMsg = err?.error?.error?.message as string | undefined;

      if (err.status === 401 && !isLogin) {
        localStorage.removeItem(TOKEN_KEY);
        toast.error('Session expired — please log in again.');
        router.navigate(['/login']);
      } else if (err.status === 403) {
        toast.error('You are not authorized for this site.');
      } else if (err.status === 0) {
        toast.error('Cannot reach the server. Is the API running?');
      } else {
        toast.error(apiMsg ?? `Something went wrong (${err.status}).`);
      }
      return throwError(() => err);
    })
  );
};
