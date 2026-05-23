import {pipe, switchMap, tap} from 'rxjs';
import {ProductService} from '../services/product.service';
import {ProductState} from '../data/State/product.state';
import {patchState, signalStore, withHooks, withMethods, withState} from '@ngrx/signals';
import {inject} from '@angular/core';
import {ProductQuery} from '../data/dto/product-query.dto';
import {rxMethod} from '@ngrx/signals/rxjs-interop';
import { CreateProductRequest } from '../data/requests/create-product.request';

const initialState: ProductState = {
  products: [],
  totalCount: 0,
  filters: { page: 1, pageSize: 10, search: '', category: '' },
  selectedProduct: null,
  loading: false,
  error: null,
};

export const ProductStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),

  withMethods((store: any, productService: ProductService = inject(ProductService)) => {

    const loadProducts = rxMethod<ProductQuery>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((filters: ProductQuery) =>
          productService.getProducts(filters).pipe(
            tap({
              next: (result) => patchState(store, {
                products: result.items,
                totalCount: result.totalCount,
                loading: false
              }),
              error: () => patchState(store, { error: 'Error loading products', loading: false })
            })
          )
        )
      )
    );

    return {
      loadProducts,

      createProduct(request: CreateProductRequest, onSuccess: () => void): void {
        patchState(store, { loading: true });
        productService.createProduct(request).subscribe({
          next: () => {
            loadProducts(store.filters());
            onSuccess();
          },
          error: () => patchState(store, { error: 'Error creating product', loading: false })
        });
      },

      updateProduct(id: string, request: Partial<CreateProductRequest>, onSuccess: () => void): void {
        patchState(store, { loading: true });
        productService.updateProduct(id, request).subscribe({
          next: () => {
            loadProducts(store.filters());
            onSuccess();
          },
          error: () => patchState(store, { error: 'Error updating product', loading: false })
        });
      },

      updateFilters(partialFilter: Partial<ProductQuery>): void {
        patchState(store, (state: ProductState) => ({
          filters: { ...state.filters, ...partialFilter, page: partialFilter.page ?? 1 }
        }));
      },

      deleteProduct(id: string): void {
        patchState(store, { loading: true });
        productService.deleteProduct(id).subscribe({
          next: () => loadProducts(store.filters()),
          error: () => patchState(store, { error: 'There was an error deleting the product.', loading: false })
        });
      }
    };
  }),

  withHooks({
    onInit(store) {
      // Every time call 'updateFilters()', the 'loadProducts' method will be triggered automatically
      store.loadProducts(store.filters);
    }
  })
);
