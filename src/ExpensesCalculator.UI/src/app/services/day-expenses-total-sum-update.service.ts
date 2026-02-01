import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface DayExpensesTotalSumUpdate {
  dayExpensesId: string;
  newSum: number;
}

@Injectable({
  providedIn: 'root'
})
export class DayExpensesTotalSumUpdateService {
  private dayExpensesTotalSumUpdated$ = new Subject<DayExpensesTotalSumUpdate>();

  // Observable for components to subscribe to
  get dayExpensesTotalSumUpdates() {
    return this.dayExpensesTotalSumUpdated$.asObservable();
  }

  // Method to emit day expenses total sum updates
  emitDayExpensesTotalSumUpdate(dayExpensesId: string, newSum: number): void {
    this.dayExpensesTotalSumUpdated$.next({ dayExpensesId, newSum });
  }
}
