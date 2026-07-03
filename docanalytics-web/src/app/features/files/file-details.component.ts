import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy, Component, computed, effect, inject, untracked,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map } from 'rxjs/operators';
import { FileDetailsService } from './file-details.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import { InvoiceLineItem, StepHistoryItem } from './file-details.models';

@Component({
  selector: 'app-file-details',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, RouterLink, StatusBadgeComponent],
  template: `
    <section class="fd">
      <!-- back link -->
      <a class="fd-back" routerLink="../..">← Back to batch</a>

      <!-- ─────────── File info header (FR-2.5) ─────────── -->
      @if (svc.detailLoading()) {
        <div class="fd-card fd-state"><span class="spinner"></span> Loading file…</div>
      } @else if (svc.detailError()) {
        <div class="fd-card fd-state fd-error">
          {{ svc.detailError() }}
          <button class="fd-btn" (click)="svc.loadDetails()">Retry</button>
        </div>
      } @else if (info(); as fi) {
        <header class="fd-card fd-header">
          <div>
            <p class="fd-eyebrow">File</p>
            <h1 class="fd-title">{{ fi.name }}</h1>
            <p class="fd-sub">Current step: <strong>{{ fi.current_step }}</strong></p>
          </div>
          <div class="fd-header-right">
            <app-status-badge [status]="fi.current_status" />
            <button class="fd-btn" (click)="svc.downloadLogs()">Download Logs</button>
          </div>
        </header>

        <!-- ─────────── Step timeline (FR-2.5) ─────────── -->
        <div class="fd-card">
          <h2 class="fd-h2">Processing timeline</h2>
          @if (history().length === 0) {
            <div class="fd-state">No steps recorded for this file.</div>
          } @else {
            <ol class="tl">
              @for (s of history(); track $index) {
                <li class="tl-item" [class.is-failed]="isFailed(s)">
                  <span class="tl-dot" [class]="'dot-' + stepClass(s)"></span>
                  <div class="tl-body">
                    <div class="tl-row">
                      <span class="tl-step">{{ s.step }}</span>
                      <span class="chip" [class]="'chip-' + stepClass(s)">{{ s.status }}</span>
                      <span class="tl-ts">{{ s.ts ? (s.ts | date: 'medium') : '—' }}</span>
                    </div>
                    @if (s.error; as e) {
                      <div class="tl-err">
                        <div><strong>{{ e.code }}</strong>{{ e.message ? ' — ' + e.message : '' }}</div>
                        @if (e.suggested_fix) {
                          <div class="tl-fix">💡 Suggested fix: {{ e.suggested_fix }}</div>
                        }
                      </div>
                    }
                  </div>
                </li>
              }
            </ol>
          }
        </div>
      }

      <!-- ─────────── Invoice line items ─────────── -->
      <div class="fd-card">
        <h2 class="fd-h2">Invoice line items</h2>
        @if (svc.invoiceLoading()) {
          <div class="fd-state"><span class="spinner"></span> Loading line items…</div>
        } @else if (!svc.hasInvoice()) {
          <div class="fd-state">This file could not be found.</div>
        } @else if (svc.invoiceError()) {
          <div class="fd-state fd-error">
            {{ svc.invoiceError() }}
            <button class="fd-btn" (click)="svc.loadLineItems()">Retry</button>
          </div>
        } @else if (items().length === 0) {
          <div class="fd-state">No line items — this file has no extracted invoice items.</div>
        } @else {
          <div class="fd-scroll">
          <table class="tbl">
            <thead>
              <tr>
                <th class="r">#</th>
                <th>Description</th>
                <th>Category</th>
                <th class="r">Qty</th>
                <th class="r">Unit price</th>
                <th class="r">Line total</th>
                <th class="r">Confidence</th>
              </tr>
            </thead>
            <tbody>
              @for (li of items(); track li.line_number) {
                <tr>
                  <td class="r">{{ li.line_number }}</td>
                  <td>{{ li.description }}</td>
                  <td>{{ li.category_name ?? 'Uncategorized' }}</td>
                  <td class="r">{{ num(li.quantity, 3) }}</td>
                  <td class="r">{{ num(li.unit_price, 2) }}</td>
                  <td class="r">{{ num(li.line_total, 2) }}</td>
                  <td class="r">{{ pct(li.confidence) }}</td>
                </tr>
              }
            </tbody>
            <tfoot>
              <tr>
                <td colspan="5" class="r"><strong>Grand total</strong></td>
                <td class="r"><strong>{{ num(svc.invoice()?.grand_total ?? 0, 2) }}</strong></td>
                <td></td>
              </tr>
            </tfoot>
          </table>
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .fd { display: flex; flex-direction: column; gap: var(--space-2); max-width: 1100px; }
    .fd-back { font-size: .85rem; }
    .fd-card { background: var(--white); border: 1px solid var(--cool-gray);
      border-radius: 8px; padding: var(--space-2); }
    .fd-header { display: flex; flex-wrap: wrap;  justify-content: space-between; align-items: center; gap: var(--space-2); }
    .fd-header-right { display: flex; align-items: center; gap: var(--space-1); }
    .fd-eyebrow { margin: 0; font-size: .72rem; text-transform: uppercase;
      letter-spacing: .04em; color: var(--dark-gray-3); }
    .fd-title { margin: 2px 0 0; font-family: var(--font-display); font-size: 1.15rem; color: var(--dark-gray); }
    .fd-sub { margin: 4px 0 0; font-size: .82rem; color: var(--dark-gray-3); }
    .fd-h2 { margin: 0 0 var(--space-1); font-size: .95rem; color: var(--dark-gray); }
    .fd-state { display: flex; align-items: center; gap: var(--space-1);
      color: var(--dark-gray-3); font-size: .88rem; padding: var(--space-1) 0; }
    .fd-error { color: var(--text-error); }
    .fd-btn { border: 1px solid var(--cool-gray); background: var(--white);
      color: var(--slate-blue); border-radius: 6px; padding: 6px 12px;
      font-size: .82rem; cursor: pointer; }
    .fd-btn:hover { border-color: var(--slate-blue); }

    /* timeline */
    .tl { list-style: none; margin: 0; padding: 0; }
    .tl-item { position: relative; padding: 0 0 var(--space-2) var(--space-3); border-left: 2px solid var(--cool-gray); }
    .tl-item:last-child { border-left-color: transparent; padding-bottom: 0; }
    .tl-dot { position: absolute; left: -7px; top: 2px; width: 12px; height: 12px; border-radius: 50%;
      background: var(--cool-gray); }
    .dot-success { background: var(--status-success, #2e7d32); }
    .dot-failed  { background: var(--status-error, #c62828); }
    .dot-processing { background: var(--slate-blue); }
    .tl-row { display: flex; align-items: center; gap: var(--space-1); flex-wrap: wrap; }
    .tl-step { font-weight: 600; color: var(--dark-gray); }
    .tl-ts { font-size: .78rem; color: var(--dark-gray-3); margin-left: auto; }
    .chip { font-size: .72rem; padding: 2px 8px; border-radius: 999px; }
    .chip-success { background: #e6f4ea; color: #1e7e34; }
    .chip-failed  { background: #fdecea; color: #c62828; }
    .chip-processing { background: #e8f0fe; color: #1a56b0; }
    .tl-err { margin-top: 6px; font-size: .82rem; color: var(--text-error); }
    .tl-fix { margin-top: 4px; color: var(--dark-gray-3); }

    /* invoice table */
    .fd-scroll { overflow-x: auto; }
    .fd-scroll .tbl { min-width: 640px; }
    .tbl { width: 100%; border-collapse: collapse; font-size: .85rem; }
    .tbl th, .tbl td { padding: 8px 10px; border-bottom: 1px solid var(--cool-gray); text-align: left; }
    .tbl th { color: var(--dark-gray-3); font-weight: 600; }
    .tbl .r { text-align: right; }
    .tbl tfoot td { border-top: 2px solid var(--cool-gray); border-bottom: none; }

    .spinner { width: 14px; height: 14px; border: 2px solid var(--cool-gray);
      border-top-color: var(--slate-blue); border-radius: 50%; animation: sp .7s linear infinite; }
    @keyframes sp { to { transform: rotate(360deg); } }
  `],
})
export class FileDetailsComponent {
  protected readonly svc = inject(FileDetailsService);
  private readonly route = inject(ActivatedRoute);
  private readonly site = inject(SiteContextService);

  private readonly fileId = toSignal(
    this.route.paramMap.pipe(map((p) => p.get('fileId'))),
    { initialValue: this.route.snapshot.paramMap.get('fileId') },
  );

  protected readonly info = computed(() => this.svc.detail()?.file_info ?? null);
  protected readonly history = computed<StepHistoryItem[]>(() => this.svc.detail()?.history ?? []);
  protected readonly items = computed<InvoiceLineItem[]>(() => this.svc.invoice()?.items ?? []);

  constructor() {
    // reload on file switch (param-only nav) AND on site switch — both tracked,
    // loads run in untracked so query reads inside don't re-fire the effect (R3 lesson).
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
