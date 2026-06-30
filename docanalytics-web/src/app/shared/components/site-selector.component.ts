import {
  Component, ElementRef, HostListener, computed, inject, signal,
} from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { SiteContextService } from '../../core/services/site-context.service';

@Component({
  selector: 'app-site-selector',
  standalone: true,
  template: `
    <div class="dd">
      <!-- trigger button (closed state) -->
      <button
        type="button" class="dd-trigger"
        [class.open]="open()"
        (click)="toggle()"
        [attr.aria-expanded]="open()"
        aria-haspopup="listbox">
        <span class="label">{{ currentName() }}</span>
        <span class="material-icons chevron" aria-hidden="true">expand_more</span>
      </button>

      <!-- options list -->
      @if (open()) {
        <ul class="dd-list" role="listbox" tabindex="-1">
          @for (s of sites(); track s.site_id; let i = $index) {
            <li
              role="option"
              [attr.aria-selected]="s.site_id === currentSiteId()"
              [class.selected]="s.site_id === currentSiteId()"
              [class.active]="i === activeIndex()"
              (click)="choose(s.site_id)"
              (mouseenter)="activeIndex.set(i)">
              {{ s.site_name }}
            </li>
          }
        </ul>
      }
    </div>
  `,
  styles: [`
    .dd { position: relative; display: inline-block; min-width: 220px; }

    /* ----- closed trigger ----- */
    .dd-trigger {
      width: 100%; display: flex; align-items: center; justify-content: space-between;
      font-family: var(--font-body); font-size: 16px; color: var(--dark-gray);
      background: var(--white); border: 1px solid var(--cool-gray);
      border-radius: 4px; padding: 8px 12px; cursor: pointer;
    }
    .dd-trigger:focus { outline: none; border-color: var(--slate-blue); }
    .dd-trigger.open { border-color: var(--slate-blue); }
    .chevron { font-size: 20px; color: var(--dark-gray-3); transition: transform .15s; }
    .dd-trigger.open .chevron { transform: rotate(180deg); }

    /* ----- open list ----- */
    .dd-list {
      position: absolute; top: calc(100% + 4px); left: 0; right: 0; z-index: 50;
      margin: 0; padding: 4px 0; list-style: none;
      background: var(--white); border: 1px solid var(--cool-gray);
      border-radius: 4px; box-shadow: 0 4px 16px 0 rgba(0,0,0,.08);
      max-height: 280px; overflow-y: auto;
    }
    .dd-list li {
      font-family: var(--font-body); font-size: 16px; color: var(--dark-gray);
      padding: 10px 12px; cursor: pointer;
    }
    /* AVEVA spec: hovered/active row = light gray */
    .dd-list li.active { background: var(--light-gray); }
    /* selected row = light gray + emphasis (no native blue) */
    .dd-list li.selected { background: var(--light-gray); font-weight: 600; }
  `],
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
    return this.sites().find(s => s.site_id === id)?.site_name ?? 'Select site';
  });

  toggle(): void {
    this.open.update(v => !v);
    if (this.open()) {
      // start the keyboard highlight on the currently-selected row
      const idx = this.sites().findIndex(s => s.site_id === this.currentSiteId());
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
        e.preventDefault(); this.toggle();
      }
      return;
    }
    const last = this.sites().length - 1;
    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        this.activeIndex.update(i => Math.min(i + 1, last));
        break;
      case 'ArrowUp':
        e.preventDefault();
        this.activeIndex.update(i => Math.max(i - 1, 0));
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
