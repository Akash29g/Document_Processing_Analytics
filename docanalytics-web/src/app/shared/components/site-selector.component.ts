import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { SiteContextService } from '../../core/services/site-context.service';

@Component({
  selector: 'app-site-selector',
  standalone: true,
  template: `
    @if (sites().length) {
      <select class="site-select"
              [value]="currentSiteId() ?? ''"
              (change)="onChange($event)">
        @for (s of sites(); track s.site_id) {
          <option [value]="s.site_id">{{ s.site_name }}</option>
        }
      </select>
    } @else {
      <span class="site-pill">Site: {{ currentSiteId() ?? '—' }}</span>
    }
  `,
  styles: [`
    .site-select, .site-pill {
      background: var(--purple-050); border: 1px solid var(--purple-200);
      color: var(--purple-900); border-radius: 999px;
      padding: 6px 14px; font-size: 13px; font-weight: 600;
    }
    .site-select { cursor: pointer; }
  `],
})
export class SiteSelectorComponent {
  private auth = inject(AuthService);
  private siteCtx = inject(SiteContextService);
  private router = inject(Router);

  sites = this.auth.sites;                       // signal<SiteSummary[]>
  currentSiteId = this.siteCtx.selectedSiteId;   // signal<string | null>

  onChange(e: Event): void {
    const newId = (e.target as HTMLSelectElement).value;
    if (!newId) return;
    // swap the :siteId segment, keep the rest of the path → reloads data for the new site (FR-5.2)
    const swapped = this.router.url.replace(/^\/site\/[^/]+/, `/site/${newId}`);
    this.router.navigateByUrl(swapped.startsWith('/site/') ? swapped : `/site/${newId}/dashboard`);
  }
}
