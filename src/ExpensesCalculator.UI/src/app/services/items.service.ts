import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Item {
  id: string;
  name: string;
  comment?: string | null;
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
  tags?: string[];
}

export interface ItemResponse {
  id?: string;
  name?: string;
  comment?: string;
  price?: number;
  amount?: number;
  rating?: number;
  tags?: string[];
  users?: string[];
  checkId: string;
  checkTotalSum: number;
  dayExpensesTotalSum: number;
}

export interface DeleteItemResponse {
  checkTotalSum: number;
  dayExpensesTotalSum: number;
}

@Injectable({
  providedIn: 'root'
})
export class ItemsService {
  private apiUrl = `${environment.apiUrl}/`;

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
    if (request.onlyMyItems !== undefined) params = params.set('isOnlyMyItems', request.onlyMyItems.toString());
    if (request.tags && request.tags.length > 0) {
      request.tags.forEach(tag => {
        params = params.append('tags', tag);
      });
    }

    return this.http.get<PagedResult<Item>>(`${this.apiUrl}Recommendations`, { params });
  }

  getItem(id: string): Observable<Item> {
    return this.http.get<Item>(`${this.apiUrl}Items?id=${id}`);
  }

  createItem(item: Item) {
    return this.http.post<ItemResponse>(`${this.apiUrl}Items`, item);
  }

  editItem(item: Item) {
    return this.http.put<ItemResponse>(`${this.apiUrl}Items`, item);
  }

  deleteItem(id: string) {
    return this.http.delete<DeleteItemResponse>(`${this.apiUrl}Items/${id}`);
  }

  updateItemRatingAndTags(id: string, rating: number, tags: string[]) {
    return this.http.put(`${this.apiUrl}Recommendations/${id}/rating-tags`, { rating, tags });
  }

  getAllDistinctTags(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}Recommendations/tags`);
  }

  // Recommendation-specific methods
  createRecommendationItem(item: { name: string; comment?: string; price: number; amount: number; rating: number; tags: string[] }) {
    return this.http.post(`${this.apiUrl}Recommendations`, item);
  }

  editRecommendationItem(item: { id: string; name: string; comment?: string; price: number; amount: number; rating: number; tags: string[] }) {
    return this.http.put(`${this.apiUrl}Recommendations`, item);
  }

  deleteRecommendationItem(id: string) {
    return this.http.delete(`${this.apiUrl}Recommendations/${id}`);
  }
}
