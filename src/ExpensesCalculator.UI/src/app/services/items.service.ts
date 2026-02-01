import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Item {
  id: string;
  name: string;
  description?: string | null;
  price: number;
  amount: number;
  rating: number;
  tags: string[];
  users: string[];
  checkId: string;
  dayExpensesId?: string;
  canEdit?: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalPages: number;
  totalCount: number;
  currentUserId?: string;
}

export interface ItemsRequest {
  sortColumn?: string;
  sortOrder?: string;
  filterText?: string;
  filterCriteria?: string;
  pageNumber?: number;
  pageSize?: number;
  onlyMyItems?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ItemsService {
  private apiUrl = 'https://localhost:7054/api/';

  constructor(private http: HttpClient) { }

  getAllCheckItems(checkId: string): Observable<Item[]> {
    return this.http.get<Item[]>(`${this.apiUrl}Items/check/${checkId}`);
  }

  getAllUserItems(request: ItemsRequest): Observable<PagedResult<Item>> {
    let params = new HttpParams();

    if (request.sortColumn) params = params.set('sortColumn', request.sortColumn);
    if (request.sortOrder) params = params.set('sortOrder', request.sortOrder);
    if (request.filterText) params = params.set('filterText', request.filterText);
    if (request.filterCriteria) params = params.set('filterCriteria', request.filterCriteria);
    if (request.pageNumber) params = params.set('pageNumber', request.pageNumber.toString());
    if (request.pageSize) params = params.set('pageSize', request.pageSize.toString());

    return this.http.get<PagedResult<Item>>(`${this.apiUrl}Recommendations`, { params });
  }

  getItem(id: string): Observable<Item> {
    return this.http.get<Item>(`${this.apiUrl}Items?id=${id}`);
  }

  createItem(item: Item) {
    return this.http.post<number>(`${this.apiUrl}Items`, item);
  }

  editItem(item: Item) {
    return this.http.put<number>(`${this.apiUrl}Items`, item);
  }

  deleteItem(id: string) {
    return this.http.delete<number>(`${this.apiUrl}Items?id=${id}`);
  }

  updateItemRatingAndTags(id: string, rating: number, tags: string[]) {
    return this.http.put(`${this.apiUrl}Recommendations/${id}/rating-tags`, { rating, tags });
  }

  getAllDistinctTags(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}Recommendations/tags`);
  }
}
