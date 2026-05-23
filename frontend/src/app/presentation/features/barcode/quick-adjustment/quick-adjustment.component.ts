import { Component, DestroyRef, inject, input, output } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

function quantityValidator(control: AbstractControl): ValidationErrors | null {
  const quantity = Number(control.value);
  const reason = control.parent?.get('reason')?.value;

  if (!Number.isFinite(quantity)) {
    return { quantityInvalid: true };
  }

  if (reason === 'adjustment') {
    return quantity === 0 ? { quantityCannotBeZero: true } : null;
  }

  return quantity > 0 ? null : { quantityMustBePositive: true };
}

@Component({
  selector: 'app-quick-adjustment',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './quick-adjustment.component.html',
  styleUrl: './quick-adjustment.component.scss'
})

export class QuickAdjustmentComponent {
  private fb = inject(NonNullableFormBuilder);
  private destroyRef = inject(DestroyRef);
  
  currentStock = input.required<number>();
  submitAdjustment = output<{ quantity: number; reason: string }>();

  adjustForm = this.fb.group({
    quantity: [1, [Validators.required, quantityValidator]],
    reason: ['received', [Validators.required]]
  });

  constructor() {
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

    this.adjustForm.patchValue({ quantity: 0 });
    this.adjustForm.markAsPristine();
    this.adjustForm.markAsUntouched();
  }
}