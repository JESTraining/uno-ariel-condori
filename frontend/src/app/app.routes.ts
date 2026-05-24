import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'products',
    pathMatch: 'full'
  },
  {
    path: 'products',
    loadComponent: () =>
      import('./presentation/features/product-management/product-list.page/product-list.page.component')
        .then(m => m.ProductListPage)
  },
  {
    path: 'barcode-scan',
    loadComponent: () => 
      import('./presentation/features/barcode/barcode-scan.page/barcode-scan.page.component')
        .then(m => m.BarcodeScanPage)
  },
  {
    path: 'stock-movements',
    loadComponent: () => 
      import('./presentation/features/stock-movement/stock-movement.page/stock-movement.page.component')
        .then(m => m.StockMovementPage)
  }
];
