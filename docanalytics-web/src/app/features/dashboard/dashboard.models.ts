// FR-1.1 — summary counters. ⚠️ VERIFY shape in Swagger (see checklist at the end):
// backend DashboardService returns a FLAT object via snake_case JSON.
export interface DashboardSummary {
  queued: number;
  in_progress: number;
  completed: number;
  failed: number;
  last_updated?: string; // present only if the backend sets it
}

// FR-1.4 — one row per failed step
export interface RecentFailure {
  file_id: string;
  file_name: string;
  failed_step: string;
  error_code?: string | null;
  error_message?: string | null;
  failed_at: string;
}

export type FailuresSortBy = 'failed_at' | 'file_name' | 'failed_step';

export interface RecentFailuresQuery {
  page: number;
  pageSize: number;
  sortBy: FailuresSortBy;
  sortDir: 'asc' | 'desc';
}
