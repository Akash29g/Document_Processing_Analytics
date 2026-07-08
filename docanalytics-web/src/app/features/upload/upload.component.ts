import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { UploadService } from './upload.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-upload',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './upload.component.html',
  styleUrl: './upload.component.css',
})
export class UploadComponent {
  protected svc = inject(UploadService);
  private toast = inject(ToastService);
  protected dragging = signal(false);
  protected fileName = signal<string | null>(null);

  protected onDrop(e: DragEvent): void {
    e.preventDefault();
    this.dragging.set(false);
    const f = e.dataTransfer?.files?.[0];
    if (f) this.handle(f);
  }

  protected onPick(e: Event): void {
    const f = (e.target as HTMLInputElement).files?.[0];
    if (f) this.handle(f);
  }

  private async handle(file: File): Promise<void> {
    if (!file.name.toLowerCase().endsWith('.pdf')) {
      this.toast.error('Only PDF invoices are supported.');
      return;
    }
    this.fileName.set(file.name);
    const ok = await this.svc.upload(file);
    if (ok) this.toast.success(`"${file.name}" uploaded — extracting now.`);
    else this.toast.error(this.svc.error() ?? 'Upload failed.');
  }
}
