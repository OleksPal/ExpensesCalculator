import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Check {
  id: string;
  location: string;
  payer: string;
  photo?: Uint8Array | null;
  dayExpensesId: string;
  totalSum: number;
}

@Injectable({
  providedIn: 'root'
})
export class ChecksService {
  private apiUrl = 'https://localhost:7054/api/';

  constructor(private http: HttpClient) { }

  getAllDayExpensesChecks(dayExpensesId: string): Observable<Check[]> {
    return this.http.get<Check[]>(`${this.apiUrl}Checks/day-expenses/${dayExpensesId}`);
  }

  getCheck(id: string): Observable<Check> {
    return this.http.get<Check>(`${this.apiUrl}Checks/${id}`);
  }

  createCheck(location: string, payer: string, dayExpensesId: string) {
    const body = {
      location: location,
      payer: payer,
      dayExpensesId: dayExpensesId
    };

    return this.http.post<void>(`${this.apiUrl}Checks`, body);
  }

  editCheck(id: string, location: string, payer: string, dayExpensesId: string) {
    const body = {
      id: id,
      location: location,
      payer: payer,
      dayExpensesId: dayExpensesId
    };

    return this.http.put<void>(`${this.apiUrl}Checks`, body);
  }

  deleteCheck(id: string) {
    return this.http.delete<void>(`${this.apiUrl}Checks/${id}`);
  }
}
