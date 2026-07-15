// The single response contract every endpoint returns (DT-2, NFR-5).
export interface ApiResponse<T> {
  data: T | null;
  meta?: Meta; // present only on list endpoints
  error: ApiError | null;
}

export interface Meta {
  total_count: number;
  page: number;
  page_size: number;
  total_pages: number;
}

export interface ApiError {
  code: string;
  message: string;
  details?: unknown;
}
