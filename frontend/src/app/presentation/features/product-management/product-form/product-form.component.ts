import { Component, effect, inject, input, output } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductDetailsDto } from '../../../../data/dto/product-details.dto';
import { CreateProductRequest } from '../../../../data/requests/create-product.request';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss'
})

export class ProductFormComponent {
  private fb = inject(NonNullableFormBuilder);

  productToEdit = input<ProductDetailsDto | null>(null);
  
  save = output<CreateProductRequest>();
  cancel = output<void>();

  isEditMode = () => !!this.productToEdit();

  productForm = this.fb.group({
    sku: ['', [Validators.required, Validators.minLength(3)]],
    barcode: [null as string | null],
    name: ['', [Validators.required]],
    category: ['', [Validators.required]],
    location: ['', [Validators.required]],
    price: [0, [Validators.required, Validators.min(0.01)]],
    reorderThreshold: [10, [Validators.required, Validators.min(0)]],
    version: [ this.productToEdit()?.version || '00000000-0000-0000-0000-000000000000' ]
  });

  constructor() {
    effect(() => {
      const product = this.productToEdit();
      if (product) {
        this.productForm.patchValue(product);
        this.productForm.controls.sku.disable();
      } else {
        this.productForm.reset({ price: 0, reorderThreshold: 10 });
        this.productForm.controls.sku.enable();
      }
    });
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      const rawValues = this.productForm.getRawValue();
      this.save.emit(rawValues as CreateProductRequest);
    }
  }
}