import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Check {
  id: string;
  location: string;
  payer: string;
  photo?: Uint8Array | null;
  dayExpensesId: string;
  totalSum: number;
}

export interface DeleteCheckResponse {
  dayExpensesTotalSum: number;
}

@Injectable({
  providedIn: 'root'
})
export class ChecksService {
  private apiUrl = `${environment.apiUrl}/`;

  constructor(private http: HttpClient) { }

  getAllDayExpensesChecks(dayExpensesId: string): Observable<Check[]> {
    return this.http.get<Check[]>(`${this.apiUrl}Checks/day-expenses/${dayExpensesId}`);
  }

  createCheck(location: string, payer: string, dayExpensesId: string) {
    const body = {
      location: location,
      payer: payer,
      dayExpensesId: dayExpensesId
    };

    return this.http.post<Check>(`${this.apiUrl}Checks`, body);
  }

  editCheck(id: string, location: string, payer: string) {
    const body = {
      id: id,
      location: location,
      payer: payer
    };

    return this.http.put<Check>(`${this.apiUrl}Checks`, body);
  }

  deleteCheck(id: string) {
    return this.http.delete<DeleteCheckResponse>(`${this.apiUrl}Checks/${id}`);
  }
}
