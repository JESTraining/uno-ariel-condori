import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { CatalogItem } from '../data/models/catalog.model';

@Injectable({
  providedIn: 'root'
})

@Injectable({ providedIn: 'root' })
export class CatalogService extends ApiService {
  constructor() {
    super('api/catalog');
  }
  
  getCategories(): Observable<CatalogItem[]> {
    return this.get<CatalogItem[]>('categories');
  }

  getLocations(): Observable<CatalogItem[]> {
    return this.get<CatalogItem[]>('locations');
  }
}
