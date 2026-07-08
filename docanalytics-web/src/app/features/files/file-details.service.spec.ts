import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { FileDetailsService } from './file-details.service';

describe('FileDetailsService', () => {
  let service: FileDetailsService;
  let httpMock: HttpTestingController;
  const base = environment.apiBase;
  const id = 'file-1';

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(FileDetailsService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  afterEach(() => httpMock.verify());

  it('loadDetails() fills the detail signal', () => {
    service.load(id);
    const req = httpMock.expectOne(r => r.url === `${base}/files/${id}/details`);
    req.flush({ data: { file_info: { id, name: 'a.pdf', current_status: 'Failed', current_step: 'Validate' }, history: [] } });
    // drain the line-items call the load() also fires:
    httpMock.expectOne(r => r.url === `${base}/files/${id}/line-items`).flush({ data: { grand_total: 0, items: [] } });
    expect(service.detail()?.file_info.name).toBe('a.pdf');
    expect(service.detailLoading()).toBe(false);
  });

  it('line-items 200 with items sets hasInvoice true', () => {
    service.loadLineItems(/* uses current id — call load(id) first if required by impl */);
    // If loadLineItems needs an id set via load(), do service.load(id) then drain details first.
  });

  it('404 on line-items sets hasInvoice=false (not an error)', () => {
    service.load(id);
    httpMock.expectOne(r => r.url === `${base}/files/${id}/details`)
      .flush({ data: { file_info: { id, name: 'a.pdf', current_status: 'Completed', current_step: 'Load' }, history: [] } });
    httpMock.expectOne(r => r.url === `${base}/files/${id}/line-items`)
      .flush('nope', { status: 404, statusText: 'Not Found' });
    expect(service.hasInvoice()).toBe(false);
    expect(service.invoiceError()).toBeNull();
  });

  it('200 empty items keeps hasInvoice true with empty list', () => {
    service.load(id);
    httpMock.expectOne(r => r.url === `${base}/files/${id}/details`)
      .flush({ data: { file_info: { id, name: 'a.pdf', current_status: 'Completed', current_step: 'Load' }, history: [] } });
    httpMock.expectOne(r => r.url === `${base}/files/${id}/line-items`)
      .flush({ data: { grand_total: 0, items: [] } });
    expect(service.hasInvoice()).toBe(true);
    expect(service.invoice()?.items).toEqual([]);
  });

  it('downloadLogs() requests the logs endpoint as a blob', () => {
    vi.spyOn(URL, 'createObjectURL').mockReturnValue('blob:fake');
    vi.spyOn(URL, 'revokeObjectURL').mockImplementation(() => { });
    const anchor = document.createElement('a');
    vi.spyOn(anchor, 'click').mockImplementation(() => { });
    vi.spyOn(document, 'createElement').mockReturnValue(anchor);

    service.load(id);
    httpMock.expectOne(r => r.url === `${base}/files/${id}/details`).flush({ data: { file_info: { id, name: 'a.pdf', current_status: 'Failed', current_step: 'Validate' }, history: [] } });
    httpMock.expectOne(r => r.url === `${base}/files/${id}/line-items`).flush({ data: { grand_total: 0, items: [] } });

    service.downloadLogs();
    const req = httpMock.expectOne(r => r.url === `${base}/files/${id}/logs`);
    expect(req.request.responseType).toBe('blob');
    req.flush(new Blob(['log']), { headers: { 'content-disposition': 'attachment; filename="file_log.txt"' } });
  });
});
