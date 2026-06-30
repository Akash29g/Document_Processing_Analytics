import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { SiteContextService } from '../core/services/site-context.service';
import { ToastService } from '../core/services/toast.service';
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
        <span class="user">Viewer</span>
      </header>
      <main class="content"><router-outlet /></main>
    </div>

    <!-- global toast outlet -->
    <div class="toasts">
      @for (t of toast.toasts(); track t.id) {
        <div class="toast"
             [class.error]="t.type === 'error'"
             [class.success]="t.type === 'success'">
          {{ t.text }}
          <button (click)="toast.dismiss(t.id)">×</button>
        </div>
      }
    </div>
  </div>
  `,
  styles: [`
    .shell { display: grid; grid-template-columns: 220px 1fr; height: 100vh; }
    .sidebar { background: var(--purple-900); color: #fff; padding: 16px 12px; }
    .brand { font-weight: 700; font-size: 18px; padding: 8px 12px 20px; }
    nav { display: flex; flex-direction: column; gap: 4px; }
    nav a { color: var(--purple-200); padding: 10px 12px; border-radius: 8px; font-size: 14px; }
    nav a:hover { background: var(--purple-700); color: #fff; }
    nav a.active { background: var(--purple-700); color: #fff; font-weight: 600; }
    .main { display: flex; flex-direction: column; min-width: 0; }
    .topbar { display: flex; align-items: center; gap: 12px; background: #fff;
              border-bottom: 1px solid var(--line); padding: 12px 20px; }
    .site-pill { background: var(--purple-050); border: 1px solid var(--purple-200);
                 color: var(--purple-900); border-radius: 999px; padding: 6px 14px;
                 font-size: 13px; font-weight: 600; }
    .spacer { flex: 1; }
    .user { color: var(--muted); font-size: 13px; }
    .content { padding: 20px; overflow: auto; }
    .toasts { position: fixed; bottom: 20px; right: 20px; display: flex;
              flex-direction: column; gap: 8px; z-index: 1000; }
    .toast { background: var(--purple-900); color: #fff; padding: 10px 14px;
             border-radius: 8px; box-shadow: 0 4px 12px rgba(61,17,82,.2);
             display: flex; gap: 12px; align-items: center; font-size: 14px; }
    .toast.error { background: var(--err); }
    .toast.success { background: var(--ok); }
    .toast button { background: transparent; border: none; color: #fff;
                    cursor: pointer; font-size: 16px; line-height: 1; }
  `],
})
export class ShellComponent {
  private route = inject(ActivatedRoute);
  private siteCtx = inject(SiteContextService);
  toast = inject(ToastService);

  // current :siteId from the URL, exposed as a signal for the template
  siteId = toSignal(this.route.paramMap.pipe(map(p => p.get('siteId'))), { initialValue: null });

  constructor() {
    // mirror the :siteId URL param into the global service (DT-3 design)
    this.route.paramMap
      .pipe(takeUntilDestroyed())
      .subscribe(p => this.siteCtx.setSite(p.get('siteId')));
  }

  link(page: string) {
    return ['/site', this.siteId(), page];
  }
}
