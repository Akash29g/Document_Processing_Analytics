import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { SiteContextService } from '../../core/services/site-context.service';
import { ToastService } from '../../core/services/toast.service';
import { AuthService } from '../../core/services/auth.service';
import { SiteSelectorComponent } from '../../shared/components/site-selector/site-selector.component';
import { ThemeService } from '../../core/services/theme.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, SiteSelectorComponent],
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
