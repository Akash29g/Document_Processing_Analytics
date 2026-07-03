import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-refresh-timer',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './refresh-timer.component.html',
  styleUrl: './refresh-timer.component.css',
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
