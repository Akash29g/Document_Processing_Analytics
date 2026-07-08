import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';

interface UploadUrlResponse { file_id: string; upload_url: string; }

@Injectable({ providedIn: 'root' })
export class UploadService {
  private http = inject(HttpClient);
  private base = environment.apiBase;

  readonly uploading = signal(false);
  readonly error = signal<string | null>(null);
  readonly lastFileId = signal<string | null>(null);

  async upload(file: File): Promise<boolean> {
    this.uploading.set(true);
    this.error.set(null);
    try {
      // 1) ask our API for a presigned URL
      const ctx = new HttpContext().set(SKIP_ERROR_TOAST, true);
      const res = await firstValueFrom(this.http.post<ApiResponse<UploadUrlResponse>>(
        `${this.base}/files/upload-url`,
        { file_name: file.name, size_bytes: file.size },
        { context: ctx }));

      const data = res.data!;
      // 2) PUT the bytes straight to S3 (no auth header — it's a signed URL)
      await fetch(data.upload_url, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/pdf' },
        body: file,
      }).then(r => { if (!r.ok) throw new Error('S3 upload failed'); });

      // 3) tell our API it's done → enqueues extraction
      await firstValueFrom(this.http.post<ApiResponse<unknown>>(
        `${this.base}/files/${data.file_id}/complete`, {}, { context: ctx }));

      this.lastFileId.set(data.file_id);
      return true;
    } catch (e: any) {
      this.error.set(e?.error?.error?.message ?? e?.message ?? 'Upload failed.');
      return false;
    } finally {
      this.uploading.set(false);
    }
  }
}
