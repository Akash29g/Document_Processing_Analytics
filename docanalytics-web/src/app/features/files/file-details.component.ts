import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  untracked,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map } from 'rxjs/operators';
import { FileDetailsService } from './file-details.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { AuthService } from '../../core/services/auth.service';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import { InvoiceLineItem, StepHistoryItem } from './file-details.models';

@Component({
  selector: 'app-file-details',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, RouterLink, StatusBadgeComponent],
  templateUrl: './file-details.component.html',
  styleUrl: './file-details.component.css',
})
export class FileDetailsComponent {
  protected readonly svc = inject(FileDetailsService);
  private readonly route = inject(ActivatedRoute);
  private readonly site = inject(SiteContextService);
  private readonly auth = inject(AuthService);

  private readonly fileId = toSignal(this.route.paramMap.pipe(map((p) => p.get('fileId'))), {
    initialValue: this.route.snapshot.paramMap.get('fileId'),
  });

  protected readonly info = computed(() => this.svc.detail()?.file_info ?? null);
  protected readonly history = computed<StepHistoryItem[]>(() => this.svc.detail()?.history ?? []);
  protected readonly items = computed<InvoiceLineItem[]>(() => this.svc.invoice()?.items ?? []);

  /** True when the logged-in user is an Admin. */
  protected readonly isAdmin = computed(() => this.auth.currentUser()?.role === 'Admin');

  constructor() {
    // reload on file switch (param-only nav) AND on site switch — both tracked,
    // loads run in untracked so query reads inside don't re-fire the effect (R3 lesson)
    effect(() => {
      const id = this.fileId();
      this.site.selectedSiteId();
      if (!id) return;
      untracked(() => this.svc.load(id));
    });
  }

  protected isFailed(s: StepHistoryItem): boolean {
    return s.status?.toLowerCase() === 'failed';
  }
  protected stepClass(s: StepHistoryItem): 'success' | 'failed' | 'processing' {
    const v = s.status?.toLowerCase();
    if (v === 'failed') return 'failed';
    if (v === 'processing') return 'processing';
    return 'success';
  }
  protected num(v: number | null | undefined, dp: number): string {
    return v == null ? '—' : Number(v).toFixed(dp);
  }
  protected pct(c: number | null): string {
    return c == null ? '—' : (c * 100).toFixed(1) + '%';
  }
}
