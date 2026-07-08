import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FilterBarComponent } from './filter-bar.component';

describe('FilterBarComponent', () => {
  let fixture: ComponentFixture<FilterBarComponent>;
  let comp: FilterBarComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [FilterBarComponent] }).compileComponents();
    fixture = TestBed.createComponent(FilterBarComponent);
    comp = fixture.componentInstance;
    fixture.detectChanges();
  });

  const evt = (value: string) => ({ target: { value } } as unknown as Event);

  it('emits changed on status select', () => {
    const spy = vi.fn(); comp.changed.subscribe(spy);
    comp.onStatus(evt('failed'));
    expect(spy).toHaveBeenCalledWith(expect.objectContaining({ status: 'failed' }));
  });

  it('isDirty() is false initially and true after a change', () => {
    expect(comp.isDirty()).toBe(false);
    comp.onStatus(evt('completed'));
    expect(comp.isDirty()).toBe(true);
  });

  it('clear() resets filters and emits', () => {
    const spy = vi.fn(); comp.changed.subscribe(spy);
    comp.onStatus(evt('failed'));
    comp.clear();
    expect(comp.isDirty()).toBe(false);
    expect(spy).toHaveBeenLastCalledWith(expect.objectContaining({ status: 'all', source: null }));
  });

  it('respects a custom statusLabel input', () => {
    fixture.componentRef.setInput('statusLabel', 'Step');
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Step');
  });
});
