import { SortDir } from '../../shared/components/data-table/data-table.component';

// ── Batch List (Dev A · FR-2.1–2.3) ──
export interface BatchListItem {
  transaction_id: string;
  state: string; // Processing | Completed | Failed
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
export type BatchSortBy =
  'last_updated' | 'submitted_at' | 'state' | 'source_system' | 'total_files';
export interface BatchListQuery {
  page: number;
  pageSize: number;
  status: string;
  source: string | null;
  from: string | null;
  to: string | null;
  search: string | null;
  sortBy: BatchSortBy;
  sortDir: SortDir;
}

// ── Batch Detail + Files (Dev B · FR-2.4) ──
export interface BatchFileStats {
  uploaded: number;
  processing: number;
  failed: number;
  completed: number;
}
export interface BatchTimes {
  submitted_at: string;
  last_updated_at: string;
  completed_at: string | null;
}
export interface BatchDetail {
  id: string;
  status: string;
  source: string;
  total_files: number;
  file_stats: BatchFileStats;
  times: BatchTimes;
}
export interface BatchFile {
  id: string;
  file_name: string;
  file_type: string;
  status: string;
  current_step: string | null;
  file_size_bytes: number;
  created_at: string;
  last_updated_at: string;
}
export interface FilesQuery {
  page: number;
  pageSize: number;
}
