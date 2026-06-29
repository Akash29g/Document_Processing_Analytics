import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Mirrors the server's FR-5.3 check: block a :siteId the user isn't granted.
// Runs AFTER authGuard, so the session/sites are already hydrated.
export const siteAccessGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const siteId = route.paramMap.get('siteId');
  if (!siteId) return router.createUrlTree(['/login']);

  if (!auth.hasSiteAccess(siteId)) {
    // Phase 0 skeleton: bounce to login. (Round 5 can route to a /forbidden page.)
    return router.createUrlTree(['/login']);
  }

  return true;
};
