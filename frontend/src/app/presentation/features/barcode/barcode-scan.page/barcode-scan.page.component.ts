import { CurrencyPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { BarcodeScanStore } from '../../../../stores/barcode-scan.store';
import { BarcodeInputComponent } from '../barcode-input/barcode-input.component';
import { QuickAdjustmentComponent } from '../quick-adjustment/quick-adjustment.component';
import { InventoryErrorBannerComponent } from '../../../shared/inventory-error-banner/inventory-error-banner.component';

@Component({
  selector: 'app-barcode-scan.page',
  standalone: true,
  imports: [
    BarcodeInputComponent, 
    QuickAdjustmentComponent, 
    CurrencyPipe, 
    InventoryErrorBannerComponent
  ],
  providers: [BarcodeScanStore],
  templateUrl: './barcode-scan.page.component.html',
  styleUrl: './barcode-scan.page.component.scss'
})

export class BarcodeScanPage {
  protected store = inject(BarcodeScanStore);

  onResyncScannedItem() {
    const barcode = this.store.scannedProduct()?.barcode;

    if (barcode) {
      this.store.lookupBarcode(barcode);
    }
  }
}