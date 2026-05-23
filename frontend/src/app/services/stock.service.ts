import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductDetailsDto } from '../data/dto/product-details.dto';
import { StockMovementRequest } from '../data/requests/stock-movement.request';
import { ApiService } from './api.service';
import { StockMovementResultDto } from '../data/dto/stock-movement.result.dto';

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
}