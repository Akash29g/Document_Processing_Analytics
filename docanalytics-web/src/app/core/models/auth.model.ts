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
  must_change_password: boolean;
}

// GET /auth/me → data
export interface MeResponse {
  user: AuthUser;
  sites: SiteSummary[];
}

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
  must_change_password: boolean;
  refresh_token: string; // NEW (R4)
}

// POST /auth/refresh → data
export interface RefreshResponse {
  token: string;
  refresh_token: string;
}

// GET /auth/me → data
export interface MeResponse {
  user: AuthUser;
  sites: SiteSummary[];
}

export interface TwoFactorChallengeResponse {
  requires_two_factor: true;
  challenge_token: string;
}

export type LoginOrChallenge = LoginResponse | TwoFactorChallengeResponse;

export interface TwoFactorSetupResponse {
  secret: string;
  otp_auth_uri: string;
  manual_key: string;
}

export interface TwoFactorConfirmResponse {
  recovery_codes: string[];
}

export interface SessionSummary {
  id: string;
  device_label: string;
  ip_address: string | null;
  created_at: string;
  last_used_at: string | null;
  is_current: boolean;
}
