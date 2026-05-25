import { Injectable, signal } from '@angular/core';

export interface ToastMessage {
  id: number;
  message: string;
  type: 'success' | 'error' | 'warning';
}

@Injectable({ providedIn: 'root' })
export class ToastService {

  toasts = signal<ToastMessage[]>([]);
  private counter = 0;

  show(message: string, type: 'success' | 'error' | 'warning' = 'success', duration = 5000): void {
    const id = this.counter++;
    
    this.toasts.update(current => [...current, { id, message, type }]);

    setTimeout(() => {
      this.toasts.update(current => current.filter(t => t.id !== id));
    }, duration);
  }

  remove(id: number): void {
    this.toasts.update(current => current.filter(t => t.id !== id));
  }
}