import { AfterViewInit, Component, ElementRef, output, viewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-barcode-input',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './barcode-input.component.html',
  styleUrl: './barcode-input.component.scss'
})

export class BarcodeInputComponent implements AfterViewInit {
  barcodeValue = '';
  scan = output<string>();
  
  private inputElement = viewChild<ElementRef<HTMLInputElement>>('scanField');

  ngViewInit() {
  }

  ngAfterViewInit(): void {
    this.inputElement()?.nativeElement.focus();
  }

  onScanTrigger(): void {
    const trimmed = this.barcodeValue.trim();
    if (trimmed) {
      this.scan.emit(trimmed);
      this.barcodeValue = '';
    }
  }
}