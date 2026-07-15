import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';

export interface FilterOption {
  value: string;
  label: string;
}

export interface FilterValues {
  status: string; // all | in_progress | completed | failed
  source: string | null; // null = all sources
  from: string | null; // 'YYYY-MM-DD' or null
  to: string | null; // 'YYYY-MM-DD' or null
}

@Component({
  selector: 'app-filter-bar',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,

  templateUrl: './filter-bar.component.html',
  styleUrl: './filter-bar.component.css',
})
export class FilterBarComponent {
  statusLabel = input<string>('Status');
  statusOptions = input<FilterOption[]>([
    { value: 'all', label: 'All statuses' },
    { value: 'in_progress', label: 'In Progress' },
    { value: 'completed', label: 'Completed' },
    { value: 'failed', label: 'Failed' },
  ]);
  sourceOptions = input<FilterOption[]>([]);
  showSource = input<boolean>(true);
  showDateRange = input<boolean>(true);

  private _status = signal('all');
  private _source = signal<string | null>(null);
  private _from = signal<string | null>(null);
  private _to = signal<string | null>(null);

  status = this._status.asReadonly();
  source = this._source.asReadonly();
  from = this._from.asReadonly();
  to = this._to.asReadonly();

  isDirty = computed(
    () => this._status() !== 'all' || !!this._source() || !!this._from() || !!this._to(),
  );

  changed = output<FilterValues>();

  private emit(): void {
    this.changed.emit({
      status: this._status(),
      source: this._source(),
      from: this._from(),
      to: this._to(),
    });
  }
  onStatus(e: Event) {
    this._status.set((e.target as HTMLSelectElement).value);
    this.emit();
  }
  onSource(e: Event) {
    const v = (e.target as HTMLSelectElement).value;
    this._source.set(v || null);
    this.emit();
  }
  onFrom(e: Event) {
    const v = (e.target as HTMLInputElement).value;
    this._from.set(v || null);
    this.emit();
  }
  onTo(e: Event) {
    const v = (e.target as HTMLInputElement).value;
    this._to.set(v || null);
    this.emit();
  }
  clear() {
    this._status.set('all');
    this._source.set(null);
    this._from.set(null);
    this._to.set(null);
    this.emit();
  }
}
