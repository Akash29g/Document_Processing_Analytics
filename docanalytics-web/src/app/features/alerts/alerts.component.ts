import {
  ChangeDetectionStrategy, Component, ElementRef, HostListener,
  computed, effect, inject, signal, untracked,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { AlertsService } from './alerts.service';
import { AlertRulePayload } from './alerts.models';
import { SiteContextService } from '../../core/services/site-context.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-alerts',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, DatePipe],
  templateUrl: './alerts.component.html',
  styleUrl: './alerts.component.css',
})
export class AlertsComponent {
  protected svc = inject(AlertsService);
  private site = inject(SiteContextService);
  private auth = inject(AuthService);
  private host = inject(ElementRef);

  protected isAdmin = computed(() =>
    (this.auth.currentUser()?.role ?? '').toLowerCase() === 'admin');  // ← keep your working signal name

  // scalar form fields (template-driven ngModel)
  protected form = this.blank();

  // recipient multi-select state
  protected selected = signal<string[]>([]);
  protected menuOpen = signal(false);
  protected selectedLabel = computed(() => {
    const n = this.selected().length;
    return n === 0 ? 'Select recipients' : `${n} recipient${n > 1 ? 's' : ''} selected`;
  });

  // split the stored "a@x,b@x" into individual addresses
  protected recipientsOf(email: string | null | undefined): string[] {
    return (email ?? '').split(',').map((e) => e.trim()).filter(Boolean);
  }

  // "user.a@acme.com" → "user.a"
  protected shortName(email: string): string {
    return email.split('@')[0];
  }

  // email → role map, built from the site's recipient list
  private roleByEmail = computed(() => {
    const map = new Map<string, string>();
    for (const r of this.svc.recipients()) map.set(r.email.toLowerCase(), r.role);
    return map;
  });

  // split → tag admins → sort admins first (order otherwise preserved)
  protected recipientChips(email: string | null | undefined) {
    const roles = this.roleByEmail();
    return this.recipientsOf(email)
      .map((e) => ({
        email: e,
        name: this.shortName(e),
        isAdmin: (roles.get(e.toLowerCase()) ?? '').toLowerCase() === 'admin',
      }))
      .sort((a, b) => Number(b.isAdmin) - Number(a.isAdmin));   // admins to the front
  }


  constructor() {
    effect(() => {
      const s = this.site.selectedSiteId();
      if (!s) return;
      untracked(() => {
        this.svc.loadRules();
        this.svc.loadRecipients();   // ← load the dropdown options for this site
        this.selected.set([]);       // reset picks when site changes
      });
    });
  }

  protected toggleMenu(): void { this.menuOpen.update(v => !v); }
  protected isChecked(email: string): boolean { return this.selected().includes(email); }
  protected toggleEmail(email: string): void {
    this.selected.update(list =>
      list.includes(email) ? list.filter(e => e !== email) : [...list, email]);
  }

  protected submit(): void {
    if (!this.form.name || this.selected().length === 0) return;   // need a name + ≥1 recipient
    const payload: AlertRulePayload = {
      name: this.form.name,
      email: this.selected().join(','),   // ← chosen users → comma-joined email string
      threshold_percent: this.form.threshold_percent,
      window_minutes: this.form.window_minutes,
      cooldown_minutes: this.form.cooldown_minutes,
      is_enabled: true,
    };
    this.svc.create(payload);
    this.form = this.blank();
    this.selected.set([]);
    this.menuOpen.set(false);
  }

  private blank() {
    return { name: '', threshold_percent: 10, window_minutes: 60, cooldown_minutes: 60 };
  }

  // close the dropdown on outside click
  @HostListener('document:click', ['$event'])
  onDocClick(e: MouseEvent): void {
    if (this.menuOpen() && !this.host.nativeElement.contains(e.target)) this.menuOpen.set(false);
  }
}
