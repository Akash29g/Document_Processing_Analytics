import { TestBed } from '@angular/core/testing';
import { ToastService } from './toast.service';

describe('ToastService', () => {
  let service: ToastService;

  beforeEach(() => {
    vi.useFakeTimers();
    TestBed.configureTestingModule({});
    service = TestBed.inject(ToastService);
  });

  afterEach(() => {
    vi.clearAllTimers();
    vi.useRealTimers();
  });

  it('show() adds a toast with text and type', () => {
    service.show('Hello', 'info');
    expect(service.toasts().length).toBe(1);
    expect(service.toasts()[0].text).toBe('Hello');
    expect(service.toasts()[0].type).toBe('info');
  });

  it('assigns incrementing ids', () => {
    service.show('a');
    service.show('b');
    const [t1, t2] = service.toasts();
    expect(t2.id).toBeGreaterThan(t1.id);
  });

  it('error() adds an error toast', () => {
    service.error('Boom');
    expect(service.toasts()[0].type).toBe('error');
  });

  it('success() adds a success toast', () => {
    service.success('Yay');
    expect(service.toasts()[0].type).toBe('success');
  });

  it('warning() adds a warning toast', () => {
    service.warning('Careful');
    expect(service.toasts()[0].type).toBe('warning');
  });

  it('dismiss() removes a toast by id', () => {
    service.show('a');
    const id = service.toasts()[0].id;
    service.dismiss(id);
    expect(service.toasts().length).toBe(0);
  });

  it('auto-dismisses after the default 5000ms', () => {
    service.show('bye');
    vi.advanceTimersByTime(4999);
    expect(service.toasts().length).toBe(1);
    vi.advanceTimersByTime(1);
    expect(service.toasts().length).toBe(0);
  });
});
