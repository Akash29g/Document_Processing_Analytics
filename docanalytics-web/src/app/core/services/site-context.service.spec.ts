import { TestBed } from '@angular/core/testing';
import { SiteContextService } from './site-context.service';

describe('SiteContextService', () => {
  let service: SiteContextService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SiteContextService);
  });

  it('defaults selectedSiteId to null', () => {
    expect(service.selectedSiteId()).toBeNull();
  });

  it('setSite() updates the selectedSiteId signal', () => {
    service.setSite('site-123');
    expect(service.selectedSiteId()).toBe('site-123');
  });

  it('setSite(null) clears the selection', () => {
    service.setSite('site-123');
    service.setSite(null);
    expect(service.selectedSiteId()).toBeNull();
  });
});
