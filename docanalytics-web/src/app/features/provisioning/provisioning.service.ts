import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { ProvisionedSite, ProvisionedUser, TenantSummary } from './provisioning.models';

@Injectable({ providedIn: 'root' })
export class ProvisioningService {
  private http = inject(HttpClient);
  private base = `${environment.apiBase}/provisioning`;

  readonly tenants = signal<TenantSummary[]>([]);
  readonly users = signal<ProvisionedUser[]>([]);
  readonly sitesList = signal<ProvisionedSite[]>([]);
  readonly selectedTenant = signal<TenantSummary | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async loadTenants(): Promise<void> {
    this.loading.set(true); this.error.set(null);
    try {
      const res = await firstValueFrom(
        this.http.get<ApiResponse<TenantSummary[]>>(`${this.base}/tenants`));
      this.tenants.set(res.data ?? []);
    } catch (e: any) {
      this.error.set(this.msg(e, 'Failed to load tenants.'));
    } finally { this.loading.set(false); }
  }

  async selectTenant(t: TenantSummary): Promise<void> {
    this.selectedTenant.set(t);
    await Promise.all([this.loadUsers(t.id), this.loadSites(t.id)]);
  }

  async loadUsers(tenantId: string): Promise<void> {
    const res = await firstValueFrom(
      this.http.get<ApiResponse<ProvisionedUser[]>>(`${this.base}/tenants/${tenantId}/users`));
    this.users.set(res.data ?? []);
  }

  async loadSites(tenantId: string): Promise<void> {
    const res = await firstValueFrom(
      this.http.get<ApiResponse<ProvisionedSite[]>>(`${this.base}/tenants/${tenantId}/sites`));
    this.sitesList.set(res.data ?? []);
  }

  async createTenant(name: string, orgDomain: string): Promise<string | null> {
    try {
      await firstValueFrom(this.http.post<ApiResponse<TenantSummary>>(
        `${this.base}/tenants`, { name, org_domain: orgDomain }));
      await this.loadTenants();
      return null;
    } catch (e: any) { return this.msg(e, 'Failed to create tenant.'); }
  }

  async createAdmin(tenantId: string, firstName: string, lastName: string): Promise<string | null> {
    try {
      await firstValueFrom(this.http.post<ApiResponse<ProvisionedUser>>(
        `${this.base}/tenants/${tenantId}/admins`, { first_name: firstName, last_name: lastName }));
      await Promise.all([this.loadUsers(tenantId), this.loadTenants()]);
      return null;
    } catch (e: any) { return this.msg(e, 'Failed to create admin.'); }
  }

  async removeUser(tenantId: string, user: ProvisionedUser): Promise<string | null> {
    const kind = user.role === 'Admin' ? 'admins' : 'users';
    try {
      await firstValueFrom(this.http.delete<ApiResponse<unknown>>(
        `${this.base}/tenants/${tenantId}/${kind}/${user.id}`));
      await Promise.all([this.loadUsers(tenantId), this.loadTenants()]);
      return null;
    } catch (e: any) { return this.msg(e, 'Failed to remove user.'); }
  }

  async createSite(tenantId: string, name: string, location: string): Promise<string | null> {
    try {
      await firstValueFrom(this.http.post<ApiResponse<ProvisionedSite>>(
        `${this.base}/tenants/${tenantId}/sites`, { name, location: location || null }));
      await Promise.all([this.loadSites(tenantId), this.loadTenants()]);
      return null;
    } catch (e: any) { return this.msg(e, 'Failed to create site.'); }
  }

  async removeSite(tenantId: string, siteId: string): Promise<string | null> {
    try {
      await firstValueFrom(this.http.delete<ApiResponse<unknown>>(
        `${this.base}/tenants/${tenantId}/sites/${siteId}`));
      await Promise.all([this.loadSites(tenantId), this.loadTenants()]);
      return null;
    } catch (e: any) { return this.msg(e, 'Failed to remove site.'); }
  }

  private msg(e: any, fallback: string): string {
    return e?.error?.error?.message ?? fallback;
  }
}
