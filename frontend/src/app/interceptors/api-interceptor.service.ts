import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
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
        let processedErrorMessage = 'An unexpected server error occurred.';
        console.error('API error intercepted:', err);
        if(err instanceof HttpErrorResponse) {
          if(err.status === 409) {
            processedErrorMessage = err.error?.message || 'Conflict error occurred.';
          } else {
            processedErrorMessage = err.error?.message || `HTTP error ${err.status}: ${err.statusText}` || processedErrorMessage;
          }
        }

        console.error('HTTP error', err);
        return throwError(() => new Error(processedErrorMessage));
      })
    );
  }
}

