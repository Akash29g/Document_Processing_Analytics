export interface AlertRule {
  id: string;
  name: string;
  threshold_percent: number;
  window_minutes: number;
  email: string;
  is_enabled: boolean;
  cooldown_minutes: number;
  last_triggered_at: string | null;
  created_at: string;
  updated_at: string;
}

export interface AlertRulePayload {
  name: string;
  threshold_percent: number;
  window_minutes: number;
  email: string;
  cooldown_minutes: number;
  is_enabled: boolean;
}

export interface Recipient {
  id: string;
  email: string;
  role: string;
}

