import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/** Factory guard: roleGuard(['Admin']) — allows only the listed roles. */
export const roleGuard =
  (allowed: string[]): CanActivateFn =>
  async () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (!(await auth.ensureSession())) return router.createUrlTree(['/login']);

    const role = auth.currentUser()?.role ?? '';
    return allowed.includes(role) ? true : router.createUrlTree(['/login']);
  };
