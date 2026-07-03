import { Component, computed, input } from '@angular/core';

type BadgeStyle = { bg: string; fg: string; icon: string };

@Component({
  selector: 'app-status-badge',
  standalone: true,
  templateUrl: './status-badge.component.html',
  styleUrl: './status-badge.component.css',
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
