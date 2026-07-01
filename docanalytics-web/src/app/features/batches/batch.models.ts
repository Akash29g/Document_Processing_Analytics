import { SortDir } from '../../shared/components/data-table.component';

// Matches BatchListItemDto exactly (snake_case JSON via your global naming policy).
export interface BatchListItem {
  transaction_id: string;
  state: string;                 // Processing | Completed | Failed
  source_system: string;
  total_files: number;
  uploaded_count: number;
  processing_count: number;
  failed_count: number;
  completed_count: number;
  submitted_at: string;
  last_updated_at: string;
  completed_at: string | null;
}

// Must match backend ApplySorting whitelist: submitted_at | state | source_system | total_files | last_updated
export type BatchSortBy = 'last_updated' | 'submitted_at' | 'state' | 'source_system' | 'total_files';

export interface BatchListQuery {
  page: number;
  pageSize: number;
  status: string;         // FILTER value: all | in_progress | completed | failed  (backend maps → state)
  source: string | null;
  from: string | null;
  to: string | null;
  search: string | null;
  sortBy: BatchSortBy;
  sortDir: SortDir;
}
