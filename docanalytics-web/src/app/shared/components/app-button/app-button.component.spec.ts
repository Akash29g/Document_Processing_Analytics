import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppButtonComponent } from './app-button.component';

describe('AppButtonComponent', () => {
  let fixture: ComponentFixture<AppButtonComponent>;
  let el: HTMLElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppButtonComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AppButtonComponent);
    el = fixture.nativeElement as HTMLElement;
  });

  const button = () => el.querySelector('button.btn') as HTMLButtonElement;

  it('should create', () => {
    fixture.detectChanges();
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should not be disabled by default', () => {
    fixture.detectChanges();
    expect(button().disabled).toBe(false);
  });

  it('should be disabled when [disabled]=true', () => {
    fixture.componentRef.setInput('disabled', true);
    fixture.detectChanges();
    expect(button().disabled).toBe(true);
  });

  it('should be disabled when [loading]=true', () => {
    fixture.componentRef.setInput('loading', true);
    fixture.detectChanges();
    expect(button().disabled).toBe(true);
  });

  it('should show a spinner when [loading]=true', () => {
    fixture.componentRef.setInput('loading', true);
    fixture.detectChanges();
    expect(el.querySelector('.spinner')).not.toBeNull();
  });

  it('should NOT show a spinner when not loading', () => {
    fixture.componentRef.setInput('loading', false);
    fixture.detectChanges();
    expect(el.querySelector('.spinner')).toBeNull();
  });

  it('should emit clicked when the button is clicked', () => {
    fixture.detectChanges();
    const spy = vi.fn();
    fixture.componentInstance.clicked.subscribe(spy);

    button().click();

    expect(spy).toHaveBeenCalledTimes(1);
  });
});
