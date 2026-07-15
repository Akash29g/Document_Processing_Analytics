import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

@Component({
  selector: 'app-chart-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './chart-card.component.html',
  styleUrl: './chart-card.component.css',
})
export class ChartCardComponent {
  title = input.required<string>();
  subtitle = input<string>('');
  loading = input<boolean>(false);
  error = input<string | null>(null);
  empty = input<boolean>(false);
  emptyMessage = input<string>('No data to display');
  retry = output<void>(); // NEW
}
