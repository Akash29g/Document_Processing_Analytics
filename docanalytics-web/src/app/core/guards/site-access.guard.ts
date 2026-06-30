import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { SiteContextService } from '../services/site-context.service';

export const siteAccessGuard: CanActivateFn = async (route) => {
  const auth = inject(AuthService);
  const siteCtx = inject(SiteContextService);
  const router = inject(Router);

  const siteId = route.paramMap.get('siteId');
  if (!siteId) return router.createUrlTree(['/login']);

  // ensure user + sites are loaded (handles hard refresh where only the token survives)
  const ok = await auth.ensureSession();
  if (!ok) return router.createUrlTree(['/login']);

  // FR-5.3 client-side check (server still enforces)
  if (auth.hasSiteAccess(siteId)) {
    siteCtx.setSite(siteId);
    return true;
  }

  // logged in but not authorized for THIS site → first allowed site, else login
  const fallback = auth.sites()[0];
  return router.createUrlTree(fallback ? ['/site', fallback.site_id, 'dashboard'] : ['/login']);
};
