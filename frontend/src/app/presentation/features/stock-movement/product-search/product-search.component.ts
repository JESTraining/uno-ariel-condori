import { HttpClient } from '@angular/common/http';
import { Component, inject, output, signal } from '@angular/core';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { tap, filter, debounceTime, distinctUntilChanged, switchMap, map } from 'rxjs';
import { ProductListItemDto } from '../../../../data/dto/product-list-item.dto';
import { ProductService } from '../../../../services/product.service';

@Component({
  selector: 'app-product-search',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './product-search.component.html',
  styleUrl: './product-search.component.scss'
})

export class ProductSearchComponent {
  private http = inject(HttpClient);
  private productService = inject(ProductService);
  
  productSelected = output<ProductListItemDto>();
  
  searchControl = new FormControl('', { nonNullable: true });
  results = signal<ProductListItemDto[]>([]);
  loading = signal(false);
  showResults = signal(false);

  constructor() {
    this.searchControl.valueChanges.pipe(
      tap(() => {
        if (this.searchControl.value.trim().length < 2) {
          this.results.set([]);
        }
      }),
      filter(text => text.trim().length >= 2),
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => this.loading.set(true)),
      switchMap(query => 
        this.productService.getProducts({ search: query, pageSize: 15, page: 1 })
      ),
      map(response => response.items),
      tap(() => this.loading.set(false))
    ).subscribe({
      next: (items) => this.results.set(items),
      error: () => {
        this.loading.set(false);
        this.results.set([]);
      }
    });
  }

  onFocus(): void {
    this.showResults.set(true);
  }

  onBlur(): void {
    this.showResults.set(false);
  }

  selectProduct(product: ProductListItemDto): void {
    this.searchControl.setValue(product.name, { emitEvent: false });
    this.results.set([]);
    this.showResults.set(false);
    
    this.productSelected.emit(product);
  }
}
