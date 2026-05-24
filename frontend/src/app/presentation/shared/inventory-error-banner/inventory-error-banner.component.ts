import { Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'app-inventory-error-banner',
  standalone: true,
  imports: [],
  templateUrl: './inventory-error-banner.component.html',
  styleUrl: './inventory-error-banner.component.scss'
})

export class InventoryErrorBannerComponent {
  error = input<string | null>(null);
  
  retrySync = output<void>();

  protected isConcurrencyConflict = computed(() => {
    const message = this.error();
    if (!message) return false;
    
    const cleanError = message.toLowerCase();
    return cleanError.includes('modified') || cleanError.includes('retry') || cleanError.includes('conflict');
  });
}