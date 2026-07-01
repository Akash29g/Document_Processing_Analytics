import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';

export interface FilterOption { value: string; label: string; }

export interface FilterValues {
  status: string;         // all | in_progress | completed | failed
  source: string | null;  // null = all sources
  from: string | null;    // 'YYYY-MM-DD' or null
  to: string | null;      // 'YYYY-MM-DD' or null
}

@Component({
  selector: 'app-filter-bar',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="fb">
      <label class="fb-field">
        <span class="fb-label">Status</span>
        <select class="fb-input" [value]="status()" (change)="onStatus($event)">
          @for (o of statusOptions(); track o.value) {
            <option [value]="o.value">{{ o.label }}</option>
          }
        </select>
      </label>

      @if (showSource()) {
        <label class="fb-field">
          <span class="fb-label">Source</span>
          <select class="fb-input" [value]="source() ?? ''" (change)="onSource($event)">
            <option value="">All sources</option>
            @for (o of sourceOptions(); track o.value) {
              <option [value]="o.value">{{ o.label }}</option>
            }
          </select>
        </label>
      }

      @if (showDateRange()) {
        <label class="fb-field">
          <span class="fb-label">From</span>
          <input class="fb-input" type="date" [value]="from() ?? ''" (change)="onFrom($event)" />
        </label>
        <label class="fb-field">
          <span class="fb-label">To</span>
          <input class="fb-input" type="date" [value]="to() ?? ''" (change)="onTo($event)" />
        </label>
      }

      <button class="fb-clear" type="button" (click)="clear()" [disabled]="!isDirty()">Clear</button>
    </div>
  `,
  styles: [`
    .fb { display: flex; flex-wrap: wrap; align-items: flex-end; gap: var(--space-2);
          padding: var(--space-2); background: var(--white);
          border: 1px solid var(--cool-gray); border-radius: 8px; }
    .fb-field { display: flex; flex-direction: column; gap: 4px; }
    .fb-label { font-size: 0.72rem; color: var(--dark-gray-3); text-transform: uppercase; letter-spacing: .04em; }
    .fb-input { height: 34px; padding: 0 8px; font: inherit; color: var(--dark-gray);
                border: 1px solid var(--cool-gray); border-radius: 6px; background: var(--white); }
    .fb-input:focus { outline: none; border-color: var(--slate-blue); }
    .fb-clear { height: 34px; margin-left: auto; padding: 0 14px; cursor: pointer;
                border: 1px solid var(--cool-gray); border-radius: 6px; background: var(--bg-light);
                color: var(--dark-gray-3); }
    .fb-clear:disabled { opacity: .5; cursor: default; }
  `],
})
export class FilterBarComponent {
  // config — Batches shows all three; Errors/Activity-Log can hide source/date as needed
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

  isDirty = computed(() =>
    this._status() !== 'all' || !!this._source() || !!this._from() || !!this._to());

  changed = output<FilterValues>();

  private emit(): void {
    this.changed.emit({
      status: this._status(), source: this._source(),
      from: this._from(), to: this._to(),
    });
  }
  onStatus(e: Event) { this._status.set((e.target as HTMLSelectElement).value); this.emit(); }
  onSource(e: Event) { const v = (e.target as HTMLSelectElement).value; this._source.set(v || null); this.emit(); }
  onFrom(e: Event) { const v = (e.target as HTMLInputElement).value; this._from.set(v || null); this.emit(); }
  onTo(e: Event) { const v = (e.target as HTMLInputElement).value; this._to.set(v || null); this.emit(); }
  clear() { this._status.set('all'); this._source.set(null); this._from.set(null); this._to.set(null); this.emit(); }
}
