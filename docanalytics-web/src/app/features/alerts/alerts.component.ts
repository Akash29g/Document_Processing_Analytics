import { ChangeDetectionStrategy, Component, computed, effect, inject, untracked } from '@angular/core';
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

  protected isAdmin = computed(() =>
    (this.auth.currentUser()?.role ?? '').toLowerCase() === 'admin');

  // new-rule form model
  protected form: AlertRulePayload = this.blank();

  constructor() {
    // load on entry + reload on site switch (guarded, untracked — same pattern as your other pages)
    effect(() => {
      const site = this.site.selectedSiteId();
      if (!site) return;
      untracked(() => this.svc.loadRules());
    });
  }

  protected submit(): void {
    if (!this.form.name || !this.form.email) return;
    this.svc.create(this.form);
    this.form = this.blank();
  }

  private blank(): AlertRulePayload {
    return { name: '', threshold_percent: 10, window_minutes: 60, email: '', cooldown_minutes: 60, is_enabled: true };
  }
}
