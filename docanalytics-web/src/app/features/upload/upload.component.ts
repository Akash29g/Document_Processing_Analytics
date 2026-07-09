import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import JSZip from 'jszip';
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
  protected expanding = signal(false);
  protected fileNames = signal<string[]>([]);

  protected onDrop(e: DragEvent): void {
    e.preventDefault();
    this.dragging.set(false);
    const files = Array.from(e.dataTransfer?.files ?? []);
    if (files.length) this.handle(files);
  }

  protected onPick(e: Event): void {
    const files = Array.from((e.target as HTMLInputElement).files ?? []);
    if (files.length) this.handle(files);
  }

  private async handle(files: File[]): Promise<void> {
    this.expanding.set(true);
    let pdfs: File[];
    try {
      pdfs = await this.expandToPdfs(files);
    } finally {
      this.expanding.set(false);
    }

    if (!pdfs.length) {
      this.toast.error('No PDF invoices found. Accepted: .pdf files or a .zip containing PDFs.');
      return;
    }

    this.fileNames.set(pdfs.map(f => f.name));
    const ok = await this.svc.uploadBatch(pdfs);        // 👈 same batch upload
    if (ok) this.toast.success(`${pdfs.length} invoice(s) uploaded — extracting now.`);
    else this.toast.error(this.svc.error() ?? 'Upload failed.');
  }

  /** Turn a mixed selection (PDFs + ZIPs) into a flat list of PDF Files. */
  private async expandToPdfs(files: File[]): Promise<File[]> {
    const out: File[] = [];
    for (const f of files) {
      const lower = f.name.toLowerCase();
      if (lower.endsWith('.zip')) {
        const zip = await JSZip.loadAsync(f);
        for (const entry of Object.values(zip.files)) {
          const name = entry.name.split('/').pop() ?? entry.name;   // strip folder path
          if (entry.dir) continue;
          if (name.startsWith('.') || entry.name.startsWith('__MACOSX')) continue; // skip junk
          if (!name.toLowerCase().endsWith('.pdf')) continue;
          const blob = await entry.async('blob');
          out.push(new File([blob], name, { type: 'application/pdf' }));
        }
      } else if (lower.endsWith('.pdf')) {
        out.push(f);
      }
    }
    return out;
  }
}
