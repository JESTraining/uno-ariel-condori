import { NgClass } from '@angular/common';
import { Component, computed, input, output } from '@angular/core';
import { ProductListItemDto } from '../../../../data/dto/product-list-item.dto';

@Component({
  selector: 'app-low-stock-card',
  standalone: true,
  imports: [NgClass],
  templateUrl: './low-stock-card.component.html',
  styleUrl: './low-stock-card.component.scss'
})

export class LowStockCardComponent {
  product = input.required<ProductListItemDto>();
  triggerRestock = output<ProductListItemDto>();

  ratio = computed(() => {
    const threshold = this.product().reorderThreshold;
    if (threshold === 0) return 0;
    const calc = this.product().currentStock / threshold;
    return calc > 1 ? 1 : calc;
  });

  isCritical = computed(() => this.ratio() <= 0.25);

  onRestockClick(): void {
    this.triggerRestock.emit(this.product());
  }
}