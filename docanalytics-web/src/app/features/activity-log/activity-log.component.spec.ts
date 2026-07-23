import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivityLogComponent } from './activity-log.component';
import { ActivityLogService } from './activity-log.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { provideRouter, Router } from '@angular/router';

describe('ActivityLogComponent', () => {
  const svc = {
    rows: signal([]),
    loading: signal(false),
    error: signal(null),
    meta: signal(null),
    query: { sortBy: 'ts', sortDir: 'desc' },
    load: vi.fn(),
    setPage: vi.fn(),
    setPageSize: vi.fn(),
    setSort: vi.fn(),
    setFilters: vi.fn(),
    setSearch: vi.fn(),
  };
  const site = signal<string | null>('s1');

  beforeEach(async () => {
    Object.values(svc).forEach((v) => (v as any).mockClear?.());
    await TestBed.configureTestingModule({
      imports: [ActivityLogComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        { provide: ActivityLogService, useValue: svc },
        { provide: SiteContextService, useValue: { selectedSiteId: site } },
      ],
    }).compileComponents();
  });

  it('creates and wires the service', () => {
    const comp = TestBed.createComponent(ActivityLogComponent).componentInstance;
    expect(comp).toBeTruthy();
  });

  it('onFilters() delegates to the service', () => {
    const comp = TestBed.createComponent(ActivityLogComponent).componentInstance as any;
    comp.onFilters({ status: 'BATCH_SUBMITTED', source: null, from: null, to: null });
    expect(svc.setFilters).toHaveBeenCalled();
  });

  it('onSort() delegates to the service', () => {
    const comp = TestBed.createComponent(ActivityLogComponent).componentInstance as any;
    comp.onSort({ sortBy: 'event_type', sortDir: 'asc' });
    expect(svc.setSort).toHaveBeenCalledWith('event_type', 'asc');
  });

  it('reloads when the selected site changes', () => {
    const fixture = TestBed.createComponent(ActivityLogComponent);
    fixture.detectChanges(); // runs the effect once
    svc.load.mockClear();
    site.set('s2');
    TestBed.tick();
    fixture.detectChanges();
    expect(svc.load).toHaveBeenCalled();
  });

  it('isNavigable returns true for Batch entity', () => {
    const comp = TestBed.createComponent(ActivityLogComponent).componentInstance as any;
    expect(comp.isNavigable({ entity_type: 'Batch', entity_id: 'abc', batch_id: null })).toBe(true);
  });

  it('isNavigable returns true for File entity with batch_id', () => {
    const comp = TestBed.createComponent(ActivityLogComponent).componentInstance as any;
    expect(comp.isNavigable({ entity_type: 'File', entity_id: 'abc', batch_id: 'bbb' })).toBe(true);
  });

  it('isNavigable returns false for File entity without batch_id', () => {
    const comp = TestBed.createComponent(ActivityLogComponent).componentInstance as any;
    expect(comp.isNavigable({ entity_type: 'File', entity_id: 'abc', batch_id: null })).toBe(false);
  });

  it('navigateTo returns early when siteId is null', () => {
    site.set(null);
    const comp = TestBed.createComponent(ActivityLogComponent).componentInstance as any;
    expect(() =>
      comp.navigateTo({ entity_type: 'Batch', entity_id: 'abc', batch_id: null }),
    ).not.toThrow();
    site.set('s1'); // restore
  });

  it('navigateTo routes to batch detail', () => {
    const comp = TestBed.createComponent(ActivityLogComponent).componentInstance as any;
    const router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate').mockResolvedValue(true);
    comp.navigateTo({ entity_type: 'Batch', entity_id: 'b1', batch_id: null });
    expect(router.navigate).toHaveBeenCalled();
  });

  it('navigateTo routes to file detail', () => {
    const comp = TestBed.createComponent(ActivityLogComponent).componentInstance as any;
    const router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate').mockResolvedValue(true);
    comp.navigateTo({ entity_type: 'File', entity_id: 'f1', batch_id: 'b1' });
    expect(router.navigate).toHaveBeenCalled();
  });
});
