// GET /batches/{id}
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
  status: string;            // Completed | Failed | Processing | Queued
  source: string;
  total_files: number;
  file_stats: BatchFileStats;
  times: BatchTimes;
}

// GET /batches/{id}/files  — list envelope { data, meta }; query = page + pageSize ONLY
export interface BatchFile {
  id: string;
  file_name: string;
  file_type: string;          // CSV | PDF | ...
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
