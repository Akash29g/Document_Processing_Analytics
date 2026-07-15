import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { ActivityLogService } from './activity-log.service';

describe('ActivityLogService', () => {
  let service: ActivityLogService;
  let httpMock: HttpTestingController;
  const base = environment.apiBase;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ActivityLogService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  afterEach(() => httpMock.verify());

  it('load() sends default paging + sort', () => {
    service.load();
    const req = httpMock.expectOne((r) => r.url === `${base}/activity-log`);
    expect(req.request.params.get('page')).toBe('1');
    expect(req.request.params.get('pageSize')).toBe('20');
    req.flush({ data: [], meta: { total_count: 0, page: 1, page_size: 20, total_pages: 0 } });
    expect(service.rows()).toEqual([]);
  });

  it('setFilters() forwards eventType/from/to', () => {
    service.setFilters({ eventType: 'FILE_STATE_CHANGED', from: '2026-01-01', to: '2026-01-31' });
    const req = httpMock.expectOne((r) => r.url === `${base}/activity-log`);
    expect(req.request.params.get('eventType')).toBe('FILE_STATE_CHANGED');
    expect(req.request.params.get('from')).toBe('2026-01-01');
    expect(req.request.params.get('to')).toBe('2026-01-31');
    req.flush({ data: [], meta: null });
  });

  it('setSort() resets page to 1', () => {
    service.setPage(4);
    httpMock.expectOne((r) => r.url === `${base}/activity-log`).flush({ data: [], meta: null });
    service.setSort('event_type', 'asc');
    const req = httpMock.expectOne((r) => r.url === `${base}/activity-log`);
    expect(req.request.params.get('page')).toBe('1');
    expect(req.request.params.get('sortBy')).toBe('event_type');
    req.flush({ data: [], meta: null });
  });

  it('fills rows + meta on success', () => {
    service.load();
    httpMock
      .expectOne((r) => r.url === `${base}/activity-log`)
      .flush({
        data: [{ ts: '2026-01-01', event_type: 'X', entity: 'a.pdf', actor: 'system' }],
        meta: { total_count: 1, page: 1, page_size: 20, total_pages: 1 },
      });
    expect(service.rows().length).toBe(1);
    expect(service.meta()?.total_count).toBe(1);
  });
});
