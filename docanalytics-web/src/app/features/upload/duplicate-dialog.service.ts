import { Injectable, signal } from '@angular/core';

export type DuplicateChoice = 'replace' | 'rename' | 'skip';

@Injectable({ providedIn: 'root' })
export class DuplicateDialogService {
  /** null = closed; string = filename being asked about */
  readonly pending = signal<string | null>(null);

  private resolver: ((c: DuplicateChoice) => void) | null = null;

  /** Awaited by the upload loop — resolves when a button is clicked. */
  ask(fileName: string): Promise<DuplicateChoice> {
    this.pending.set(fileName);
    return new Promise<DuplicateChoice>((resolve) => (this.resolver = resolve));
  }

  choose(choice: DuplicateChoice): void {
    this.pending.set(null);
    this.resolver?.(choice);
    this.resolver = null;
  }
}
