import {Component, computed, input, output} from '@angular/core';
import {CurrencyPipe} from '@angular/common';
import {ProductListItemDto} from '../../../../data/dto/product-list-item.dto';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-product-table',
  standalone: true,
  imports: [
    CurrencyPipe,
    RouterLink
  ],
  templateUrl: './product-table.component.html',
  styleUrl: './product-table.component.scss'
})
export class ProductTableComponent {
  products = input.required<ProductListItemDto[]>();
  totalCount = input.required<number>();
  currentPage = input.required<number>();
  pageSize = input.required<number>();

  edit = output<string>();
  delete = output<string>();
  pageChange = output<number>();

  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()) || 1);
  isFirstPage = computed(() => this.currentPage() === 1);
  isLastPage = computed(() => this.currentPage() >= this.totalPages());

  changePage(offset: number): void {
    this.pageChange.emit(this.currentPage() + offset);
  }
}
