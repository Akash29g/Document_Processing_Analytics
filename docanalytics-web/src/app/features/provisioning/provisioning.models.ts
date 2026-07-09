export interface TenantSummary {
  id: string;
  name: string;
  org_domain: string;
  is_active: boolean;
  site_count: number;
  user_count: number;
  admin_count: number;
}

export interface ProvisionedUser {
  id: string;
  email: string;
  role: string;
  is_active: boolean;
  created_at: string;
}

export interface ProvisionedSite {
  id: string;
  name: string;
  location: string | null;
  is_active: boolean;
}
