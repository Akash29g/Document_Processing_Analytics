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
    .btn { display:inline-flex; align-items:center; gap:8px; background:var(--purple-500);
           color:#fff; border:none; border-radius:8px; padding:9px 16px;
           font-size:14px; font-weight:600; cursor:pointer; }
    .btn:hover:not(:disabled) { background:var(--purple-700); }
    .btn:disabled { opacity:.6; cursor:not-allowed; }
    .spinner { width:14px; height:14px; border:2px solid rgba(255,255,255,.4);
               border-top-color:#fff; border-radius:50%; animation:spin .6s linear infinite; }
    @keyframes spin { to { transform:rotate(360deg); } }
  `],
})
export class AppButtonComponent {
  loading = input(false);
  disabled = input(false);
  clicked = output<void>();
}
