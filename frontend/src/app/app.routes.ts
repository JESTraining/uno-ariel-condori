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
  }
];
