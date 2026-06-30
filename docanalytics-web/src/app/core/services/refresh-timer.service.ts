import { DestroyRef, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EMPTY, fromEvent, merge, of, timer } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class RefreshTimerService {
  /** Fires onTick now + every intervalMs. Pauses while tab hidden; re-fires on return. */
  start(intervalMs: number, onTick: () => void, destroyRef: DestroyRef): void {
    const visible$ = merge(
      of(!document.hidden),
      fromEvent(document, 'visibilitychange').pipe(map(() => !document.hidden)),
    );
    visible$
      .pipe(
        switchMap(visible => (visible ? timer(0, intervalMs) : EMPTY)),
        takeUntilDestroyed(destroyRef),
      )
      .subscribe(() => onTick());
  }
}
