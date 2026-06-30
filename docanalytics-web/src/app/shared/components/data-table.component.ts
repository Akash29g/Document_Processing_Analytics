import {
  Component, Directive, TemplateRef, computed, contentChildren, inject, input, output,
} from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';

export type SortDir = 'asc' | 'desc';
export interface SortState { sortBy: string; sortDir: SortDir; }

export interface ColumnDef<T = any> {
  key: string;                 // property name OR a unique key when you use a cell template
  header: string;
  sortable?: boolean;
  align?: 'left' | 'right' | 'center';
  width?: string;              // e.g. '160px'
  value?: (row: T) => unknown; // optional accessor for computed/nested values
}

/** Override any column's cell:  <ng-template dtCell="error_message" let-row>...</ng-template> */
@Directive({ selector: 'ng-template[dtCell]' })
export class DtCellDirective {
  readonly dtCell = input.required<string>();
  readonly template = inject(TemplateRef);
}

@Component({
  selector: 'app-data-table',
  imports: [NgTemplateOutlet],
  template: `
    <div class="dt">
      <table>
        <thead>
          <tr>
            @for (col of columns(); track col.key) {
              <th
                [style.width]="col.width"
                [class.sortable]="col.sortable"
                [style.text-align]="col.align || 'left'"
                (click)="onHeaderClick(col)">
                <span>{{ col.header }}</span>
                @if (col.sortable && sortBy() === col.key) {
                  <span class="arrow">{{ sortDir() === 'asc' ? '▲' : '▼' }}</span>
                }
              </th>
            }
          </tr>
        </thead>

        <tbody>
          @if (error()) {
            <tr><td [attr.colspan]="columns().length" class="state error">
              <span>{{ error() }}</span>
              <button type="button" class="retry" (click)="retry.emit()">Retry</button>
            </td></tr>
          } @else if (loading()) {
            @for (r of skeletonRows; track r) {
              <tr class="skeleton">
                @for (col of columns(); track col.key) { <td><span class="bar"></span></td> }
              </tr>
            }
          } @else if (!rows().length) {
            <tr><td [attr.colspan]="columns().length" class="state empty">{{ emptyMessage() }}</td></tr>
          } @else {
            @for (row of rows(); track key(row, $index)) {
              <tr (click)="rowClick.emit(row)" [class.clickable]="clickable()">
                @for (col of columns(); track col.key) {
                  <td [style.text-align]="col.align || 'left'">
                    @if (cellTemplates().get(col.key); as tpl) {
                      <ng-container
                        [ngTemplateOutlet]="tpl"
                        [ngTemplateOutletContext]="{ $implicit: row, row }" />
                    } @else {
                      {{ display(row, col) }}
                    }
                  </td>
                }
              </tr>
            }
          }
        </tbody>
      </table>

      @if (!loading() && !error() && rows().length) {
        <div class="dt-footer">
          <span class="count">{{ totalCount() }} records</span>
          <div class="pager">
            <label class="psize">
              Rows:
              <select [value]="pageSize()" (change)="onPageSize($event)">
                @for (n of pageSizeOptions(); track n) { <option [value]="n">{{ n }}</option> }
              </select>
            </label>
            <button type="button" [disabled]="page() <= 1" (click)="prev()">‹ Prev</button>
            <span class="page-info">Page {{ page() }} of {{ totalPages() }}</span>
            <button type="button" [disabled]="page() >= totalPages()" (click)="next()">Next ›</button>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .dt { background: var(--white); border: 1px solid var(--cool-gray);
          border-radius: 8px; overflow: hidden; }
    table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
    thead th { text-align: left; font-family: var(--font-display);
               font-weight: 600; color: var(--dark-gray-3); background: var(--bg-light);
               padding: 10px 14px; border-bottom: 1px solid var(--cool-gray);
               white-space: nowrap; user-select: none; }
    th.sortable { cursor: pointer; }
    th.sortable:hover { color: var(--slate-blue); }
    .arrow { margin-left: 4px; font-size: 0.7rem; color: var(--slate-blue); }
    tbody td { padding: 10px 14px; border-bottom: 1px solid var(--bg-light);
               color: var(--dark-gray); vertical-align: top; }
    tbody tr.clickable { cursor: pointer; }
    tbody tr.clickable:hover { background: var(--bg-light); }
    .state { text-align: center; padding: 28px 14px; color: var(--dark-gray-3); }
    .state.error { color: var(--text-error); }
    .retry { margin-left: 10px; border: 1px solid var(--slate-blue); background: transparent;
             color: var(--slate-blue); border-radius: 6px; padding: 4px 12px; cursor: pointer; }
    .skeleton .bar { display: block; height: 12px; border-radius: 4px;
                     background: linear-gradient(90deg, var(--bg-light), var(--cool-gray), var(--bg-light));
                     background-size: 200% 100%; animation: dt-shimmer 1.2s infinite; }
    @keyframes dt-shimmer { to { background-position: -200% 0; } }
    .dt-footer { display: flex; align-items: center; justify-content: space-between;
                 padding: 10px 14px; background: var(--bg-light); font-size: 0.8rem;
                 color: var(--dark-gray-3); }
    .pager { display: flex; align-items: center; gap: 10px; }
    .pager button { border: 1px solid var(--cool-gray); background: var(--white);
                    border-radius: 6px; padding: 4px 10px; cursor: pointer; color: var(--dark-gray); }
    .pager button:disabled { opacity: 0.45; cursor: default; }
    .psize select { margin-left: 4px; border: 1px solid var(--cool-gray);
                    border-radius: 6px; padding: 2px 4px; }
  `],
})
export class DataTableComponent<T = any> {
  readonly columns = input.required<ColumnDef<T>[]>();
  readonly rows = input<T[]>([]);
  readonly loading = input(false);
  readonly error = input<string | null>(null);
  readonly emptyMessage = input('No records to display.');

  readonly sortBy = input<string | null>(null);
  readonly sortDir = input<SortDir>('desc');

  readonly page = input(1);
  readonly pageSize = input(10);
  readonly totalCount = input(0);
  readonly totalPages = input(1);
  readonly pageSizeOptions = input<number[]>([10, 20, 50]);

  readonly clickable = input(false);
  readonly rowId = input<((row: T) => string | number) | null>(null);

  readonly sortChange = output<SortState>();
  readonly pageChange = output<number>();
  readonly pageSizeChange = output<number>();
  readonly retry = output<void>();
  readonly rowClick = output<T>();

  protected readonly skeletonRows = [0, 1, 2, 3, 4];

  private readonly cells = contentChildren(DtCellDirective);
  protected readonly cellTemplates = computed(() => {
    const map = new Map<string, TemplateRef<any>>();
    for (const c of this.cells()) map.set(c.dtCell(), c.template);
    return map;
  });

  protected display(row: T, col: ColumnDef<T>): unknown {
    return col.value ? col.value(row) : (row as any)[col.key];
  }
  protected key(row: T, i: number): string | number {
    const fn = this.rowId();
    return fn ? fn(row) : i;
  }
  protected onHeaderClick(col: ColumnDef<T>): void {
    if (!col.sortable) return;
    const same = this.sortBy() === col.key;
    const dir: SortDir = same ? (this.sortDir() === 'asc' ? 'desc' : 'asc') : 'desc';
    this.sortChange.emit({ sortBy: col.key, sortDir: dir });
  }
  protected prev(): void { if (this.page() > 1) this.pageChange.emit(this.page() - 1); }
  protected next(): void { if (this.page() < this.totalPages()) this.pageChange.emit(this.page() + 1); }
  protected onPageSize(e: Event): void {
    this.pageSizeChange.emit(Number((e.target as HTMLSelectElement).value));
  }
}
