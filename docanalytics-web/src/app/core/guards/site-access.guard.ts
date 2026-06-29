import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Mirrors the server's FR-5.3 check: block a :siteId the user isn't granted.
export const siteAccessGuard: CanActivateFn = async (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const siteId = route.paramMap.get('siteId');
  if (!siteId) return router.createUrlTree(['/login']);

  // 🔑 Ensure the session (and the sites list) is loaded BEFORE checking access.
  // ensureSession() is idempotent — if authGuard already loaded it, this returns instantly.
  const ok = await auth.ensureSession();
  if (!ok) return router.createUrlTree(['/login']);

  if (!auth.hasSiteAccess(siteId)) {
    return router.createUrlTree(['/login']); // Round 5 can route to a /forbidden page
  }

  return true;
};
