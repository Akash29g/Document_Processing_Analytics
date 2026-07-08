import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChartCardComponent } from './chart-card.component';

describe('ChartCardComponent', () => {
  let fixture: ComponentFixture<ChartCardComponent>;
  let el: HTMLElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChartCardComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ChartCardComponent);
    el = fixture.nativeElement as HTMLElement;
    fixture.componentRef.setInput('title', 'My Chart');   // required input
  });

  it('renders the title and subtitle', () => {
    fixture.componentRef.setInput('subtitle', 'per day');
    fixture.detectChanges();
    expect(el.querySelector('.cc-title')?.textContent).toContain('My Chart');
    expect(el.querySelector('.cc-sub')?.textContent).toContain('per day');
  });

  it('shows the loading state', () => {
    fixture.componentRef.setInput('loading', true);
    fixture.detectChanges();
    expect(el.querySelector('.spinner')).not.toBeNull();
    expect(el.textContent).toContain('Loading');
  });

  it('shows the error state with the message', () => {
    fixture.componentRef.setInput('error', 'Boom happened');
    fixture.detectChanges();
    expect(el.querySelector('.cc-error')).not.toBeNull();
    expect(el.textContent).toContain('Boom happened');
  });

  it('emits retry when the retry button is clicked', () => {
    fixture.componentRef.setInput('error', 'Boom');
    fixture.detectChanges();
    const spy = vi.fn();
    fixture.componentInstance.retry.subscribe(spy);
    (el.querySelector('.cc-retry') as HTMLButtonElement).click();
    expect(spy).toHaveBeenCalledTimes(1);
  });

  it('shows the empty state with a custom message', () => {
    fixture.componentRef.setInput('empty', true);
    fixture.componentRef.setInput('emptyMessage', 'Nothing here');
    fixture.detectChanges();
    expect(el.textContent).toContain('Nothing here');
  });

  it('shows no state overlay when not loading/error/empty', () => {
    fixture.detectChanges();
    expect(el.querySelector('.cc-state')).toBeNull();
  });
});
