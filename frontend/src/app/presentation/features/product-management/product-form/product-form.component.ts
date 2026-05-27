import { Component, effect, inject, input, OnInit, output, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductDetailsDto } from '../../../../data/dto/product-details.dto';
import { CreateProductRequest } from '../../../../data/requests/create-product.request';
import { CatalogService } from '../../../../services/catalog.service';
import { CatalogItem } from '../../../../data/models/catalog.model';
import { forkJoin, merge } from 'rxjs';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss'
})

export class ProductFormComponent implements OnInit {
  private fb = inject(NonNullableFormBuilder);
  private catalogService = inject(CatalogService);

  categories = signal<CatalogItem[]>([]);
  locations = signal<CatalogItem[]>([]);
  loading = signal(true);

  productToEdit = input<ProductDetailsDto | null>(null);
  
  save = output<CreateProductRequest>();
  cancel = output<void>();

  isEditMode = () => !!this.productToEdit();

  productForm = this.fb.group({
    sku: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(64)]],
    barcode: [null as string | null],
    name: ['', [Validators.required]],
    category: ['', [Validators.required]],
    location: ['', [Validators.required]],
    price: [0, [Validators.required, Validators.min(0.01)]],
    reorderThreshold: [10, [Validators.required, Validators.min(0)]],
    version: [ this.productToEdit()?.version || '00000000-0000-0000-0000-000000000000' ]
  });

  ngOnInit(): void {
    forkJoin({
      categories: this.catalogService.getCategories(),
      locations: this.catalogService.getLocations()
    }).subscribe({
      next: (data) => {
        this.categories.set(data.categories);
        this.locations.set(data.locations);
        this.loading.set(false);
        this.setupSkuAutoGeneration();
      },
      error: () => this.loading.set(false)
    });
  }

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

  setupSkuAutoGeneration(): void {
    if(this.isEditMode()) 
      return;

    merge(
      this.productForm.controls.category.valueChanges,
      this.productForm.controls.location.valueChanges
    ).subscribe(() => {
      const categoryId = this.productForm.controls.category.value;
      const locationId = this.productForm.controls.location.value;

      if (categoryId && locationId) {
        const generatedSku = `${categoryId}-${locationId}-${Math.floor(1000 + Math.random() * 9000)}`;
        
        this.productForm.controls.sku.setValue(generatedSku, { emitEvent: false });
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