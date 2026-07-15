import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StatusBadgeComponent } from './status-badge.component';

describe('StatusBadgeComponent', () => {
  let fixture: ComponentFixture<StatusBadgeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusBadgeComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(StatusBadgeComponent);
  });

  const styleFor = (status: string) => {
    fixture.componentRef.setInput('status', status);
    fixture.detectChanges();
    return fixture.componentInstance.style();
  };

  it('should create', () => {
    fixture.componentRef.setInput('status', 'Completed');
    fixture.detectChanges();
    expect(fixture.componentInstance).toBeTruthy();
  });

  it.each([
    ['Completed', 'check_circle', 'var(--text-confirmed)'],
    ['Success', 'check_circle', 'var(--text-confirmed)'],
    ['Failed', 'error', 'var(--text-error)'],
    ['Error', 'error', 'var(--text-error)'],
    ['Processing', 'pause_circle', 'var(--text-warning)'],
    ['In Progress', 'pause_circle', 'var(--text-warning)'], // space stripped
    ['in_progress', 'pause_circle', 'var(--text-warning)'], // underscore stripped
    ['Queued', 'schedule', 'var(--dark-gray-3)'],
  ])('maps "%s" → icon "%s"', (status, icon, fg) => {
    const style = styleFor(status);
    expect(style.icon).toBe(icon);
    expect(style.fg).toBe(fg);
  });

  it('falls back to the queued/default style for an unknown status', () => {
    const style = styleFor('banana');
    expect(style.icon).toBe('schedule');
    expect(style.fg).toBe('var(--dark-gray-3)');
  });

  it('is case- and separator-insensitive', () => {
    expect(styleFor('COMPLETED').icon).toBe('check_circle');
    expect(styleFor('  failed  ').icon).toBe('error');
  });
});
