import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ComparisonService } from './comparison.service';
import { ThroughputChartComponent } from '../dashboard/throughput-chart/throughput-chart.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-comparison',
  standalone: true,
  imports: [FormsModule, ThroughputChartComponent, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './comparison.component.html',
  styleUrl: './comparison.component.css',
})
export class ComparisonComponent {
  protected readonly svc = inject(ComparisonService);

  // two-way bound date inputs (ngModel) for each range
  protected aFrom = '';
  protected aTo = '';
  protected bFrom = '';
  protected bTo = '';

  // computed totals + delta for the insight footer
  protected readonly totalA = computed(() => this.svc.total(this.svc.rangeA().points));
  protected readonly totalB = computed(() => this.svc.total(this.svc.rangeB().points));
  protected readonly delta = computed(() => {
    const a = this.totalA();
    if (a === 0) return null; // avoid divide-by-zero → hide footer
    return Math.round(((this.totalB() - a) / a) * 100);
  });

  protected runA(): void {
    if (this.aFrom && this.aTo) this.svc.loadA(this.aFrom, this.aTo);
  }
  protected runB(): void {
    if (this.bFrom && this.bTo) this.svc.loadB(this.bFrom, this.bTo);
  }
}
