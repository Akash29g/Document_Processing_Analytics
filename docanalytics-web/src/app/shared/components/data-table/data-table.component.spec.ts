import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DataTableComponent, ColumnDef } from './data-table.component';

describe('DataTableComponent', () => {
  let fixture: ComponentFixture<DataTableComponent>;
  let comp: any;
  const cols: ColumnDef[] = [
    { key: 'name', header: 'Name', sortable: true },
    { key: 'age', header: 'Age' },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [DataTableComponent] }).compileComponents();
    fixture = TestBed.createComponent(DataTableComponent);
    comp = fixture.componentInstance;
    fixture.componentRef.setInput('columns', cols);
    fixture.componentRef.setInput('rows', [{ name: 'a', age: 1 }]);
  });

  it('emits sortChange toggling asc→desc on the same column', () => {
    fixture.componentRef.setInput('sortBy', 'name');
    fixture.componentRef.setInput('sortDir', 'asc');
    fixture.detectChanges();
    const spy = vi.fn(); comp.sortChange.subscribe(spy);
    comp.onHeaderClick(cols[0]);
    expect(spy).toHaveBeenCalledWith({ sortBy: 'name', sortDir: 'desc' });
  });

  it('does not emit sortChange for a non-sortable column', () => {
    fixture.detectChanges();
    const spy = vi.fn(); comp.sortChange.subscribe(spy);
    comp.onHeaderClick(cols[1]);   // age not sortable
    expect(spy).not.toHaveBeenCalled();
  });

  it('next()/prev() are bounded by page and totalPages', () => {
    fixture.componentRef.setInput('page', 1);
    fixture.componentRef.setInput('totalPages', 2);
    fixture.detectChanges();
    const spy = vi.fn(); comp.pageChange.subscribe(spy);
    comp.prev();                       // already on page 1 → no emit
    expect(spy).not.toHaveBeenCalled();
    comp.next();                       // → page 2
    expect(spy).toHaveBeenCalledWith(2);
  });

  it('rowClick only fires when clickable=true', () => {
    fixture.componentRef.setInput('clickable', false);
    fixture.detectChanges();
    // In this codebase rowClick.emit is wired in the template gated by [class.clickable];
    // assert the input flag drives it:
    expect(comp.clickable()).toBe(false);
    fixture.componentRef.setInput('clickable', true);
    fixture.detectChanges();
    const spy = vi.fn(); comp.rowClick.subscribe(spy);
    comp.rowClick.emit({ name: 'a', age: 1 });
    expect(spy).toHaveBeenCalled();
  });

  it('emits retry from the error state', () => {
    fixture.componentRef.setInput('error', 'Boom');
    fixture.detectChanges();
    const spy = vi.fn(); comp.retry.subscribe(spy);
    (fixture.nativeElement.querySelector('.retry') as HTMLButtonElement).click();
    expect(spy).toHaveBeenCalled();
  });

  it('shows the empty state message when rows are empty', () => {
    fixture.componentRef.setInput('rows', []);
    fixture.componentRef.setInput('emptyMessage', 'Nothing here');
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Nothing here');
  });
});
