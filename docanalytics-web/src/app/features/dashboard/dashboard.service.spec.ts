import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { DashboardService } from './dashboard.service';

describe('DashboardService (throughput + distribution)', () => {
  let service: DashboardService;
  let httpMock: HttpTestingController;
  const base = environment.apiBase;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(DashboardService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('loadThroughput() fills the throughput signal from data.points', () => {
    service.loadThroughput();
    const req = httpMock.expectOne(r => r.url === `${base}/dashboard/throughput`);
    expect(req.request.method).toBe('GET');
    req.flush({ data: { points: [{ label: 'd1', value: 3 }] } });
    expect(service.throughput().length).toBe(1);
    expect(service.throughputLoading()).toBe(false);
  });

  it('loadThroughput() defaults to [] when data is null', () => {
    service.loadThroughput();
    httpMock.expectOne(r => r.url === `${base}/dashboard/throughput`).flush({ data: null });
    expect(service.throughput()).toEqual([]);
  });

  it('loadThroughput() flips loading true immediately', () => {
    service.loadThroughput();
    expect(service.throughputLoading()).toBe(true);
    httpMock.expectOne(r => r.url === `${base}/dashboard/throughput`).flush({ data: { points: [] } });
  });

  it('loadThroughput() sets error on failure', () => {
    service.loadThroughput();
    httpMock.expectOne(r => r.url === `${base}/dashboard/throughput`)
      .flush('boom', { status: 500, statusText: 'Server Error' });
    expect(service.throughputError()).toBe('Could not load throughput.');
    expect(service.throughputLoading()).toBe(false);
  });

  it('loadStatusDistribution() fills the statusDistribution signal', () => {
    service.loadStatusDistribution();
    httpMock.expectOne(r => r.url === `${base}/dashboard/status-distribution`)
      .flush({ data: { points: [{ label: 'Completed', value: 7 }, { label: 'Failed', value: 2 }] } });
    expect(service.statusDistribution().length).toBe(2);
    expect(service.distributionLoading()).toBe(false);
  });

  it('loadStatusDistribution() sets error on failure', () => {
    service.loadStatusDistribution();
    httpMock.expectOne(r => r.url === `${base}/dashboard/status-distribution`)
      .flush('boom', { status: 500, statusText: 'Server Error' });
    expect(service.distributionError()).toBe('Could not load status distribution.');
    expect(service.distributionLoading()).toBe(false);
  });
});
