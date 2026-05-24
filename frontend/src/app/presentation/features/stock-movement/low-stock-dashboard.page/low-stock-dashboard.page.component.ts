import { Component, inject, OnInit } from '@angular/core';
import { ProductListItemDto } from '../../../../data/dto/product-list-item.dto';
import { StockStore } from '../../../../stores/stock.store';
import { LowStockCardComponent } from '../low-stock-card/low-stock-card.component';

@Component({
  selector: 'app-low-stock-dashboard.page',
  standalone: true,
  imports: [LowStockCardComponent],
  templateUrl: './low-stock-dashboard.page.component.html',
  styleUrl: './low-stock-dashboard.page.component.scss'
})

export class LowStockDashboardPage implements OnInit {
  protected stockStore = inject(StockStore);

  ngOnInit(): void {
    this.stockStore.loadLowStockAlerts();
  }

  handleSimulatedRestock(product: ProductListItemDto): void {
    console.log(`📦 [Simulated Procurement Sync] Restock Request Triggered`);
    console.log(product);

    alert(`[SIMULATION] Restock request dispatched successfully for ${product.name}. Check the browser developer tools console for payload records.`);
  }
}