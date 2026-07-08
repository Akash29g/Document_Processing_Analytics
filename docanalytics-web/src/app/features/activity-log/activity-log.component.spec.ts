import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivityLogComponent } from './activity-log.component';
import { ActivityLogService } from './activity-log.service';
import { SiteContextService } from '../../core/services/site-context.service';

describe('ActivityLogComponent', () => {
  const svc = {
    rows: signal([]), loading: signal(false), error: signal(null), meta: signal(null),
    query: { sortBy: 'ts', sortDir: 'desc' },
    load: vi.fn(), setPage: vi.fn(), setPageSize: vi.fn(),
    setSort: vi.fn(), setFilters: vi.fn(), setSearch: vi.fn(),
  };
  const site = signal<string | null>('s1');

  beforeEach(async () => {
    Object.values(svc).forEach(v => (v as any).mockClear?.());
    await TestBed.configureTestingModule({
      imports: [ActivityLogComponent],
      providers: [
        provideHttpClient(), provideHttpClientTesting(),
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
    fixture.detectChanges();       // runs the effect once
    svc.load.mockClear();
    site.set('s2');
    TestBed.tick();
    fixture.detectChanges();
    expect(svc.load).toHaveBeenCalled();
  });
});
