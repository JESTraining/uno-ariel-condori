import { inject } from "@angular/core";
import { signalStore, withState, withMethods, patchState } from "@ngrx/signals";
import { rxMethod } from "@ngrx/signals/rxjs-interop";
import { pipe, tap, switchMap, catchError, of } from "rxjs";
import { BarcodeScanState } from "../data/State/barcode-scan.state";
import { StockService } from "../services/stock.service";
import { ProductService } from "../services/product.service";
import { StockMovementResultDto } from "../data/dto/stock-movement.result.dto";
import { ProductStore } from "./product.store";

const initialState: BarcodeScanState = {
  scannedProduct: null,
  loading: false,
  error: null,
  successMessage: null,
};

export const BarcodeScanStore = signalStore(
  withState(initialState),
  withMethods((store, stockService = inject(StockService), 
    productService = inject(ProductService),
    productStore = inject(ProductStore)) => {
    
    const lookupBarcode = rxMethod<string>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null, successMessage: null, scannedProduct: null })),
        switchMap((barcode) =>
          productService.getProductByBarcode(barcode).pipe(
            tap((product) => patchState(store, { scannedProduct: product, loading: false })),
            catchError(() => {
              patchState(store, { 
                error: 'Product not found or invalid barcode.', 
                loading: false 
              });
              return of(null); 
            })
          )
        )
      )
    );

    return {
      lookupBarcode,
      resetStore(): void {
        patchState(store, initialState);
      },
      applyAdjustment(quantityChange: number, reason: string): void {
        const product = store.scannedProduct();
        if (!product) 
            return;
        
        if (product.currentStock + quantityChange < 0) {
          patchState(store, { error: 'Transaction aborted: Stock cannot fall below zero.' });
          return;
        }

        patchState(store, { loading: true, error: null, successMessage: null });
        
        stockService.saveMovement({ productId: product.id, quantityChange, reason, version: product.version }).subscribe({
          next: (response: StockMovementResultDto) => {
            productStore.updateProductFromMovement(product.id, response.newStock, response.version);
            patchState(store, (state) => ({
              loading: false,
              successMessage: 'Stock adjusted successfully!',
              scannedProduct: state.scannedProduct 
                ? { ...state.scannedProduct, currentStock: response.newStock, version: response.version }
                : null
            }));
          },
          error: (err: Error) => patchState(store, { error: 'Failed: ' + err.message, loading: false })
        });
      }
    };
  })
);