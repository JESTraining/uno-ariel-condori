import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductDetailsDto } from '../data/dto/product-details.dto';
import { StockMovementRequest } from '../data/requests/stock-movement.request';
import { ApiService } from './api.service';
import { StockMovementResultDto } from '../data/dto/stock-movement.result.dto';
import { ProductListItemDto } from '../data/dto/product-list-item.dto';
import { StockMovementDto } from '../data/dto/stock-movement.dto';

@Injectable({
  providedIn: 'root'
})

export class StockService extends ApiService {
  constructor() {
    super('api/stock');
  }

  saveMovement(request: StockMovementRequest): Observable<StockMovementResultDto> {
    return this.post<StockMovementResultDto>(request, 'movements');
  }

  getLowStockAlerts(): Observable<ProductListItemDto[]> {
    return this.get<ProductListItemDto[]>(`alert/low`);
  }

  getMovementHistory(productId: string): Observable<StockMovementDto[]> {
    return this.get<StockMovementDto[]>(`movements/${productId}`);
  }
}