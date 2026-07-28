import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { siteAccessGuard } from './core/guards/site-access.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./features/auth/forgot-password.component').then((m) => m.ForgotPasswordComponent),
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./features/auth/reset-password.component').then((m) => m.ResetPasswordComponent),
  },

  {
    // Everything under a site is guarded + rendered inside Shubh's App shell.
    path: 'site/:siteId',
    canActivate: [authGuard, siteAccessGuard],
    loadComponent: () => import('./layout/shell/shell.component').then((m) => m.ShellComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
      },

      {
        path: 'batches',
        loadComponent: () =>
          import('./features/batches/batch-list.component').then((m) => m.BatchListComponent),
      },

      {
        path: 'upload',
        loadComponent: () =>
          import('./features/upload/upload.component').then((m) => m.UploadComponent),
      },

      {
        path: 'alerts',
        loadComponent: () =>
          import('./features/alerts/alerts.component').then((m) => m.AlertsComponent),
      },

      {
        path: 'activity-log',
        loadComponent: () =>
          import('./features/activity-log/activity-log.component').then(
            (m) => m.ActivityLogComponent,
          ),
      },

      {
        path: 'batches/:batchId/files/:fileId',
        loadComponent: () =>
          import('./features/files/file-details.component').then((m) => m.FileDetailsComponent),
      },

      // 👇 Future rounds add their routes here (keep BOTH entries on merge):
      //   batches            (Round 3 — you)
      //   batches/:batchId   (Round 3 — Shubh)
      //   batches/:batchId/files/:fileId (Round 4 — you)
      //   errors             (Round 4 — Shubh)
      //   activity-log       (Round 5 — you)
      {
        path: 'batches/:batchId',
        loadComponent: () =>
          import('./features/batches/batch-detail/batch-detail.component').then(
            (m) => m.BatchDetailComponent,
          ),
      },

      {
        path: 'errors',
        loadComponent: () =>
          import('./features/errors/errors.component').then((m) => m.ErrorsComponent),
      },

      {
        path: 'comparison',
        loadComponent: () =>
          import('./features/comparison/comparison.component').then((m) => m.ComparisonComponent),
      },

      {
        path: 'admin',
        canActivate: [roleGuard(['Admin'])],
        loadComponent: () =>
          import('./features/admin/admin.component').then((m) => m.AdminComponent),
      },

      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    ],
  },

  { path: '', pathMatch: 'full', redirectTo: 'login' },

  {
    path: 'change-password',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/auth/change-password.component').then((m) => m.ChangePasswordComponent),
  },
  {
    path: 'security',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/security/sessions.component').then((m) => m.SessionsComponent),
  },
  {
    path: 'security/2fa',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/security/two-factor-setup.component').then(
        (m) => m.TwoFactorSetupComponent,
      ),
  },

  {
    path: 'provision',
    canActivate: [roleGuard(['Developer'])],
    loadComponent: () =>
      import('./features/provisioning/provisioning.component').then((m) => m.ProvisioningComponent),
  },
  { path: '**', redirectTo: 'login' },
];
