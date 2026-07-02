import { Component, computed, input } from '@angular/core';

type BadgeStyle = { bg: string; fg: string; icon: string };

@Component({
  selector: 'app-status-badge',
  standalone: true,
  template: `
    <span class="badge" [style.background]="style().bg" [style.color]="style().fg">
      <span class="material-icons" aria-hidden="true">{{ style().icon }}</span>
      {{ status() }}
    </span>
  `,
  styles: [`
    .badge {
      display: inline-flex; align-items: center; gap: 6px;
      padding: 3px 10px; border-radius: 999px;
      font-family: var(--font-body); font-size: 12px; font-weight: 600;
      line-height: 1.6; white-space: nowrap;
    }
    .material-icons { font-size: 14px; line-height: 1; }
  `],
})
export class StatusBadgeComponent {
  status = input.required<string>();

  private key = computed(() => this.status().toLowerCase().replace(/[\s_]/g, ''));

  style = computed<BadgeStyle>(() => {
    switch (this.key()) {
      case 'completed':
      case 'success':
        // confirmed: tint fill + muted green text + check icon
        return { bg: 'rgba(0,152,72,.12)', fg: 'var(--text-confirmed)', icon: 'check_circle' };
      case 'failed':
      case 'error':
        return { bg: 'rgba(220,10,10,.12)', fg: 'var(--text-error)', icon: 'error' };
      case 'inprogress':
      case 'processing':
        // warning amber (token fg auto-flips: dark-brown in light, bright-amber in dark)
        return { bg: 'rgba(245,166,36,.15)', fg: 'var(--text-warning)', icon: 'pause_circle' };
      case 'queued':
      default:
        return { bg: 'rgba(190,204,214,.25)', fg: 'var(--dark-gray-3)', icon: 'schedule' };
    }
  });
}
