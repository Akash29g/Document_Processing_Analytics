export interface AdminUser {
  id: string;
  email: string;
  role: string;
  is_active: boolean;
  created_at: string;
  site_ids: string[];
}

export interface AdminSite {
  id: string;
  name: string;
  location: string | null;
  is_active: boolean;
}

export interface AdminCreatedUser {
  id: string;
  email: string;
  credentials_emailed: boolean;
}

// ── Error Catalog ──
export interface ErrorCatalogEntry {
  id: string;
  error_code: string;
  description: string;
  remediation_msg: string | null;
  created_at: string;
  updated_at: string;
}

export interface CreateErrorCatalogPayload {
  error_code: string;
  description: string;
  remediation_msg: string | null;
}

export interface UpdateErrorCatalogPayload {
  description: string;
  remediation_msg: string | null;
}
