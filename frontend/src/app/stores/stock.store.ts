import { inject } from "@angular/core";
import { signalStore, withState, withMethods, patchState } from "@ngrx/signals";
import { rxMethod } from "@ngrx/signals/rxjs-interop";
import { pipe, tap, switchMap } from "rxjs";
import { StockState } from "../data/State/stock.state";
import { StockService } from "../services/stock.service";
import { StockMovementRequest } from "../data/requests/stock-movement.request";
import { ProductStore } from "./product.store";
import { StockMovementResultDto } from "../data/dto/stock-movement.result.dto";

const initialState: StockState = {
  history: [],
  lowStockAlerts: [],
  loading: false,
  error: null,
  success: false,
};

export const StockStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, stockService = inject(StockService), productStore = inject(ProductStore)) => {
    
    const loadHistory = rxMethod<string>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null, success: false })),
        switchMap((productId) =>
          stockService.getMovementHistory(productId).pipe(
            tap({
              next: (historyItems) => patchState(store, { history: historyItems, loading: false }),
              error: () => patchState(store, { error: 'Failed to retrieve stock history log.', loading: false })
            })
          )
        )
      )
    );

    const loadLowStockAlerts = rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() =>
          stockService.getLowStockAlerts().pipe(
            tap({
              next: (alerts) => patchState(store, { lowStockAlerts: alerts, loading: false }),
              error: (err: Error) => patchState(store, { error: 'Failed to load low stock alerts. ' + err.message, loading: false })
            })
          )
        )
      )
    );

    return {
      loadHistory,
      loadLowStockAlerts,

      executeMovement(request: StockMovementRequest, onSuccess: () => void): void {
        if (store.loading()) return;

        patchState(store, { loading: true, error: null, success: false });

        stockService.saveMovement(request).subscribe({
          next: (response: StockMovementResultDto) => {
            patchState(store, { success: true });
            productStore.updateProductFromMovement(request.productId, response.newStock, response.version);
            loadHistory(request.productId);
            onSuccess();
          },
          error: (err: Error) => patchState(store, { error: 'Error in stock movement. ' + err.message, loading: false })
        });
      }
    };
  })
);