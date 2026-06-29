import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { SiteContextService } from '../services/site-context.service';

// NOTE: must match AuthService's storage key (coordinate with Dev A — we agreed on 'da_token').
// We read from localStorage (not AuthService) to avoid circular DI: AuthService uses HttpClient,
// and HttpClient runs these interceptors.
const TOKEN_KEY = 'da_token';

export const authSiteInterceptor: HttpInterceptorFn = (req, next) => {
  const siteId = inject(SiteContextService).selectedSiteId();
  const token = localStorage.getItem(TOKEN_KEY);

  let headers = req.headers;
  if (token) headers = headers.set('Authorization', `Bearer ${token}`);
  if (siteId) headers = headers.set('X-Site-Id', siteId);

  return next(req.clone({ headers }));
};
