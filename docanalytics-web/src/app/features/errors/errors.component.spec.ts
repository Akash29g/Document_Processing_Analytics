import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ErrorsComponent } from './errors.component';
import { signal } from '@angular/core';
import { provideRouter, Router } from '@angular/router';
import { SiteContextService } from '../../core/services/site-context.service';

describe('ErrorsComponent (helpers)', () => {
  let comp: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ErrorsComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]), // ← add
        {
          provide: SiteContextService, // ← add
          useValue: { selectedSiteId: signal<string | null>('s1') },
        },
      ],
    }).compileComponents();
    // no detectChanges → template not rendered; site is null so the effect is a no-op
    comp = TestBed.createComponent(ErrorsComponent).componentInstance;
  });

  it('pct() rounds a value/max to a percentage', () => {
    expect(comp.pct(50, 100)).toBe(50);
    expect(comp.pct(1, 3)).toBe(33);
    expect(comp.pct(2, 3)).toBe(67);
  });

  it('shortDate() slices YYYY- off an ISO date → MM-DD', () => {
    expect(comp.shortDate('2026-07-08')).toBe('07-08');
  });

  it('shortDate() returns short labels unchanged', () => {
    expect(comp.shortDate('abc')).toBe('abc');
  });

  it('navigateToFile returns early when batch_id is null', () => {
    expect(() =>
      comp.navigateToFile({ file_id: 'abc', file_name: 'test.pdf', batch_id: null }),
    ).not.toThrow();
  });

  it('navigateToFile returns early when batch_id is empty', () => {
    expect(() =>
      comp.navigateToFile({ file_id: 'abc', file_name: 'test.pdf', batch_id: '' }),
    ).not.toThrow();
  });

  it('navigateToFile navigates when siteId and batch_id are valid', () => {
    const router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate').mockResolvedValue(true);
    comp.navigateToFile({ file_id: 'f1', file_name: 'f.pdf', batch_id: 'b1' });
    expect(router.navigate).toHaveBeenCalled();
  });
});
