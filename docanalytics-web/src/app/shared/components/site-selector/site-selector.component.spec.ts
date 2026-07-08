import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { SiteContextService } from '../../../core/services/site-context.service';
import { SiteSelectorComponent } from './site-selector.component';

describe('SiteSelectorComponent', () => {
  let fixture: ComponentFixture<SiteSelectorComponent>;
  let comp: SiteSelectorComponent;

  const sitesSig = signal<{ site_id: string; site_name: string }[]>([]);
  const selectedSig = signal<string | null>('s1');
  const navSpy = vi.fn();

  beforeEach(async () => {
    sitesSig.set([
      { site_id: 's1', site_name: 'Plant One' },
      { site_id: 's2', site_name: 'Plant Two' },
    ]);
    selectedSig.set('s1');
    navSpy.mockClear();

    await TestBed.configureTestingModule({
      imports: [SiteSelectorComponent],
      providers: [
        { provide: AuthService, useValue: { sites: sitesSig } },
        { provide: SiteContextService, useValue: { selectedSiteId: selectedSig } },
        { provide: Router, useValue: { url: '/site/s1/dashboard', navigateByUrl: navSpy } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SiteSelectorComponent);
    comp = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(comp).toBeTruthy();
  });

  it('currentName reflects the selected site', () => {
    expect(comp.currentName()).toBe('Plant One');
  });

  it('currentName falls back to "Select site" when none selected', () => {
    selectedSig.set(null);
    expect(comp.currentName()).toBe('Select site');
  });

  it('toggle opens then closes the dropdown', () => {
    expect(comp.open()).toBe(false);
    comp.toggle();
    expect(comp.open()).toBe(true);
    comp.toggle();
    expect(comp.open()).toBe(false);
  });

  it('toggle sets activeIndex to the current site row', () => {
    selectedSig.set('s2');
    comp.toggle();
    expect(comp.activeIndex()).toBe(1);
  });

  it('choose navigates to the new site url and closes', () => {
    comp.choose('s2');
    expect(navSpy).toHaveBeenCalledWith('/site/s2/dashboard');
    expect(comp.open()).toBe(false);
  });

  it('choose does nothing when re-selecting the current site', () => {
    comp.choose('s1');
    expect(navSpy).not.toHaveBeenCalled();
  });
});
