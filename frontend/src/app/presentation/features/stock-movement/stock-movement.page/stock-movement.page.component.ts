import { Component, effect, inject, signal, OnInit } from '@angular/core';
import { ProductListItemDto } from '../../../../data/dto/product-list-item.dto';
import { ProductStore } from '../../../../stores/product.store';
import { StockStore } from '../../../../stores/stock.store';
import { MovementFormComponent } from '../movement-form/movement-form.component';
import { MovementHistoryComponent } from '../movement-history/movement-history.component';
import { StockMovementRequest } from '../../../../data/requests/stock-movement.request';
import { ProductSearchComponent } from '../product-search/product-search.component';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../../services/product.service';

@Component({
  selector: 'app-stock-movement.page',
  standalone: true,
  imports: [MovementFormComponent, MovementHistoryComponent, ProductSearchComponent],
  templateUrl: './stock-movement.page.component.html',
  styleUrl: './stock-movement.page.component.scss'
})

export class StockMovementPage implements OnInit {
  protected catalogStore = inject(ProductStore);
  protected stockStore = inject(StockStore);
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);

  activeProduct = signal<ProductListItemDto | null>(null);

  constructor() {
    effect(() => {
      const currentSelection = this.activeProduct();
      if (currentSelection) {
        const matchingProduct = this.catalogStore.products().find(p => p.id === currentSelection.id);
        if (matchingProduct) {
          this.activeProduct.set(matchingProduct as unknown as ProductListItemDto);
        }
      }
    }, { allowSignalWrites: true });
  }

  ngOnInit(): void {
    const productId = this.route.snapshot.queryParamMap.get('productId');
    if (productId) {
      this.productService.getProductById(productId).subscribe({
        next: (product) => {
          const listItem = { ...(product as any), isLowStock: product.currentStock < product.reorderThreshold } as ProductListItemDto;
          this.activeProduct.set(listItem);
          this.stockStore.loadHistory(product.id);
        },
        error: () => console.error('Failed to load product for stock movement page')
      });
    }
  }

  onProductSelect(product: ProductListItemDto): void {
    this.activeProduct.set(product);
    this.stockStore.loadHistory(product.id);
  }

  onPostMovement(request: StockMovementRequest): void {
    this.stockStore.executeMovement(request, () => {
      this.catalogStore.loadProducts(this.catalogStore.filters());
    });
  }
}