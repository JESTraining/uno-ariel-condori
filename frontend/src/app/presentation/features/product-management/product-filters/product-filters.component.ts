import {Component, DestroyRef, inject, input, OnInit, output} from '@angular/core';
import {debounceTime, distinctUntilChanged} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {ProductQuery} from '../../../../data/dto/product-query.dto';
import {NonNullableFormBuilder, ReactiveFormsModule} from '@angular/forms';

@Component({
  selector: 'app-product-filters',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './product-filters.component.html',
  styleUrl: './product-filters.component.scss'
})

export class ProductFiltersComponent implements OnInit {
  private fb = inject(NonNullableFormBuilder);
  private destroyRef = inject(DestroyRef);

  initialFilters = input.required<ProductQuery>();
  filterChange = output<Partial<ProductQuery>>();

  filterForm = this.fb.group({
    search: [''],
    category: ['']
  });

  ngOnInit(): void {
    this.filterForm.patchValue({
      search: this.initialFilters().search || '',
      category: this.initialFilters().category || ''
    }, { emitEvent: false });

    this.filterForm.valueChanges
      .pipe(
        debounceTime(350),
        distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(values => {
        this.filterChange.emit(values);
      });
  }
}
