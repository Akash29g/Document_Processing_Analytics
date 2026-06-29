import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SiteContextService {
  /** the currently selected site id (source of truth = the :siteId URL param) */
  readonly selectedSiteId = signal<string | null>(null);

  setSite(id: string | null): void {
    this.selectedSiteId.set(id);
  }
}
