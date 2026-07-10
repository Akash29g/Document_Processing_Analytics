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
