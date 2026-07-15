import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ProvisioningService } from './provisioning.service';
import { ProvisionedUser, TenantSummary } from './provisioning.models';

@Component({
  selector: 'app-provisioning',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './provisioning.component.html',
  styleUrl: './provisioning.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProvisioningComponent {
  protected svc = inject(ProvisioningService);
  protected auth = inject(AuthService);
  private router = inject(Router);

  // create-tenant form
  protected tName = '';
  protected tDomain = '';
  // create-admin form
  protected aFirst = '';
  protected aLast = '';
  // create-site form
  protected sName = '';
  protected sLocation = '';

  protected formError = signal<string | null>(null);
  protected notice = signal<string | null>(null);

  // logout confirmation dialog
  protected showLogoutConfirm = signal(false);

  constructor() {
    this.svc.loadTenants();
  }

  protected select(t: TenantSummary): void {
    this.formError.set(null);
    this.notice.set(null);
    this.svc.selectTenant(t);
  }

  protected async addTenant(): Promise<void> {
    if (!this.tName.trim() || !this.tDomain.trim()) return;
    const err = await this.svc.createTenant(this.tName.trim(), this.tDomain.trim().toLowerCase());
    this.formError.set(err);
    if (!err) {
      this.notice.set(`Company "${this.tName.trim()}" onboarded.`);
      this.tName = '';
      this.tDomain = '';
    }
  }

  protected async addAdmin(): Promise<void> {
    const t = this.svc.selectedTenant();
    if (!t || !this.aFirst.trim() || !this.aLast.trim()) return;
    const err = await this.svc.createAdmin(t.id, this.aFirst.trim(), this.aLast.trim());
    this.formError.set(err);
    if (!err) {
      this.notice.set('Admin created — credentials emailed.');
      this.aFirst = '';
      this.aLast = '';
    }
  }

  protected async remove(u: ProvisionedUser): Promise<void> {
    const t = this.svc.selectedTenant();
    if (!t) return;
    if (!confirm(`Remove ${u.email}?`)) return;
    this.formError.set(await this.svc.removeUser(t.id, u));
  }

  protected async addSite(): Promise<void> {
    const t = this.svc.selectedTenant();
    if (!t || !this.sName.trim()) return;
    const err = await this.svc.createSite(t.id, this.sName.trim(), this.sLocation.trim());
    this.formError.set(err);
    if (!err) {
      this.notice.set('Site added.');
      this.sName = '';
      this.sLocation = '';
    }
  }

  protected async removeSite(siteId: string, name: string): Promise<void> {
    const t = this.svc.selectedTenant();
    if (!t) return;
    if (!confirm(`Remove site "${name}"? Users lose access to it.`)) return;
    this.formError.set(await this.svc.removeSite(t.id, siteId));
  }

  // ── logout flow with confirmation (mirrors the shell) ──
  protected askLogout(): void {
    this.showLogoutConfirm.set(true);
  }
  protected cancelLogout(): void {
    this.showLogoutConfirm.set(false);
  }
  protected confirmLogout(): void {
    this.showLogoutConfirm.set(false);
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
