import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { BatchService } from './batch.service';

describe('BatchService', () => {
  let service: BatchService;
  let httpMock: HttpTestingController;
  const base = environment.apiBase;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(BatchService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  afterEach(() => httpMock.verify());

  it('loadBatches() sends default paging + sort params', () => {
    service.loadBatches();
    const req = httpMock.expectOne(r => r.url === `${base}/batches`);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('page')).toBe('1');
    expect(req.request.params.get('pageSize')).toBe('20');
    req.flush({ data: [], meta: { total_count: 0, page: 1, page_size: 20, total_pages: 0 } });
    expect(service.batches()).toEqual([]);
  });

  it('loadBatches() fills batches + meta on success', () => {
    service.loadBatches();
    httpMock.expectOne(r => r.url === `${base}/batches`)
      .flush({
        data: [{ transaction_id: 'TID-1', state: 'Failed' }],
        meta: { total_count: 1, page: 1, page_size: 20, total_pages: 1 }
      });
    expect(service.batches().length).toBe(1);
    expect(service.meta()?.total_count).toBe(1);
  });

  it('loadBatches() sets error signal on failure', () => {
    service.loadBatches();
    httpMock.expectOne(r => r.url === `${base}/batches`)
      .flush('boom', { status: 500, statusText: 'Server Error' });
    expect(service.error()).toBeTruthy();
    expect(service.loading()).toBe(false);
  });

  it('setSort() sends the new sort params', () => {
    service.setSort('submitted_at', 'asc');
    const req = httpMock.expectOne(r => r.url === `${base}/batches`);
    expect(req.request.params.get('sortBy')).toBe('submitted_at');
    expect(req.request.params.get('sortDir')).toBe('asc');
    req.flush({ data: [], meta: null });
  });

  it('loadSources() fills the sources signal', () => {
    service.loadSources();
    const req = httpMock.expectOne(r => r.url === `${base}/batches/sources`);
    req.flush({ data: ['S3', 'SFTP'] });
    expect(service.sources().length).toBe(2);
  });
});
