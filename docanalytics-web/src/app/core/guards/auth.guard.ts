import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = async () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // No token at all → straight to login.
  if (!auth.token()) {
    return router.createUrlTree(['/login']);
  }

  // Token exists but signals empty (e.g. after refresh) → rehydrate via /auth/me.
  if (!auth.currentUser()) {
    const ok = await auth.ensureSession();
    if (!ok) return router.createUrlTree(['/login']);
  }

  return true;
};
