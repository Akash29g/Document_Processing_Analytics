import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { SiteContextService } from '../core/services/site-context.service';
import { ToastService } from '../core/services/toast.service';
import { AuthService } from '../core/services/auth.service';
import { SiteSelectorComponent } from '../shared/components/site-selector.component';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, SiteSelectorComponent],
  template: `
  <div class="shell">
    <aside class="sidebar">
      <div class="brand">DocAnalytics</div>
      <nav>
        <a [routerLink]="link('dashboard')" routerLinkActive="active">Dashboard</a>
        <a [routerLink]="link('batches')" routerLinkActive="active">Batches</a>
        <a [routerLink]="link('errors')" routerLinkActive="active">Errors</a>
        <a [routerLink]="link('activity-log')" routerLinkActive="active">Activity Log</a>
      </nav>
    </aside>

    <div class="main">
      <header class="topbar">
        <app-site-selector />
        <span class="spacer"></span>
        <span class="user">{{ user()?.role ?? 'Viewer' }}</span>
        <button class="logout-btn" (click)="logout()">Log out</button>
      </header>
      <main class="content"><router-outlet /></main>
    </div>

    <!-- global toast outlet -->
<div class="toasts">
  @for (t of toast.toasts(); track t.id) {
    <div class="toast"
         [class.error]="t.type === 'error'"
         [class.success]="t.type === 'success'"
         [class.warning]="t.type === 'warning'">
      <span class="material-icons" aria-hidden="true">{{ icon(t.type) }}</span>
      <span class="toast-text">{{ t.text }}</span>
      <button (click)="toast.dismiss(t.id)">×</button>
    </div>
  }
</div>

  </div>
  `,
  styles: [`
  .shell { display: grid; grid-template-columns: 220px 1fr; height: 100vh; }

  /* Sidebar = brand chrome → AVEVA purple is allowed here */
  .sidebar { background: var(--aveva-purple); color: #fff; padding: 16px 12px; }
  .brand { font-family: var(--font-display); font-weight: 700; font-size: 18px; padding: 8px 12px 20px; }
  nav { display: flex; flex-direction: column; gap: 4px; }
  nav a {
    font-family: var(--font-display); font-weight: 600; font-size: 16px;
    color: rgba(255,255,255,.75); padding: 10px 12px; border-radius: 8px;
  }
  nav a:hover { background: var(--purple); color: #fff; }
  nav a.active { background: var(--purple); color: #fff; }   /* active tab — allowed */

  .main { display: flex; flex-direction: column; min-width: 0; }

  /* Topbar = white surface (NOT purple) */
  .topbar {
    height: 64px; display: flex; align-items: center; gap: 12px;
    background: var(--white); border-bottom: 1px solid var(--cool-gray);
    padding: 0 24px;
  }
  .spacer { flex: 1; }
  .user { color: var(--dark-gray-3); font-size: 14px; }

  /* Log out = ghost/secondary button (slate-blue, not purple) */
  .logout-btn {
    font-family: var(--font-display); font-weight: 600; font-size: 14px;
    background: transparent; border: 1px solid var(--slate-blue);
    color: var(--slate-blue); border-radius: 4px; padding: 8px 16px;
    cursor: pointer; transition: all .15s;
  }
  .logout-btn:hover { background: var(--slate-blue); color: #fff; }

  /* Content canvas = light page bg, NOT purple (this was a bug in your version) */
  .content { flex: 1; padding: 24px; overflow: auto; background: var(--bg-light); max-width: 1440px; }

  /* Toasts — left color bar + filled icon handled in template; bg is dark per Notification Bar spec */
  .toasts { position: fixed; bottom: 20px; right: 20px; display: flex;
            flex-direction: column; gap: 8px; z-index: 1000; }
  .toast {
    font-family: var(--font-body); font-weight: 600; font-size: 14px;
    background: rgba(0,0,0,.85); color: #fff; padding: 12px 16px;
    border-radius: 6px; box-shadow: 0 4px 12px rgba(0,0,0,.2);
    display: flex; gap: 12px; align-items: center;
    border-left: 4px solid var(--status-neutral);   /* default = info */
  }
  .toast.success { border-left-color: var(--status-confirmed); }
  .toast.error   { border-left-color: var(--status-error); }
  .toast.warning { border-left-color: var(--status-warning); }
  .toast button { background: transparent; border: none; color: #fff;
                  cursor: pointer; font-size: 16px; line-height: 1; }
`],

})
export class ShellComponent {
  private route = inject(ActivatedRoute);
  private siteCtx = inject(SiteContextService);
  private auth = inject(AuthService);
  private router = inject(Router);
  toast = inject(ToastService);

  // current :siteId from the URL, exposed as a signal for the template
  siteId = toSignal(this.route.paramMap.pipe(map(p => p.get('siteId'))), { initialValue: null });

  // current logged-in user (signal from AuthService) — drives the role label
  readonly user = this.auth.currentUser;

  constructor() {
    // mirror the :siteId URL param into the global service (DT-3 design)
    this.route.paramMap
      .pipe(takeUntilDestroyed())
      .subscribe(p => this.siteCtx.setSite(p.get('siteId')));
  }

  link(page: string) {
    return ['/site', this.siteId(), page];
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  icon(type: string): string {
    switch (type) {
      case 'success': return 'check_circle';
      case 'warning': return 'warning';
      case 'error': return 'error';
      default: return 'info';
    }
  }
}
