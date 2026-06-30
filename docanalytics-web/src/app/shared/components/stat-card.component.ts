import { Component, input } from '@angular/core';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  template: `
    <div class="card">
      <div class="title">{{ title() }}</div>
      <div class="value">{{ value() }}</div>
    </div>`,
  styles: [`
    .card {
      background: var(--white); border: 1px solid var(--cool-gray);
      border-radius: 6px; padding: 16px 18px;
      box-shadow: 0 2px 10px 0 rgba(0,0,0,.08); min-width: 160px;
    }
    .title {
      font-family: var(--font-body); color: var(--dark-gray-3);
      font-size: 12px; font-weight: 600; line-height: 16px;
      text-transform: uppercase; letter-spacing: 2px;   /* Eyebrow Title spec */
    }
    .value {
      font-family: var(--font-body); color: var(--dark-gray);
      font-size: 32px; line-height: 38px; font-weight: 700;   /* Number level one */
      margin-top: 8px;
    }
  `],
})
export class StatCardComponent {
  title = input.required<string>();
  value = input.required<string | number>();
}
