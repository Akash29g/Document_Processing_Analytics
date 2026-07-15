import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './stat-card.component.html',
  styleUrl: './stat-card.component.css',
})
export class StatCardComponent {
  title = input.required<string>();
  value = input<string | number>(''); // was input.required — now optional for skeleton use
  loading = input<boolean>(false); // NEW
}
