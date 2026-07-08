import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { ComparisonService } from './comparison.service';

describe('ComparisonService', () => {
  let service: ComparisonService;
  let httpMock: HttpTestingController;
  const url = `${environment.apiBase}/dashboard/throughput`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ComparisonService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('loadA() sends from/to and fills rangeA', async () => {
    const p = service.loadA('2026-01-01', '2026-01-31');
    const req = httpMock.expectOne(r => r.url === url);
    expect(req.request.params.get('from')).toBe('2026-01-01');
    expect(req.request.params.get('to')).toBe('2026-01-31');

    req.flush({ data: { points: [{ label: 'd1', value: 3 }, { label: 'd2', value: 4 }] } });
    await p;

    expect(service.rangeA().points.length).toBe(2);
    expect(service.rangeA().from).toBe('2026-01-01');
    expect(service.rangeA().loading).toBe(false);
  });

  it('loadA() flips loading true immediately (before the response)', () => {
    service.loadA('2026-01-01', '2026-01-31');
    expect(service.rangeA().loading).toBe(true);
    httpMock.expectOne(r => r.url === url).flush({ data: { points: [] } });
  });

  it('loadA() sets error on failure', async () => {
    const p = service.loadA('2026-01-01', '2026-01-31');
    httpMock.expectOne(r => r.url === url)
      .flush('boom', { status: 500, statusText: 'Server Error' });
    await p;

    expect(service.rangeA().error).toBe('Could not load this range. Retry.');
    expect(service.rangeA().loading).toBe(false);
  });

  it('loadB() updates rangeB independently of rangeA', async () => {
    const p = service.loadB('2026-02-01', '2026-02-28');
    httpMock.expectOne(r => r.url === url).flush({ data: { points: [{ label: 'x', value: 9 }] } });
    await p;

    expect(service.rangeB().points.length).toBe(1);
    expect(service.rangeA().points.length).toBe(0);   // A untouched
  });

  it('omits from/to params when empty', async () => {
    const p = service.loadA('', '');
    const req = httpMock.expectOne(r => r.url === url);
    expect(req.request.params.has('from')).toBe(false);
    expect(req.request.params.has('to')).toBe(false);
    req.flush({ data: { points: [] } });
    await p;
  });

  it('total() sums point values', () => {
    expect(service.total([{ label: 'a', value: 2 }, { label: 'b', value: 5 }])).toBe(7);
  });

  it('total() returns 0 for an empty array', () => {
    expect(service.total([])).toBe(0);
  });
});
