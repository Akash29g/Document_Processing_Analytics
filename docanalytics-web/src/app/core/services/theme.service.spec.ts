import { TestBed } from '@angular/core/testing';
import { ThemeService } from './theme.service';

describe('ThemeService', () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute('data-theme');
  });

  it('should default to a valid theme on init', () => {
    const service = TestBed.inject(ThemeService);
    expect(['light', 'dark']).toContain(service.theme());
  });

  it('should read a saved theme from localStorage on init', () => {
    localStorage.setItem('da_theme', 'dark');
    const service = TestBed.inject(ThemeService);
    expect(service.theme()).toBe('dark');
  });

  it('should toggle light -> dark', () => {
    const service = TestBed.inject(ThemeService);
    service.set('light');
    service.toggle();
    expect(service.theme()).toBe('dark');
  });

  it('should toggle dark -> light', () => {
    const service = TestBed.inject(ThemeService);
    service.set('dark');
    service.toggle();
    expect(service.theme()).toBe('light');
  });

  it('isDark computed should track the theme', () => {
    const service = TestBed.inject(ThemeService);
    service.set('dark');
    expect(service.isDark()).toBe(true);
    service.set('light');
    expect(service.isDark()).toBe(false);
  });

  it('should persist theme to localStorage (via effect)', () => {
    const service = TestBed.inject(ThemeService);
    service.set('dark');
    TestBed.tick();
    expect(localStorage.getItem('da_theme')).toBe('dark');
  });
});
