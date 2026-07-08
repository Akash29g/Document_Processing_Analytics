import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { ErrorService } from './error.service';

describe('ErrorService', () => {
  let service: ErrorService;
  let httpMock: HttpTestingController;
  const base = environment.apiBase;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ErrorService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();        // no leftover/unexpected requests
    vi.restoreAllMocks();
  });

  // ===== loadTop =====
  it('loadTop() calls top-frequencies with topN=10 and fills the top signal', () => {
    service.loadTop();
    const req = httpMock.expectOne(r => r.url === `${base}/errors/top-frequencies`);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('topN')).toBe('10');

    req.flush({ data: { points: [{ label: 'ERR_TIMEOUT', value: 5 }] } });

    expect(service.top().length).toBe(1);
    expect(service.top()[0].label).toBe('ERR_TIMEOUT');
    expect(service.topLoading()).toBe(false);
  });

  it('loadTop() sets topError on failure', () => {
    service.loadTop();
    httpMock.expectOne(r => r.url === `${base}/errors/top-frequencies`)
      .flush('boom', { status: 500, statusText: 'Server Error' });

    expect(service.topError()).toBe('Could not load top errors. Please retry.');
    expect(service.topLoading()).toBe(false);
  });

  // ===== loadTrend =====
  it('loadTrend() omits from/to when they are not set', () => {
    service.loadTrend();
    const req = httpMock.expectOne(r => r.url === `${base}/errors/trend`);
    expect(req.request.params.has('from')).toBe(false);
    expect(req.request.params.has('to')).toBe(false);
    req.flush({ data: { points: [] } });
  });

  // ===== loadErrors / buildParams =====
  it('loadErrors() sends default paging + sort params', () => {
    service.loadErrors();
    const req = httpMock.expectOne(r => r.url === `${base}/errors`);
    expect(req.request.params.get('page')).toBe('1');
    expect(req.request.params.get('pageSize')).toBe('20');
    expect(req.request.params.get('sortBy')).toBe('failed_at');
    expect(req.request.params.get('sortDir')).toBe('desc');
    expect(req.request.params.has('step')).toBe(false);   // omitted when null

    req.flush({ data: [], meta: { total_count: 0, page: 1, page_size: 20, total_pages: 0 } });
    expect(service.meta()?.page).toBe(1);
  });

  it('loadErrors() sets errors and meta on success', () => {
    service.loadErrors();
    const item = {
      file_id: 'f1', file_name: 'a.pdf', error_code: 'E1', error_message: 'x',
      step: 'Load', source: 'S3', failed_at: '2026-01-01', suggested_fix: 'fix',
    };
    httpMock.expectOne(r => r.url === `${base}/errors`)
      .flush({ data: [item], meta: { total_count: 1, page: 1, page_size: 20, total_pages: 1 } });

    expect(service.errors().length).toBe(1);
    expect(service.meta()?.total_count).toBe(1);
  });

  it('loadErrors() sets error signal on failure', () => {
    service.loadErrors();
    httpMock.expectOne(r => r.url === `${base}/errors`)
      .flush('boom', { status: 500, statusText: 'Server Error' });

    expect(service.error()).toBe('Could not load errors. Please retry.');
    expect(service.loading()).toBe(false);
  });

  // ===== setFilters (status → step mapping + reloads errors AND trend) =====
  it('setFilters() maps status to step and reloads errors + trend', () => {
    service.setFilters({ status: 'Validate', source: 'S3', from: null, to: null });

    const errReq = httpMock.expectOne(r => r.url === `${base}/errors`);
    expect(errReq.request.params.get('step')).toBe('Validate');
    expect(errReq.request.params.get('source')).toBe('S3');
    errReq.flush({ data: [], meta: null });

    httpMock.expectOne(r => r.url === `${base}/errors/trend`).flush({ data: { points: [] } });

    expect(service.query().step).toBe('Validate');
  });

  it('setFilters() with status "all" clears the step filter', () => {
    service.setFilters({ status: 'all', source: null, from: null, to: null });

    const errReq = httpMock.expectOne(r => r.url === `${base}/errors`);
    expect(errReq.request.params.has('step')).toBe(false);
    errReq.flush({ data: [], meta: null });

    httpMock.expectOne(r => r.url === `${base}/errors/trend`).flush({ data: { points: [] } });

    expect(service.query().step).toBeNull();
  });

  it('setFilters() forwards from/to to both errors and trend', () => {
    service.setFilters({ status: 'all', source: null, from: '2026-01-01', to: '2026-01-31' });

    const errReq = httpMock.expectOne(r => r.url === `${base}/errors`);
    expect(errReq.request.params.get('from')).toBe('2026-01-01');
    expect(errReq.request.params.get('to')).toBe('2026-01-31');
    errReq.flush({ data: [], meta: null });

    const trendReq = httpMock.expectOne(r => r.url === `${base}/errors/trend`);
    expect(trendReq.request.params.get('from')).toBe('2026-01-01');
    expect(trendReq.request.params.get('to')).toBe('2026-01-31');
    trendReq.flush({ data: { points: [] } });
  });

  // ===== paging / sorting page-reset behaviour =====
  it('setPage() keeps the requested page (no reset to 1)', () => {
    service.setPage(3);
    const req = httpMock.expectOne(r => r.url === `${base}/errors`);
    expect(req.request.params.get('page')).toBe('3');
    req.flush({ data: [], meta: null });
    expect(service.query().page).toBe(3);
  });

  it('setSort() resets page back to 1', () => {
    service.setPage(3);
    httpMock.expectOne(r => r.url === `${base}/errors`).flush({ data: [], meta: null });

    service.setSort('error_code', 'asc');
    const req = httpMock.expectOne(r => r.url === `${base}/errors`);
    expect(req.request.params.get('page')).toBe('1');
    expect(req.request.params.get('sortBy')).toBe('error_code');
    expect(req.request.params.get('sortDir')).toBe('asc');
    req.flush({ data: [], meta: null });
  });

  // ===== exportCsv / saveBlob filename parsing =====
  it('exportCsv() parses the filename from content-disposition', () => {
    vi.spyOn(URL, 'createObjectURL').mockReturnValue('blob:fake');
    vi.spyOn(URL, 'revokeObjectURL').mockImplementation(() => { });
    const anchor = document.createElement('a');
    vi.spyOn(anchor, 'click').mockImplementation(() => { });
    vi.spyOn(document, 'createElement').mockReturnValue(anchor);

    service.exportCsv();
    const req = httpMock.expectOne(r => r.url === `${base}/errors/export`);
    expect(req.request.responseType).toBe('blob');

    req.flush(new Blob(['a,b']), {
      headers: { 'content-disposition': 'attachment; filename="errors_custom.csv"' },
    });

    expect(anchor.download).toBe('errors_custom.csv');
    expect(service.exporting()).toBe(false);
  });

  it('exportCsv() sets exportError on failure', () => {
    service.exportCsv();
    httpMock.expectOne(r => r.url === `${base}/errors/export`)
      .error(new ProgressEvent('error'), { status: 500, statusText: 'Server Error' });

    expect(service.exportError()).toBe('CSV export failed. Please retry.');
    expect(service.exporting()).toBe(false);
  });
});
