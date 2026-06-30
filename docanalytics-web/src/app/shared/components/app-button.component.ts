import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  template: `
    <button class="btn" [disabled]="disabled() || loading()" (click)="clicked.emit()">
      @if (loading()) { <span class="spinner"></span> }
      <ng-content />
    </button>`,
  styles: [`
    .btn {
      display: inline-flex; align-items: center; gap: 8px;
      font-family: var(--font-display); font-weight: 600; font-size: 18px;
      background: var(--slate-blue); color: #fff;
      border: none; border-radius: 4px; padding: 13px 32px; cursor: pointer;
    }
    .btn:hover:not(:disabled) { background: #3f4fc4; }   /* one shade darker */
    .btn:disabled { background: var(--cool-gray); color: #fff; cursor: not-allowed; }
    .spinner {
      width: 14px; height: 14px;
      border: 2px solid rgba(255,255,255,.4); border-top-color: #fff;
      border-radius: 50%; animation: spin .6s linear infinite reverse;  /* counter-clockwise */
    }
    @media (max-width: 1024px) { .btn { font-size: 14px; } }
    @keyframes spin { to { transform: rotate(360deg); } }
  `],
})
export class AppButtonComponent {
  loading = input(false);
  disabled = input(false);
  clicked = output<void>();
}
