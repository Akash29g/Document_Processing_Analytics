import { SortDir } from '../../shared/components/data-table/data-table.component';

// top-frequencies + trend both return { data: { points: [{label,value}] } }
export interface ChartPoint { label: string; value: number; }

// GET /errors item — VERIFIED against Swagger
export interface ErrorListItem {
  file_id: string;
  file_name: string;
  error_code: string;      // e.g. ERR_TIMEOUT
  error_message: string;
  step: string;            // Validate | Transform | Load | ...
  source: string;          // S3_Bucket_Alpha | Manual_Upload | ...
  failed_at: string;       // ISO timestamp
  suggested_fix: string;
}

// DataTable column keys MUST equal backend sort tokens. ⚠️ VERIFY vs backend ApplySorting whitelist
export type ErrorSortBy = 'failed_at' | 'error_code' | 'file_name' | 'step' | 'source';

export interface ErrorQuery {
  page: number; pageSize: number;
  step: string | null;      // null = all steps
  source: string | null;    // null = all sources
  from: string | null; to: string | null;
  sortBy: ErrorSortBy; sortDir: SortDir;
}
