import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { API_BASE_URL } from '../app.config';

@Injectable({ providedIn: 'root' })
export class ApiInterceptor implements HttpInterceptor {
  private baseUrl = inject(API_BASE_URL);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const isAbsolute = /^https?:\/\//i.test(req.url);
    const url = isAbsolute ? req.url : `${this.baseUrl}${req.url.startsWith('/') ? '' : '/'}${req.url}`;

    const cloned = req.clone({ url });

    return next.handle(cloned).pipe(
      catchError(err => {
        console.error('HTTP error', err);
        return throwError(() => err);
      })
    );
  }
}

