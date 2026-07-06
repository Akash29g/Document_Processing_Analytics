import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ChartCardComponent } from '../../../shared/components/chart-card/chart-card.component';
import { StepPercentile } from '../dashboard.models';

@Component({
  selector: 'app-step-percentiles',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ChartCardComponent],
  templateUrl: './step-percentiles.component.html',
  styleUrl: './step-percentiles.component.css',
})
export class StepPercentilesComponent {
  data = input<StepPercentile[]>([]);
  loading = input(false);
  error = input<string | null>(null);
  retry = output<void>();
}
