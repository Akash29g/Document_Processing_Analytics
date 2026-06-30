import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div class="dash">
      <div class="panel">
        <h1>Dashboard</h1>
        <p class="muted">Coming in Round 2 — summary counters, throughput &amp; status charts, and the recent-failures table.</p>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .dash { display: flex; flex-direction: column; gap: 16px; }
    .panel {
      background: #fff;
      border: 1px solid var(--purple-200);
      border-radius: 12px;
      padding: 24px;
      box-shadow: 0 4px 14px rgba(61,17,82,.06);
    }
    .panel h1 { margin: 0 0 6px; color: var(--purple-900); }
    .muted { margin: 0; color: var(--muted); }
  `],
})
export class DashboardComponent { }
