import {ProductListItemDto} from '../dto/product-list-item.dto';
import {ProductQuery} from '../dto/product-query.dto';
import {ProductDetailsDto} from '../dto/product-details.dto';

export interface ProductState {
  products: ProductListItemDto[];
  totalCount: number;
  filters: ProductQuery;
  selectedProduct: ProductDetailsDto | null;
  loading: boolean;
  error: string | null;
}
