import { Component, effect, inject, input, output, signal } from '@angular/core';
import { ReactiveFormsModule, NonNullableFormBuilder, Validators } from '@angular/forms';
import { ProductListItemDto } from '../../../../data/dto/product-list-item.dto';
import { StockMovementRequest } from '../../../../data/requests/stock-movement.request';

@Component({
  selector: 'app-movement-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './movement-form.component.html',
  styleUrl: './movement-form.component.scss'
})

export class MovementFormComponent {
  private fb = inject(NonNullableFormBuilder);
  
  selectedProduct = input<ProductListItemDto | null>(null);
  submitMovement = output<{ request: StockMovementRequest; onSuccess: () => void }>();

  underflowError = signal<boolean>(false);

  movementForm = this.fb.group({
    reason: ['received', [Validators.required]],
    quantity: [0, [Validators.required, Validators.min(1)]],
  });

  constructor() {
    // Reset validations whenever the user selects a different product
    effect(() => {
      if (this.selectedProduct()) {
        this.validateNegativeStock();
      }
    }, { allowSignalWrites: true });
  }

  validateNegativeStock(): void {
    const product = this.selectedProduct();
    if (!product) return;

    const action = this.movementForm.controls.reason.value;
    const qty = this.movementForm.controls.quantity.value;

    if (action === 'shipped' && qty > product.currentStock) {
      this.underflowError.set(true);
    } else {
      this.underflowError.set(false);
    }
  }

  onSubmit(): void {
    const product = this.selectedProduct();
    if (this.movementForm.invalid || this.underflowError() || !product) return;

    const values = this.movementForm.getRawValue();

    const req: StockMovementRequest = {
      productId: product.id,
      quantityChange: values.quantity,
      reason: values.reason,
      version: product.version
    };

    const onSuccess = () => {
      this.movementForm.controls.reason.reset();
      this.movementForm.controls.quantity.setValue(0);
    };

    this.submitMovement.emit({ request: req, onSuccess });
  }
}