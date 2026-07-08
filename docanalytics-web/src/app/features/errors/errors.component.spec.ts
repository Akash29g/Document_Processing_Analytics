import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ErrorsComponent } from './errors.component';

describe('ErrorsComponent (helpers)', () => {
  let comp: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ErrorsComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
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
});
