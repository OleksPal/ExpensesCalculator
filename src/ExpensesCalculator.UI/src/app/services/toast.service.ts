import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Toast {
  header: string;
  message: string;
  type: 'success' | 'error' | 'warning' | 'info';
  timestamp: string;
  show: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private toastSubject = new BehaviorSubject<Toast | null>(null);
  public toast$: Observable<Toast | null> = this.toastSubject.asObservable();

  show(header: string, message: string, type: 'success' | 'error' | 'warning' | 'info' = 'info'): void {
    const now = new Date();
    const timestamp = now.toLocaleString('en-US', {
      month: 'short',
      day: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false
    });

    const toast: Toast = {
      header,
      message,
      type,
      timestamp,
      show: true
    };

    this.toastSubject.next(toast);

    // Auto-hide after 5 seconds
    setTimeout(() => {
      this.hide();
    }, 5000);
  }

  success(header: string, message: string): void {
    this.show(header, message, 'success');
  }

  error(header: string, message: string): void {
    this.show(header, message, 'error');
  }

  warning(header: string, message: string): void {
    this.show(header, message, 'warning');
  }

  info(header: string, message: string): void {
    this.show(header, message, 'info');
  }

  hide(): void {
    const currentToast = this.toastSubject.value;
    if (currentToast) {
      this.toastSubject.next({ ...currentToast, show: false });

      // Clear the toast after animation
      setTimeout(() => {
        this.toastSubject.next(null);
      }, 300);
    }
  }
}
