import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { DuplicateDialogService } from './duplicate-dialog.service';

interface CreateBatchResponse { batch_id: string; }
interface UploadUrlResponse { file_id: string; upload_url: string; }

@Injectable({ providedIn: 'root' })
export class UploadService {
  private http = inject(HttpClient);
  private base = environment.apiBase;
  private dupDialog = inject(DuplicateDialogService);

  readonly uploading = signal(false);
  readonly error = signal<string | null>(null);
  readonly progress = signal<{ done: number; total: number } | null>(null);
  readonly lastBatchId = signal<string | null>(null);

  private requestUrl(batchId: string, file: File, onDuplicate: string | null, ctx: HttpContext) {
    return firstValueFrom(this.http.post<ApiResponse<UploadUrlResponse>>(
      `${this.base}/files/upload-url`,
      { batch_id: batchId, file_name: file.name, size_bytes: file.size, on_duplicate: onDuplicate },
      { context: ctx }));
  }

  async uploadBatch(files: File[]): Promise<boolean> {
    if (!files.length) return false;
    this.uploading.set(true);
    this.error.set(null);
    this.lastBatchId.set(null);
    this.progress.set({ done: 0, total: files.length });
    const ctx = new HttpContext().set(SKIP_ERROR_TOAST, true);

    try {
      // 1) open ONE batch for the whole upload
      const batchRes = await firstValueFrom(this.http.post<ApiResponse<CreateBatchResponse>>(
        `${this.base}/files/batches`,
        { file_count: files.length },
        { context: ctx }));
      const batchId = batchRes.data!.batch_id;

      // 2) upload each file INTO that batch
      for (const file of files) {
        let res;
        try {
          res = await this.requestUrl(batchId, file, null, ctx);
        } catch (e: any) {
          if (e?.error?.error?.code === 'DUPLICATE_FILE') {
            const choice = await this.dupDialog.ask(file.name);
            if (choice === 'skip') {
              await firstValueFrom(this.http.post<ApiResponse<unknown>>(
                `${this.base}/files/batches/${batchId}/shrink`, {}, { context: ctx }));
              this.progress.update(p => p ? { done: p.done, total: p.total - 1 } : p);
              continue;
            }
            res = await this.requestUrl(batchId, file, choice, ctx);

          } else { throw e; }
        }
        const data = res.data!;

        // b) PUT bytes straight to S3 (signed URL — no auth header)
        await fetch(data.upload_url, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/pdf' },
          body: file,
        }).then(r => { if (!r.ok) throw new Error(`S3 upload failed for ${file.name}`); });

        // c) mark complete → enqueues extraction
        await firstValueFrom(this.http.post<ApiResponse<unknown>>(
          `${this.base}/files/${data.file_id}/complete`, {}, { context: ctx }));

        this.progress.update(p => p ? { done: p.done + 1, total: p.total } : p);
      }

      this.lastBatchId.set(batchId);
      return true;
    } catch (e: any) {
      this.error.set(e?.error?.error?.message ?? e?.message ?? 'Upload failed.');
      return false;
    } finally {
      this.uploading.set(false);
    }
  }
}
