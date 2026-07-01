import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { siteAccessGuard } from './core/guards/site-access.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login.component').then((m) => m.LoginComponent),
  },
  {
    // Everything under a site is guarded + rendered inside Shubh's App shell.
    path: 'site/:siteId',
    canActivate: [authGuard, siteAccessGuard],
    loadComponent: () =>
      import('./layout/shell.component').then((m) => m.ShellComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(
            (m) => m.DashboardComponent,
          ),
      },

      {
        path: 'batches',
        loadComponent: () =>
          import('./features/batches/batch-list.component').then(m => m.BatchListComponent),
      },

      // 👇 Future rounds add their routes here (keep BOTH entries on merge):
      //   batches            (Round 3 — you)
      //   batches/:batchId   (Round 3 — Shubh)
      //   batches/:batchId/files/:fileId (Round 4 — you)
      //   errors             (Round 4 — Shubh)
      //   activity-log       (Round 5 — you)
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    ],
  },
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: '**', redirectTo: 'login' },
];
