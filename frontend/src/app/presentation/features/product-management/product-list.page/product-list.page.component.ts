import {Component, inject} from '@angular/core';
import {ProductStore} from '../../../../stores/product.store';
import {ProductFiltersComponent} from '../product-filters/product-filters.component';
import {ProductTableComponent} from '../product-table/product-table.component';

@Component({
  selector: 'app-product-list.page',
  standalone: true,
  imports: [
    ProductFiltersComponent,
    ProductTableComponent
  ],
  templateUrl: './product-list.page.component.html',
  styleUrl: './product-list.page.component.scss'
})
export class ProductListPage {
  protected store = inject(ProductStore);

  onPageChange(page: number): void {
    this.store.updateFilters({ page });
  }

  onDeleteProduct(id: string): void {
    if (confirm('¿Are you sure you want to delete this product?')) {
      this.store.deleteProduct(id);
    }
  }

  onEditProduct(id: string): void {
    console.log('edit product by ID:', id);
  }

  openCreateForm(): void {
    console.log('open new form');
  }
}
