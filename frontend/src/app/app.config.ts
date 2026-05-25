import { ApplicationConfig, provideZoneChangeDetection, InjectionToken } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ApiInterceptor } from './interceptors/api-interceptor.service';

export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL');

function resolveApiBaseUrl(): string {
  const runtimeEnv = (window as Window & { __env?: { API_BASE_URL?: string } }).__env;
  return runtimeEnv?.API_BASE_URL ?? 'https://localhost:7036';
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: API_BASE_URL, useFactory: resolveApiBaseUrl },
    { provide: HTTP_INTERCEPTORS, useClass: ApiInterceptor, multi: true }
  ]
};
