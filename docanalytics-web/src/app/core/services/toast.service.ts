import { Injectable, signal } from '@angular/core';

export interface Toast {
  id: number;
  text: string;
  type: 'info' | 'warning' | 'error' | 'success';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly toasts = signal<Toast[]>([]);
  private seq = 0;

  show(text: string, type: Toast['type'] = 'info', timeoutMs = 5000): void {
    const id = ++this.seq;
    this.toasts.update((list) => [...list, { id, text, type }]);
    setTimeout(() => this.dismiss(id), timeoutMs);
  }

  error(text: string): void {
    this.show(text, 'error', 7000);
  }
  success(text: string): void {
    this.show(text, 'success');
  }
  warning(text: string): void {
    this.show(text, 'warning');
  }

  dismiss(id: number): void {
    this.toasts.update((list) => list.filter((t) => t.id !== id));
  }
}
