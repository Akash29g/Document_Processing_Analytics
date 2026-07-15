import { Component, ElementRef, HostListener, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { SiteContextService } from '../../../core/services/site-context.service';

@Component({
  selector: 'app-site-selector',
  standalone: true,
  templateUrl: './site-selector.component.html',
  styleUrl: './site-selector.component.css',
})
export class SiteSelectorComponent {
  private auth = inject(AuthService);
  private siteCtx = inject(SiteContextService);
  private router = inject(Router);
  private host = inject(ElementRef);

  sites = this.auth.sites;
  currentSiteId = this.siteCtx.selectedSiteId;

  open = signal(false);
  activeIndex = signal(0);

  currentName = computed(() => {
    const id = this.currentSiteId();
    return this.sites().find((s) => s.site_id === id)?.site_name ?? 'Select site';
  });

  toggle(): void {
    this.open.update((v) => !v);
    if (this.open()) {
      // start the keyboard highlight on the currently-selected row
      const idx = this.sites().findIndex((s) => s.site_id === this.currentSiteId());
      this.activeIndex.set(idx >= 0 ? idx : 0);
    }
  }

  choose(siteId: string): void {
    this.open.set(false);
    if (siteId === this.currentSiteId()) return;
    const url = this.router.url.replace(/\/site\/[^/]+/, `/site/${siteId}`);
    this.router.navigateByUrl(url);
  }

  // ----- click outside closes -----
  @HostListener('document:click', ['$event'])
  onDocClick(e: MouseEvent): void {
    if (!this.host.nativeElement.contains(e.target)) this.open.set(false);
  }

  // ----- keyboard nav -----
  @HostListener('keydown', ['$event'])
  onKey(e: KeyboardEvent): void {
    if (!this.open()) {
      if (e.key === 'Enter' || e.key === ' ' || e.key === 'ArrowDown') {
        e.preventDefault();
        this.toggle();
      }
      return;
    }
    const last = this.sites().length - 1;
    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        this.activeIndex.update((i) => Math.min(i + 1, last));
        break;
      case 'ArrowUp':
        e.preventDefault();
        this.activeIndex.update((i) => Math.max(i - 1, 0));
        break;
      case 'Enter':
        e.preventDefault();
        this.choose(this.sites()[this.activeIndex()].site_id);
        break;
      case 'Escape':
        this.open.set(false);
        break;
    }
  }
}
