import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { BatchDetailComponent } from './batch-detail.component';
import { BatchService } from '../batch.service';
import { SiteContextService } from '../../../core/services/site-context.service';
import { of } from 'rxjs';

describe('BatchDetailComponent', () => {
  const navSpy = vi.fn();

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BatchDetailComponent],
      providers: [
        {
          provide: BatchService, useValue: {
            detail: signal(null), detailLoading: signal(false), detailError: signal(null),
            files: signal([]), filesLoading: signal(false), filesError: signal(null),
            filesMeta: signal(null), filesQuery: signal({ page: 1, pageSize: 20 }),
            loadDetail: vi.fn(), loadFiles: vi.fn(),
            setFilesPage: vi.fn(), setFilesPageSize: vi.fn(), setBatchId: vi.fn()
          }
        },
        { provide: SiteContextService, useValue: { selectedSiteId: signal('s1') } },
        {
          provide: ActivatedRoute, useValue: {
            paramMap: of(convertToParamMap({ batchId: 'b1' })),
            snapshot: { paramMap: convertToParamMap({ batchId: 'b1' }) },
          }
        },

        { provide: Router, useValue: { navigate: navSpy } },
      ],
    }).compileComponents();
    navSpy.mockClear();
  });

  it('formatSize() renders KB and MB', () => {
    const comp = TestBed.createComponent(BatchDetailComponent).componentInstance as any;
    expect(comp.formatSize(2048)).toContain('KB');
    expect(comp.formatSize(5 * 1024 * 1024)).toContain('MB');
  });

  it('openFile() navigates to the file route', () => {
    const comp = TestBed.createComponent(BatchDetailComponent).componentInstance as any;
    comp.openFile({ id: 'f1' });
    expect(navSpy).toHaveBeenCalled();
  });
});
