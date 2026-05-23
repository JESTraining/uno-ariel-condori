import { Component, DestroyRef, inject, input, output, OnInit } from '@angular/core';
import { AbstractControl, NonNullableFormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-quick-adjustment',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './quick-adjustment.component.html',
  styleUrl: './quick-adjustment.component.scss'
})

export class QuickAdjustmentComponent implements OnInit {
  private fb = inject(NonNullableFormBuilder);
  private destroyRef = inject(DestroyRef);
  private quantityValidator: ValidatorFn = Validators.nullValidator;
  
  currentStock = input.required<number>();
  submitAdjustment = output<{ quantity: number; reason: string }>();

  adjustForm = this.fb.group({
    quantity: [1, [Validators.required]],
    reason: ['received', [Validators.required]]
  });

  ngOnInit(): void {
    const currentStock = this.currentStock();

    this.quantityValidator = (control: AbstractControl): ValidationErrors | null => {
      const quantity = Number(control.value);
      const reason = this.adjustForm.controls.reason.value;

      if (!Number.isFinite(quantity)) {
        return { quantityInvalid: true };
      }

      if (reason === 'shipped') {
        if (quantity <= 0) {
          return { quantityMustBePositive: true };
        }

        return quantity > currentStock ? { quantityExceedsStock: true } : null;
      }

      if (reason === 'adjustment') {
        if (quantity === 0) {
          return { quantityCannotBeZero: true };
        }

        if (quantity < 0) {
          return currentStock + quantity < 0 ? { quantityExceedsStock: true } : null;
        }

        return null;
      }

      return quantity > 0 ? null : { quantityMustBePositive: true };
    };

    this.adjustForm.controls.quantity.setValidators([Validators.required, this.quantityValidator]);
    this.adjustForm.controls.quantity.updateValueAndValidity({ emitEvent: false });

    this.adjustForm.controls.reason.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.adjustForm.controls.quantity.updateValueAndValidity();
      });
  }

  onFormSubmit(): void {
    if (this.adjustForm.invalid) return;

    const values = this.adjustForm.getRawValue();
    
    this.submitAdjustment.emit({
      quantity: values.quantity,
      reason: values.reason
    });

    this.adjustForm.patchValue({ quantity: 1, reason: 'received' });
    this.adjustForm.markAsPristine();
    this.adjustForm.markAsUntouched();
  }
}