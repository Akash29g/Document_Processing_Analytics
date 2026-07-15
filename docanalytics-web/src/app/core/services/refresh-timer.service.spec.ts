import { DestroyRef, inject } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { RefreshTimerService } from './refresh-timer.service';

describe('RefreshTimerService', () => {
  let service: RefreshTimerService;
  let destroyRef: DestroyRef;

  beforeEach(() => {
    vi.useFakeTimers(); // freeze time — we control the clock
    TestBed.configureTestingModule({});
    service = TestBed.inject(RefreshTimerService);
    // start() needs a real DestroyRef → grab one from an injection context
    destroyRef = TestBed.runInInjectionContext(() => inject(DestroyRef));
  });

  afterEach(() => {
    vi.clearAllTimers();
    vi.useRealTimers(); // always restore real time
  });

  it('does not fire onTick until time advances', () => {
    const onTick = vi.fn();
    service.start(1000, onTick, destroyRef);
    expect(onTick).not.toHaveBeenCalled(); // timer is async — nothing yet
  });

  it('fires onTick on start and then on each interval', () => {
    const onTick = vi.fn();
    service.start(1000, onTick, destroyRef);

    vi.advanceTimersByTime(3000); // initial tick + ~3 intervals

    expect(onTick.mock.calls.length).toBeGreaterThanOrEqual(3);
  });

  it('keeps firing as more time passes', () => {
    const onTick = vi.fn();
    service.start(1000, onTick, destroyRef);

    vi.advanceTimersByTime(1000);
    const afterOne = onTick.mock.calls.length;

    vi.advanceTimersByTime(1000);
    expect(onTick.mock.calls.length).toBeGreaterThan(afterOne);
  });
});
