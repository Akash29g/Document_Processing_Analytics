import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-refresh-timer',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="rt">
      <span class="stamp">{{ stampText() }}</span>
      <button type="button" class="btn" (click)="refresh.emit()" [disabled]="busy()">
        <span class="material-icons" aria-hidden="true">refresh</span>
        Refresh
      </button>
    </div>
  `,
  styles: [`
    .rt { display: flex; align-items: center; gap: var(--space-1); }
    .stamp { font-size: 0.78rem; color: var(--dark-gray-3); }
    .btn { display: inline-flex; align-items: center; gap: 4px; font-family: var(--font-display);
      font-size: 0.8rem; font-weight: 600; color: var(--slate-blue); background: transparent;
      border: 1px solid var(--slate-blue); border-radius: 6px; padding: 4px 10px; cursor: pointer; }
    .btn:hover:not(:disabled) { background: var(--slate-blue); color: var(--white); }
    .btn:disabled { color: var(--cool-gray); border-color: var(--cool-gray); cursor: default; }
    .material-icons { font-size: 16px; }
  `]
})
export class RefreshTimerComponent {
  lastUpdated = input<Date | null>(null);
  intervalMs = input<number>(30_000);
  busy = input<boolean>(false);
  refresh = output<void>();

  private now = signal(Date.now());

  constructor() {
    const id = setInterval(() => this.now.set(Date.now()), 1000); // 1s ticker for the countdown
    inject(DestroyRef).onDestroy(() => clearInterval(id));
  }

  secondsLeft = computed(() => {
    const lu = this.lastUpdated();
    if (!lu) return null;
    const left = Math.ceil((this.intervalMs() - (this.now() - lu.getTime())) / 1000);
    return Math.max(0, left);
  });

  stampText = computed(() => {
    if (this.busy()) return 'Refreshing…';
    const s = this.secondsLeft();
    return s === null ? '' : `Refreshing in ${s}s`;
  });
}
