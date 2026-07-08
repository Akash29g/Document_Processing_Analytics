import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StatCardComponent } from './stat-card.component';

describe('StatCardComponent', () => {
  let fixture: ComponentFixture<StatCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatCardComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(StatCardComponent);
  });

  it('should create with a title', () => {
    fixture.componentRef.setInput('title', 'Completed');
    fixture.detectChanges();
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('value defaults to empty string', () => {
    fixture.componentRef.setInput('title', 'Completed');
    expect(fixture.componentInstance.value()).toBe('');
  });

  it('loading defaults to false', () => {
    fixture.componentRef.setInput('title', 'Completed');
    expect(fixture.componentInstance.loading()).toBe(false);
  });

  it('renders the title in the DOM', () => {
    fixture.componentRef.setInput('title', 'Completed');
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Completed');
  });

  it('renders the value when not loading', () => {
    fixture.componentRef.setInput('title', 'Completed');
    fixture.componentRef.setInput('value', 42);
    fixture.componentRef.setInput('loading', false);
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('42');
  });
});
