export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthUser {
  id: string;
  email: string;
  role: string; // 'Admin' | 'Viewer'
}

// Backend returns sites as { site_id, site_name } (snake_case)
export interface SiteSummary {
  site_id: string;
  site_name: string;
}

// POST /auth/login → data
export interface LoginResponse {
  token: string;
  user: AuthUser;
  sites: SiteSummary[];
}

// GET /auth/me → data
export interface MeResponse {
  user: AuthUser;
  sites: SiteSummary[];
}
