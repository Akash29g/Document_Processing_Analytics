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

  templateUrl: './data-table.component.html',
  styleUrl: './data-table.component.css',
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
