import { HttpClient, HttpParams } from '@angular/common/http';
import { API_BASE_URL } from '../app.config';
import { inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export abstract class ApiService {
  protected readonly http = inject(HttpClient);
  protected readonly baseUrl = inject(API_BASE_URL);

  constructor(protected readonly resourcePath: string) {}

  protected buildUrl(path?: string) {
    const root = `${this.baseUrl}/${this.resourcePath}`;
    return path ? `${root}/${path}` : root;
  }

  protected get<T>(path?: string, params?: HttpParams): Observable<T> {
    return this.http.get<T>(this.buildUrl(path), { params }).pipe(catchError(this.handleError));
  }

  protected post<T>(body: any, path?: string): Observable<T> {
    return this.http.post<T>(this.buildUrl(path), body).pipe(catchError(this.handleError));
  }

  protected put<T>(body: any, path?: string): Observable<T> {
    return this.http.put<T>(this.buildUrl(path), body).pipe(catchError(this.handleError));
  }

  protected delete<T>(path?: string): Observable<T> {
    return this.http.delete<T>(this.buildUrl(path)).pipe(catchError(this.handleError));
  }

  protected handleError = (err: any) => {
    return throwError(() => err);
  };
}

