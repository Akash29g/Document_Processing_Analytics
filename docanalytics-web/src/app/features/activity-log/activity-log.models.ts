import { SortDir } from '../../shared/components/data-table/data-table.component';

// GET /api/v1/activity-log item — matches ActivityLogItemDto (snake_case JSON)
export interface ActivityLogItem {
  ts: string; // ISO-8601 (CreatedAt)
  event_type: string; // FILE_STATE_CHANGED | BATCH_SUBMITTED | BATCH_COMPLETED | BATCH_FAILED
  entity_type: string; // File | Batch
  entity: string | null; // EntityName (file name / "Batch xxxxxxxx")
  entity_id?: string; // present in DTO; unused in the table (kept optional)
  old_state: string | null; // null on BATCH_SUBMITTED
  new_state: string | null;
  actor: string; // TriggeredBy (e.g. "system")
}

// DataTable column keys MUST equal backend sort tokens (ApplySorting whitelist: ts|event_type|entity)
export type ActivityLogSortBy = 'ts' | 'event_type' | 'entity';

export interface ActivityLogQuery {
  page: number;
  pageSize: number;
  eventType: string | null; // null = all (exact match)
  entityType: string | null; // null = all (exact match — reserved, backend supports it)
  entity: string | null; // partial ILIKE match on entity_name
  from: string | null;
  to: string | null;
  sortBy: ActivityLogSortBy;
  sortDir: SortDir;
}
