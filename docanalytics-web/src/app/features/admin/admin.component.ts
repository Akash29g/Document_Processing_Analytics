import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService } from './admin.service';
import { AdminUser } from './admin.models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminComponent {
  protected svc = inject(AdminService);

  // create-user form
  protected uFirst = '';
  protected uLast = '';
  protected uSites = signal<Set<string>>(new Set());
  // create-site form
  protected sName = '';
  protected sLocation = '';
  // per-user site editing
  protected editingUser = signal<AdminUser | null>(null);
  protected editSites = signal<Set<string>>(new Set());

  protected formError = signal<string | null>(null);
  protected notice = signal<string | null>(null);

  constructor() {
    this.svc.loadAll();
  }

  protected toggleNewUserSite(id: string): void {
    const s = new Set(this.uSites());
    s.has(id) ? s.delete(id) : s.add(id);
    this.uSites.set(s);
  }

  protected async addUser(): Promise<void> {
    if (!this.uFirst.trim() || !this.uLast.trim() || this.uSites().size === 0) {
      this.formError.set('Fill in the name and pick at least one site.');
      return;
    }
    const res = await this.svc.createUser(this.uFirst.trim(), this.uLast.trim(), [
      ...this.uSites(),
    ]);
    this.formError.set(res.error);
    if (!res.error) {
      this.notice.set(`User ${res.email} created — credentials emailed.`);
      this.uFirst = '';
      this.uLast = '';
      this.uSites.set(new Set());
    }
  }

  protected startEdit(u: AdminUser): void {
    this.editingUser.set(u);
    this.editSites.set(new Set(u.site_ids));
  }

  protected toggleEditSite(id: string): void {
    const s = new Set(this.editSites());
    s.has(id) ? s.delete(id) : s.add(id);
    this.editSites.set(s);
  }

  protected async saveEdit(): Promise<void> {
    const u = this.editingUser();
    if (!u) return;
    if (this.editSites().size === 0) {
      this.formError.set('User must keep at least one site.');
      return;
    }
    const err = await this.svc.updateUserSites(u.id, [...this.editSites()]);
    this.formError.set(err);
    if (!err) {
      this.notice.set('Site access updated.');
      this.editingUser.set(null);
    }
  }

  protected cancelEdit(): void {
    this.editingUser.set(null);
  }

  protected async remove(u: AdminUser): Promise<void> {
    if (!confirm(`Remove ${u.email}? They will no longer be able to log in.`)) return;
    this.formError.set(await this.svc.deactivateUser(u.id));
  }

  protected async addSite(): Promise<void> {
    if (!this.sName.trim()) return;
    const err = await this.svc.createSite(this.sName.trim(), this.sLocation.trim());
    this.formError.set(err);
    if (!err) {
      this.notice.set('Site added.');
      this.sName = '';
      this.sLocation = '';
    }
  }

  protected siteName(id: string): string {
    return this.svc.sitesList().find((s) => s.id === id)?.name ?? '?';
  }
}
