import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

@Component({
  selector: 'app-chart-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="chart-card">
      <header class="cc-head">
        <div class="cc-titles">
          <h3 class="cc-title">{{ title() }}</h3>
          @if (subtitle()) { <p class="cc-sub">{{ subtitle() }}</p> }
        </div>
        <div class="cc-actions"><ng-content select="[card-actions]" /></div>
      </header>

      <div class="cc-body">
        @if (loading()) {
          <div class="cc-state"><span class="spinner"></span><span>Loading…</span></div>
        } @else if (error()) {
          <div class="cc-state cc-error">
            <span>⚠️ {{ error() }}</span>
            <button type="button" class="cc-retry" (click)="retry.emit()">Retry</button>
          </div>
        } @else if (empty()) {
          <div class="cc-state">{{ emptyMessage() }}</div>
        } @else {
          <ng-content />
        }
      </div>
    </section>
  `,
  styles: [`
    .chart-card {
      background: var(--white); border: 1px solid var(--cool-gray);
      border-radius: 8px; padding: var(--space-2); display: flex; flex-direction: column;
    }
    .cc-head {
      display: flex; align-items: flex-start; justify-content: space-between;
      gap: var(--space-1); margin-bottom: var(--space-2);
    }
    .cc-title { margin: 0; font-family: var(--font-display); font-size: 1rem; font-weight: 600; color: var(--dark-gray); }
    .cc-sub { margin: 4px 0 0; font-size: 0.78rem; color: var(--dark-gray-3); }
    .cc-body { flex: 1; min-height: 180px; display: flex; }
    .cc-state {
      flex: 1; display: flex; align-items: center; justify-content: center;
      gap: var(--space-1); color: var(--dark-gray-3); font-size: 0.85rem;
    }
    .cc-error { color: var(--text-error); flex-direction: column; }
    .cc-retry {
      font: inherit; font-size: 0.8rem; cursor: pointer;
      background: transparent; border: 1px solid var(--slate-blue);
      color: var(--slate-blue); border-radius: 4px; padding: 4px 12px;
    }
    .cc-retry:hover { background: var(--slate-blue); color: #fff; }
    .spinner {
      width: 16px; height: 16px; border: 2px solid var(--cool-gray);
      border-top-color: var(--slate-blue); border-radius: 50%; animation: cc-spin .7s linear infinite;
    }
    @keyframes cc-spin { to { transform: rotate(360deg); } }
  `]
})
export class ChartCardComponent {
  title = input.required<string>();
  subtitle = input<string>('');
  loading = input<boolean>(false);
  error = input<string | null>(null);
  empty = input<boolean>(false);
  emptyMessage = input<string>('No data to display');
  retry = output<void>();   // NEW
}
