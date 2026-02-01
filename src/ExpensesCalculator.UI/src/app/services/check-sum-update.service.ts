import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface CheckSumUpdate {
  checkId: string;
  newSum: number;
}

@Injectable({
  providedIn: 'root'
})
export class CheckSumUpdateService {
  private checkSumUpdated$ = new Subject<CheckSumUpdate>();

  // Observable for components to subscribe to
  get checkSumUpdates() {
    return this.checkSumUpdated$.asObservable();
  }

  // Method to emit check sum updates
  emitCheckSumUpdate(checkId: string, newSum: number): void {
    this.checkSumUpdated$.next({ checkId, newSum });
  }
}
