import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  template: `<span class="badge" [style.background]="bg()" [style.color]="fg()">{{ status() }}</span>`,
  styles: [`
    .badge { display:inline-block; padding:3px 10px; border-radius:999px;
             font-size:12px; font-weight:600; line-height:1.6; white-space:nowrap; }
  `],
})
export class StatusBadgeComponent {
  status = input.required<string>();

  private key = computed(() => this.status().toLowerCase().replace(/[\s_]/g, ''));

  bg = computed(() => {
    switch (this.key()) {
      case 'completed': case 'success': return 'rgba(46,158,107,.12)';
      case 'failed': case 'error': return 'rgba(214,69,80,.12)';
      case 'inprogress': case 'processing': return 'var(--purple-200)';
      case 'queued': return '#ECECEC';
      default: return '#ECECEC';
    }
  });

  fg = computed(() => {
    switch (this.key()) {
      case 'completed': case 'success': return 'var(--ok)';
      case 'failed': case 'error': return 'var(--err)';
      case 'inprogress': case 'processing': return 'var(--purple-900)';
      case 'queued': return 'var(--muted)';
      default: return 'var(--muted)';
    }
  });
}
