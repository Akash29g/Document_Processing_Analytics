import { Component, ElementRef, HostListener, computed, inject, signal,effect } from '@angular/core';
import { ActivatedRoute, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { SiteContextService } from '../../core/services/site-context.service';
import { ToastService } from '../../core/services/toast.service';
import { AuthService } from '../../core/services/auth.service';
import { SiteSelectorComponent } from '../../shared/components/site-selector/site-selector.component';
import { ThemeService } from '../../core/services/theme.service';
import { AlertsService } from '../../features/alerts/alerts.service';
import { DatePipe } from '@angular/common';


@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, SiteSelectorComponent, DatePipe],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.css',
})
export class ShellComponent {
  private route = inject(ActivatedRoute);
  private siteCtx = inject(SiteContextService);
  private auth = inject(AuthService);
  private router = inject(Router);
  toast = inject(ToastService);
  protected theme = inject(ThemeService);
  protected alerts = inject(AlertsService);


  // controls the logout confirmation dialog
  protected showLogoutConfirm = signal(false);

  // current :siteId from the URL, exposed as a signal for the template
  siteId = toSignal(this.route.paramMap.pipe(map(p => p.get('siteId'))), { initialValue: null });

  // current logged-in user (signal from AuthService) — drives the role label
  readonly user = this.auth.currentUser;
  private host = inject(ElementRef);
  protected menuOpen = signal(false);

  protected bellOpen = signal(false);
  private burstedForSite: string | null = null;   // toast critical alerts once per site


  protected isAdmin = computed(() =>
    (this.user()?.role ?? '').toLowerCase() === 'admin');

  // "admin@acme.com" → "AD", "user.a@acme.com" → "UA"
  protected initials = computed(() => {
    const local = (this.user()?.email ?? '').split('@')[0] ?? '';
    const parts = local.split(/[.\-_]/).filter(Boolean);
    const s = parts.length >= 2 ? parts[0][0] + parts[1][0] : local.slice(0, 2);
    return s.toUpperCase();
  });

  // "admin@acme.com" → "Acme", "user.c@globex.com" → "Globex"
  protected company = computed(() => {
    const name = (this.user()?.email ?? '').split('@')[1]?.split('.')[0] ?? '';
    return name ? name.charAt(0).toUpperCase() + name.slice(1) : '';
  });

  // "admin@acme.com" → "Admin", "user.a@acme.com" → "User A"
  protected displayName = computed(() =>
    ((this.user()?.email ?? '').split('@')[0] ?? '')
      .split(/[.\-_]/).filter(Boolean)
      .map((p) => p.charAt(0).toUpperCase() + p.slice(1)).join(' '));

  protected toggleMenu(): void { this.menuOpen.update((v) => !v); }

  protected toggleBell(): void { this.bellOpen.update((v) => !v); }
  protected onMarkRead(id: string): void { this.alerts.markRead(id); }
  protected onMarkAllRead(): void { this.alerts.markAllRead(); }


  @HostListener('document:click', ['$event'])
  onDocClick(e: MouseEvent): void {
    const target = e.target as HTMLElement;
    if (this.menuOpen() && !target.closest('.me-menu')) this.menuOpen.set(false);
    if (this.bellOpen() && !target.closest('.al-dd')) this.bellOpen.set(false);
  }


  constructor() {
    // mirror the :siteId URL param into the global service (DT-3 design)
    this.route.paramMap
      .pipe(takeUntilDestroyed())
      .subscribe(p => this.siteCtx.setSite(p.get('siteId')));

    // Load fired alerts whenever a site becomes active (login OR site switch),
    // and toast-burst any critical unread ones — once per site.
    effect(() => {
      const site = this.siteCtx.selectedSiteId();
      if (!site) return;

      this.alerts.loadNotifications(true, () => {
        if (this.burstedForSite === site) return;
        this.burstedForSite = site;
        this.alerts
          .notifications()
          .filter((n) => !n.is_read && n.severity === 'critical')
          .slice(0, 3)
          .forEach((n) => this.toast.warning(`⚠ ${n.rule_name}: ${n.message}`));
      });
    });
  }

  link(page: string) {
    return ['/site', this.siteId(), page];
  }

  // ── logout flow with confirmation ──
  askLogout(): void {
    this.showLogoutConfirm.set(true);
  }

  cancelLogout(): void {
    this.showLogoutConfirm.set(false);
  }

  confirmLogout(): void {
    this.showLogoutConfirm.set(false);
    this.alerts.clear();
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
