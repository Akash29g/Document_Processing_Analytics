import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import {
  AdminCreatedUser,
  AdminSite,
  AdminUser,
  CreateErrorCatalogPayload,
  ErrorCatalogEntry,
  UpdateErrorCatalogPayload,
} from './admin.models';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private base = `${environment.apiBase}/admin`;
  private catalogBase = `${environment.apiBase}/error-catalog`;

  // ── users + sites ──
  readonly users = signal<AdminUser[]>([]);
  readonly sitesList = signal<AdminSite[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  // ── error catalog ──
  readonly catalog = signal<ErrorCatalogEntry[]>([]);
  readonly catalogLoading = signal(false);
  readonly catalogError = signal<string | null>(null);

  async loadAll(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const [users, sites] = await Promise.all([
        firstValueFrom(this.http.get<ApiResponse<AdminUser[]>>(`${this.base}/users`)),
        firstValueFrom(this.http.get<ApiResponse<AdminSite[]>>(`${this.base}/sites`)),
      ]);
      this.users.set(users.data ?? []);
      this.sitesList.set(sites.data ?? []);
    } catch (e: any) {
      this.error.set(this.msg(e, 'Failed to load users and sites.'));
    } finally {
      this.loading.set(false);
    }
  }

  async createUser(
    firstName: string,
    lastName: string,
    siteIds: string[],
  ): Promise<{ error: string | null; email?: string }> {
    try {
      const res = await firstValueFrom(
        this.http.post<ApiResponse<AdminCreatedUser>>(`${this.base}/users`, {
          first_name: firstName,
          last_name: lastName,
          site_ids: siteIds,
        }),
      );
      await this.loadAll();
      return { error: null, email: res.data?.email };
    } catch (e: any) {
      return { error: this.msg(e, 'Failed to create user.') };
    }
  }

  async updateUserSites(userId: string, siteIds: string[]): Promise<string | null> {
    try {
      await firstValueFrom(
        this.http.put<ApiResponse<unknown>>(`${this.base}/users/${userId}/sites`, {
          site_ids: siteIds,
        }),
      );
      await this.loadAll();
      return null;
    } catch (e: any) {
      return this.msg(e, 'Failed to update site access.');
    }
  }

  async deactivateUser(userId: string): Promise<string | null> {
    try {
      await firstValueFrom(this.http.delete<ApiResponse<unknown>>(`${this.base}/users/${userId}`));
      await this.loadAll();
      return null;
    } catch (e: any) {
      return this.msg(e, 'Failed to remove user.');
    }
  }

  async createSite(name: string, location: string): Promise<string | null> {
    try {
      await firstValueFrom(
        this.http.post<ApiResponse<AdminSite>>(`${this.base}/sites`, {
          name,
          location: location || null,
        }),
      );
      await this.loadAll();
      return null;
    } catch (e: any) {
      return this.msg(e, 'Failed to create site.');
    }
  }

  // ── Error Catalog ────────────────────────────────────────────────────────

  async loadCatalog(): Promise<void> {
    this.catalogLoading.set(true);
    this.catalogError.set(null);
    try {
      const res = await firstValueFrom(
        this.http.get<ApiResponse<ErrorCatalogEntry[]>>(this.catalogBase),
      );
      this.catalog.set(res.data ?? []);
    } catch (e: any) {
      this.catalogError.set(this.msg(e, 'Failed to load error catalog.'));
    } finally {
      this.catalogLoading.set(false);
    }
  }

  async createCatalogEntry(payload: CreateErrorCatalogPayload): Promise<string | null> {
    try {
      await firstValueFrom(
        this.http.post<ApiResponse<ErrorCatalogEntry>>(this.catalogBase, payload),
      );
      await this.loadCatalog();
      return null;
    } catch (e: any) {
      return this.msg(e, 'Failed to create error catalog entry.');
    }
  }

  async updateCatalogEntry(id: string, payload: UpdateErrorCatalogPayload): Promise<string | null> {
    try {
      await firstValueFrom(
        this.http.put<ApiResponse<ErrorCatalogEntry>>(`${this.catalogBase}/${id}`, payload),
      );
      await this.loadCatalog();
      return null;
    } catch (e: any) {
      return this.msg(e, 'Failed to update error catalog entry.');
    }
  }
  async deleteCatalogEntry(id: string): Promise<string | null> {
    try {
      await firstValueFrom(this.http.delete<ApiResponse<unknown>>(`${this.catalogBase}/${id}`));
      await this.loadCatalog();
      return null;
    } catch (e: any) {
      return this.msg(e, 'Failed to delete entry.');
    }
  }

  private msg(e: any, fallback: string): string {
    return e?.error?.error?.message ?? fallback;
  }
}
