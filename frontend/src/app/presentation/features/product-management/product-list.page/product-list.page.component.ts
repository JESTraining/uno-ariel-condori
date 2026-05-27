import {Component, inject, signal} from '@angular/core';
import {ProductStore} from '../../../../stores/product.store';
import {ProductFiltersComponent} from '../product-filters/product-filters.component';
import {ProductTableComponent} from '../product-table/product-table.component';
import { ProductFormComponent } from '../product-form/product-form.component';
import { ConfirmModalComponent } from '../../../shared/confirm-modal/confirm-modal.component';
import { ProductDetailsDto } from '../../../../data/dto/product-details.dto';
import { CreateProductRequest } from '../../../../data/requests/create-product.request';
import { ToastService } from '../../../shared/toast/toast.service';

@Component({
  selector: 'app-product-list.page',
  standalone: true,
  imports: [
    ProductFiltersComponent,
    ProductTableComponent,
    ProductFormComponent,
    ConfirmModalComponent
  ],
  templateUrl: './product-list.page.component.html',
  styleUrl: './product-list.page.component.scss'
})

export class ProductListPage {
  protected store = inject(ProductStore);
  private toastService = inject(ToastService);

  isFormOpen = signal<boolean>(false);
  isDeleteModalOpen = signal<boolean>(false);
  selectedProduct = signal<ProductDetailsDto | null>(null);
  
  #productIdToDelete: string | null = null;

  onPageChange(page: number): void {
    this.store.updateFilters({ page });
  }

  openCreateForm(): void {
    this.selectedProduct.set(null);
    this.isFormOpen.set(true);
  }

  onEditProduct(id: string): void {
    const item = this.store.products().find(p => p.id === id);
    if (item) {
      this.selectedProduct.set({
        ...item,
        isActive: true,
      });
      this.isFormOpen.set(true);
    }
  }

  onSaveProduct(request: CreateProductRequest): void {
    const product = this.selectedProduct();
    if (product) {
      this.store.updateProduct(product.id, request, () => this.closeForm(), 
        (err: Error) => this.toastService.show('Error: ' + err.message, 'error'));
    } else {
      this.store.createProduct(request, () => this.closeForm(), 
        (err: Error) => this.toastService.show('Error: ' + err.message, 'error'));
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedProduct.set(null);
  }

  onOpenDeleteModal(id: string): void {
    this.#productIdToDelete = id;
    this.isDeleteModalOpen.set(true);
  }

  onConfirmDelete(): void {
    if (this.#productIdToDelete) {
      this.store.deleteProduct(this.#productIdToDelete);
      this.#productIdToDelete = null;
    }
    this.isDeleteModalOpen.set(false);
  }
}