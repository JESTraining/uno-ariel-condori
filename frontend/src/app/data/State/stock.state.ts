import { ProductListItemDto } from "../dto/product-list-item.dto";
import { StockMovementDto } from "../dto/stock-movement.dto";

export interface StockState {
  history: StockMovementDto[];
  lowStockAlerts: ProductListItemDto[];
  loading: boolean;
  error: string | null;
  success: boolean;
}