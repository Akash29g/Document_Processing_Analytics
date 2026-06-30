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
    .card { background:var(--white); border:1px solid var(--line);
            border-left:4px solid var(--purple-500); border-radius:12px;
            padding:16px 18px; box-shadow:0 1px 3px rgba(61,17,82,.08); min-width:160px; }
    .title { color:var(--muted); font-size:13px; font-weight:600;
             text-transform:uppercase; letter-spacing:.03em; }
    .value { color:var(--purple-900); font-size:30px; font-weight:700; margin-top:6px; }
  `],
})
export class StatCardComponent {
  title = input.required<string>();
  value = input.required<string | number>();
}
