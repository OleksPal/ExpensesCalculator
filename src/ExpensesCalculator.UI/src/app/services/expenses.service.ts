import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DayExpenses {
  id: string;
  date: Date;
  location: string;
  participants: string[];
  peopleWithAccess: string[];
  totalSum: number;
}
export interface PagedDayExpensesResult {
  items: DayExpenses[];
  totalPages: number;
  fromDate: string;
  toDate: string;
}

export interface ShareDayExpensesResponse {
  isSuccess: boolean;
  error: string;
}

@Injectable({
  providedIn: 'root'
})
export class ExpensesService {
  private apiUrl = 'https://localhost:7054/api/';

  constructor(private http: HttpClient) { }

  getAllDayExpenses(sortColumn: string = 'Date', sortOrder: string = 'desc', 
    filterText: string = '', filterCriteria: string = 'all',
    fromDate: string = '', toDate: string = '',
    pageNumber: number = 1, pageSize: number = 10): Observable<PagedDayExpensesResult> {
    const params = new HttpParams()
      .set('sortColumn', sortColumn)
      .set('sortOrder', sortOrder)
      .set('filterText', filterText)
      .set('filterCriteria', filterCriteria)
      .set('fromDate', fromDate)
      .set('toDate', toDate)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedDayExpensesResult>(`${this.apiUrl}DayExpenses`, { params })
  }

  getDayExpenses(id: string): Observable<DayExpenses> {
    return this.http.get<DayExpenses>(`${this.apiUrl}DayExpenses/${id}`)
  }

  createDayExpenses(date: string, location: string, participants: string[]) {
    const body = {
      date: date,
      location: location,
      participants: participants
    }

    return this.http.post<void>(`${this.apiUrl}DayExpenses`, body);
  }

  editDayExpenses(id: string, date: string, location: string, participants: string[]) {
    const body = {
      id: id,
      date: date,
      location: location,
      participants: participants
    }

    return this.http.put<void>(`${this.apiUrl}DayExpenses`, body);
  }

  deleteDayExpenses(id: string) {   
    return this.http.delete<void>(`${this.apiUrl}DayExpenses/${id}`);
  }

  shareDayExpenses(id: string, newUserWithAccess: string) {   
    return this.http.post<ShareDayExpensesResponse>(`${this.apiUrl}DayExpenses/${id}/share`, { newUserWithAccess });
  }
}
