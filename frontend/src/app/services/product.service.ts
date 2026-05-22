import {inject, Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {PagedResult} from '../data/models/page-result.model';
import {ProductListItemDto} from '../data/dto/product-list-item.dto';
import {ProductQuery} from '../data/dto/product-query.dto';
import {Observable} from 'rxjs';
import {ProductDetailsDto} from '../data/dto/product-details.dto';
import {CreateProductRequest} from '../data/requests/create-product.request';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class ProductService extends ApiService {
  constructor() {
    super('api/products');
  }

  getProducts(query: ProductQuery): Observable<PagedResult<ProductListItemDto>> {
    let params = new HttpParams()
      .set('page', query.page.toString())
      .set('pageSize', query.pageSize.toString());

    if (query.search) params = params.set('search', query.search);
    if (query.category) params = params.set('category', query.category);
    if (query.location) params = params.set('location', query.location);

    return this.get<PagedResult<ProductListItemDto>>(undefined, params);
  }

  getProductById(id: string): Observable<ProductDetailsDto> {
    return this.get<ProductDetailsDto>(id);
  }

  createProduct(product: CreateProductRequest): Observable<ProductDetailsDto> {
    return this.post<ProductDetailsDto>(product);
  }

  updateProduct(id: string, product: Partial<CreateProductRequest>): Observable<ProductDetailsDto> {
    return this.put<ProductDetailsDto>(product, id);
  }

  deleteProduct(id: string): Observable<void> {
    return this.delete<void>(id);
  }
}
