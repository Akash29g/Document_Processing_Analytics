// ── GET /api/v1/files/{id}/details  → FileDetailDto (snake_case) ──
export interface FileInfo {
  id: string;
  name: string;
  current_status: string;   // Completed | Failed | Processing | Queued
  current_step: string;
}

export interface StepError {
  code: string;
  message: string | null;
  suggested_fix: string | null;   // ErrorCatalog.remediation_msg joined by code
}

export interface StepHistoryItem {
  step: string;                    // Upload | Validate | Transform | Load
  status: string;                  // Success | Failed | Processing
  ts: string | null;               // ISO-8601
  error?: StepError | null;        // present only on failed steps
}

export interface FileDetail {
  file_info: FileInfo;
  history: StepHistoryItem[];
}

// ── GET /api/v1/files/{id}/line-items → InvoiceDetailDto (404 if no invoice) ──
export interface InvoiceLineItem {
  line_number: number;
  description: string;
  quantity: number | null;
  unit_price: number | null;
  line_total: number | null;
  confidence: number | null;       // DECIMAL(4,3) → 0–0.999
  is_valid: boolean;
  category_code: string | null;    // null when uncategorized (LEFT join)
  category_name: string | null;
}

export interface InvoiceDetail {
  grand_total: number;
  items: InvoiceLineItem[];
}
