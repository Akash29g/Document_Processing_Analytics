import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([
        // 🔌 Shubh (Phase 0) registers his functional interceptors here:
        //   authSiteInterceptor,   ← attaches Bearer token + site_id
        //   errorInterceptor,      ← maps { error } → toast / inline
      ]),
    ),
  ],
};
